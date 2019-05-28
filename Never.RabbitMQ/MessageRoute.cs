using System;
using Never.Messages;

namespace Never.RabbitMQ
{
    /// <summary>
    /// 消息路由，默认方式为广播
    /// </summary>
    public class MessageRoute : IMessageRoute, IEquatable<MessageRoute>
    {
        #region prop

        /// <summary>
        /// 队列名称
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 交换机名字
        /// </summary>
        public string Exchange { get; set; }

        /// <summary>
        /// 路由关键字
        /// </summary>
        public string RoutingKey { get; set; }

        /// <summary>
        /// 绑定路由关键字
        /// </summary>
        public string BindingKey { get; set; }

        /// <summary>
        /// 交换机类型
        /// </summary>
        public MessageRouteExchangeType ExchangeType { get; set; }

        #endregion prop

        #region addition

        /// <summary>
        /// 是否持久化
        /// </summary>
        public bool Durable { get; set; }

        /// <summary>
        /// 是否自动删除
        /// </summary>
        public bool AutoDelete { get; set; }

        /// <summary>
        /// queue是否为独享的
        /// </summary>
        public bool QueueExclusive { get; set; }

        /// <summary>
        /// 当处理过程中未响应到server，消费者最多消费的消息条数。如果设置为1，则意味着不要在同一时间给一个工作者发送多于1个的消息
        /// </summary>
        public ushort PrefetchCount { get; set; }

        #endregion addition

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageRoute"/> class.默认使用fanout广播交换方式
        /// </summary>
        public MessageRoute()
        {
            this.Exchange = "amq.fanout";
            this.QueueName = "";
            this.RoutingKey = string.Empty;
            this.ExchangeType = MessageRouteExchangeType.Fanout;
            this.PrefetchCount = 1;
        }

        #endregion ctor

        #region IEquatable

        /// <summary>
        ///
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(MessageRoute other)
        {
            return this.Exchange == other.Exchange
                    && this.ExchangeType == other.ExchangeType
                    && this.QueueName == other.QueueName
                    && this.BindingKey == other.BindingKey;
        }

        /// <summary>
        /// tostring
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("e{0},t{1},q{2},r{3},p{4}", this.Exchange, (int)this.ExchangeType, this.QueueName, this.BindingKey, this.PrefetchCount);
        }

        #endregion IEquatable
    }
}