using System;
using System.Collections.Concurrent;
using Never.Messages;
using Never.Serialization;
using RabbitMQ.Client;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class Producer : IMessageProducer
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IConnectionFactory factory = null;

        /// <summary>
        ///
        /// </summary>
        private readonly IMessageConnection messageConnection = null;

        /// <summary>
        /// The object serializer
        /// </summary>
        private readonly IBinarySerializer binarySerializer = null;

        /// <summary>
        /// 接连池
        /// </summary>
        private readonly ConcurrentDictionary<string, ConcurrentQueue<IModel>> pools = null;

        /// <summary>
        ///
        /// </summary>
        private readonly MessageRoute messageRoute = null;

        /// <summary>
        /// mq connection
        /// </summary>
        private IConnection connection = null;

        /// <summary>
        /// 是否启动
        /// </summary>
        private bool isStart = false;

        /// <summary>
        /// 是否关闭
        /// </summary>
        private bool isShutDown = false;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        public Producer(MessageConnection connection)
            : this(connection, null, new BinarySerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="messageRoute"></param>
        public Producer(MessageConnection connection, MessageRoute messageRoute)
            : this(connection, messageRoute, new BinarySerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="binarySerializer"></param>
        public Producer(MessageConnection connection, IBinarySerializer binarySerializer)
            : this(connection, null, binarySerializer)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="messageRoute"></param>
        /// <param name="binarySerializer"></param>
        public Producer(MessageConnection connection, MessageRoute messageRoute, IBinarySerializer binarySerializer)
        {
            this.messageConnection = connection;
            this.messageRoute = messageRoute;
            this.factory = new ConnectionFactory() { Uri = new Uri(connection.ConnetctionString) };
            this.binarySerializer = binarySerializer;
            this.pools = new ConcurrentDictionary<string, ConcurrentQueue<IModel>>(System.Environment.ProcessorCount, System.Environment.ProcessorCount * 2);
        }

        #endregion ctor

        #region 路由

        /// <summary>
        /// 消息路由
        /// </summary>
        public virtual IMessageConnection MessageConnection
        {
            get { return this.messageConnection; }
        }

        /// <summary>
        /// 交换路由
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        protected virtual MessageRoute ExchangeMessageRoute(IMessageRoute route)
        {
            if (route == null)
                return this.messageRoute ?? new MessageRoute();

            return (route as MessageRoute) ?? new MessageRoute();
        }

        #endregion 路由

        #region 启动与关闭

        /// <summary>
        /// 启动
        /// </summary>
        public virtual void Startup()
        {
            if (this.isStart)
                return;

            this.connection = this.factory.CreateConnection();
            this.isStart = true;
        }

        /// <summary>
        /// 重启
        /// </summary>
        private void Restart()
        {
            this.connection = this.factory.CreateConnection();
            this.isStart = true;
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public virtual void Shutdown()
        {
            if (this.isShutDown)
                return;

            this.connection.Close();
            this.isShutDown = true;
        }

        #endregion 启动与关闭

        #region 发送信息

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <exception cref="InvalidCastException">route对象不能从BCB.RabbitMQ.MessageRoute实例分配</exception>
        public virtual void Send(MessagePacket message)
        {
            this.Send(message, this.ExchangeMessageRoute(null));
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route">消息路由</param>
        public virtual void Send(MessagePacket message, IMessageRoute route)
        {
            this.Send(message, this.ExchangeMessageRoute(route));
        }

        /// <summary>
        /// 发送一条消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="route">消息路由</param>
        private void Send(MessagePacket message, MessageRoute route)
        {
            ConcurrentQueue<IModel> queues = null;
            var key = route.ToString();
            if (!this.pools.TryGetValue(key, out queues))
                this.pools.TryAdd(key, queues = new ConcurrentQueue<IModel>());

            queues.TryDequeue(out IModel model);
            if (model == null)
                model = this.CreateModel(route);

            try
            {
                model.BasicPublish(route.Exchange, route.RoutingKey, null, this.binarySerializer.Serialize(message));
            }
            catch
            {
                throw;
            }
            finally
            {
                queues.Enqueue(model);
            }
        }

        #endregion 发送信息

        #region create

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        protected virtual IModel CreateModel(MessageRoute route)
        {
            var model = this.connection.CreateModel();
            model.ExchangeDeclare(route.Exchange, route.ExchangeType.ToString().ToLower(), route.Durable, route.AutoDelete, null);
            if (route.QueueName.IsNotNullOrEmpty())
            {
                model.QueueDeclare(route.QueueName, route.Durable, route.QueueExclusive, route.AutoDelete, null);
                model.QueueBind(route.QueueName, route.Exchange, route.BindingKey, null);
            }

            return model;
        }

        #endregion create
    }
}