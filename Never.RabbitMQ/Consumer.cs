using System;
using System.Collections.Concurrent;
using Never.Messages;
using Never.Serialization;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 消费者
    /// </summary>
    public class Consumer : IMessageConsumer
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
        private readonly ConcurrentDictionary<string, Subscription> subPools = null;

        /// <summary>
        /// 接连池
        /// </summary>
        private readonly ConcurrentDictionary<string, EventingBasicConsumer> eventPools = null;

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
        /// <param name="messageRoute"></param>
        public Consumer(MessageConnection connection, MessageRoute messageRoute)
            : this(connection, messageRoute, new BinarySerializer())
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Producer"/> class.
        /// </summary>
        /// <param name="connection">The connection.</param>
        /// <param name="messageRoute"></param>
        /// <param name="binarySerializer"></param>
        public Consumer(MessageConnection connection, MessageRoute messageRoute, IBinarySerializer binarySerializer)
        {
            this.messageConnection = connection;
            this.messageRoute = messageRoute;
            this.factory = new ConnectionFactory() { Uri = new Uri(connection.ConnetctionString) };
            this.binarySerializer = binarySerializer;
            this.subPools = new ConcurrentDictionary<string, Subscription>(System.Environment.ProcessorCount, System.Environment.ProcessorCount * 2);
            this.eventPools = new ConcurrentDictionary<string, EventingBasicConsumer>(System.Environment.ProcessorCount, System.Environment.ProcessorCount * 2);
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

        #region 同步接受

        /// <summary>
        /// 接收一条消息
        /// </summary>
        /// <returns></returns>
        public virtual MessagePacket Receive()
        {
            Subscription model = null;
            var route = this.ExchangeMessageRoute(null);
            var key = route.ToString();
            if (!this.connection.IsOpen)
            {
                try
                {
                    this.connection.Dispose();
                    this.subPools.TryRemove(key, out model);
                }
                catch
                {
                }
                finally
                {
                    this.Restart();
                }
            }

            if (!this.subPools.TryGetValue(key, out model))
            {
                lock (this)
                {
                    if (!this.subPools.TryGetValue(key, out model))
                        this.subPools.TryAdd(key, model = this.CreateSubscription(route));
                }
            }

            if (model == null)
                throw new ArgumentNullException("can not init the Subscription");

            try
            {
                var args = model.Next();
                if (args != null)
                {
                    model.Model.BasicAck(args.DeliveryTag, false);
                    return this.binarySerializer.Deserialize<MessagePacket>(args.Body);
                }

                return null;
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        #endregion 同步接受

        #region 异步

        /// <summary>
        /// 异步接收一条消息
        /// </summary>
        /// <param name="messageCallback">回调</param>
        public virtual void ReceiveAsync(Action<MessagePacket> messageCallback)
        {
            EventingBasicConsumer model = null;
            var route = this.ExchangeMessageRoute(null);
            var key = route.ToString();
            if (!this.connection.IsOpen)
            {
                try
                {
                    this.connection.Dispose();
                    this.eventPools.TryRemove(key, out model);
                }
                catch
                {
                }
                finally
                {
                    this.Restart();
                }
            }

            if (!this.eventPools.TryGetValue(key, out model))
            {
                lock (this)
                {
                    if (!this.eventPools.TryGetValue(key, out model))
                        this.eventPools.TryAdd(key, model = this.CreateEventingBasic(route));
                }
            }

            if (model == null)
                throw new ArgumentNullException("can not init the EventingBasicConsumer");

            try
            {
                model.Received += (s, e) =>
                {
                    ((EventingBasicConsumer)s).Model.BasicAck(e.DeliveryTag, false);
                    messageCallback(this.binarySerializer.Deserialize<MessagePacket>(e.Body));
                };

                model.Model.BasicConsume(route.QueueName, false, model);
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        #endregion 异步

        #region create

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        protected virtual IModel CreateModel(MessageRoute route)
        {
            if (route.QueueName.IsNullOrEmpty())
                throw new ArgumentNullException("Route.QueueName", "queue name is null;");

            var model = this.connection.CreateModel();
            model.ExchangeDeclare(route.Exchange, route.ExchangeType.ToString().ToLower(), route.Durable, route.AutoDelete, null);
            model.QueueDeclare(route.QueueName, route.Durable, route.QueueExclusive, route.AutoDelete, null);
            model.QueueBind(route.QueueName, route.Exchange, route.BindingKey, null);

            if (route.PrefetchCount > 0)
                model.BasicQos(0, route.PrefetchCount, false);

            return model;
        }

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private EventingBasicConsumer CreateEventingBasic(MessageRoute route)
        {
            return new EventingBasicConsumer(this.CreateModel(route));
        }

        /// <summary>
        /// 创建Model
        /// </summary>
        /// <param name="route"></param>
        /// <returns></returns>
        private Subscription CreateSubscription(MessageRoute route)
        {
            return new Subscription(this.CreateModel(route), route.QueueName, false);
        }

        #endregion create
    }
}