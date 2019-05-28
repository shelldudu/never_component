using Never.Messages;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 消息链接对象
    /// </summary>
    public class MessageConnection : DefaultMessageConnection, IMessageConnection
    {
        #region const

        /// <summary>
        /// 格式为 amqp://guest:guest@127.0.0.1:5672/
        /// </summary>
        public const string Sample_Route = @"amqp://guest:guest@127.0.0.1:5672/";

        #endregion const
    }
}