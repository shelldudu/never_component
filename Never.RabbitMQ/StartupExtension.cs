using Never.Messages;
using Never.Serialization;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动rabbitmq消费者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="route">路由</param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQConsumer(this ApplicationStartup startup, MessageConnection connection, string key, MessageRoute route)
        {
            return UseRabbitMQConsumer(startup, connection, key, route, new BinarySerializer());
        }

        /// <summary>
        /// 启动rabbitmq消费者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="route">路由</param>
        /// <param name="binarySerializer"></param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQConsumer(this ApplicationStartup startup, MessageConnection connection, string key, MessageRoute route, IBinarySerializer binarySerializer)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseRabbitMQConsumer" + key))
                return startup;

            var consumer = new Consumer(connection, route, binarySerializer ?? new BinarySerializer());
            startup.ServiceRegister.RegisterInstance(consumer, typeof(IMessageConsumer), key);

            startup.Items["UseMessageRouteRabbitMQConsumer" + key] = "t";
            return startup;
        }

        /// <summary>
        /// 启动rabbitmq生产者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQProducer(this ApplicationStartup startup, MessageConnection connection, string key)
        {
            return UseRabbitMQProducer(startup, connection, key, new BinarySerializer());
        }

        /// <summary>
        /// 启动msmq生产者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="binarySerializer">路由</param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQProducer(this ApplicationStartup startup, MessageConnection connection, string key, IBinarySerializer binarySerializer)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseRabbitMQProducer" + key))
                return startup;

            var producer = new Producer(connection, binarySerializer ?? new BinarySerializer());
            startup.ServiceRegister.RegisterInstance(producer, typeof(IMessageProducer), key);
            startup.Items["UseRabbitMQProducer" + key] = "t";
            return startup;
        }

        /// <summary>
        /// 启动rabbitmq生产者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="route"></param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQProducer(this ApplicationStartup startup, MessageConnection connection, string key, MessageRoute route)
        {
            return UseRabbitMQProducer(startup, connection, key, route, new BinarySerializer());
        }

        /// <summary>
        /// 启动rabbitmq生产者支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="connection">msmq配置的队列名文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="route"></param>
        /// <param name="binarySerializer"></param>
        /// <returns></returns>
        public static ApplicationStartup UseRabbitMQProducer(this ApplicationStartup startup, MessageConnection connection, string key, MessageRoute route, IBinarySerializer binarySerializer)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseRabbitMQProducerRoute" + key))
                return startup;

            var producer = new Producer(connection, route, binarySerializer ?? new BinarySerializer());
            startup.ServiceRegister.RegisterInstance(producer, typeof(IMessageProducer), key);
            startup.Items["UseRabbitMQProducerRoute" + key] = "t";
            return startup;
        }
    }
}