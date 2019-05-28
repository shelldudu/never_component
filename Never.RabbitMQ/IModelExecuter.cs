using System;
using Never.Messages;
using Never.Serialization;
using RabbitMQ.Client;

namespace Never.RabbitMQ
{
    /// <summary>
    /// IModel包装
    /// </summary>
    internal sealed class IModelExecuter : IDisposable
    {
        #region prop

        /// <summary>
        /// 频道
        /// </summary>
        public IModel Model { get; private set; }

        #endregion prop

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="IModelExecuter"/> class.
        /// </summary>
        /// <param name="model">The model.</param>
        public IModelExecuter(IModel model)
        {
            this.Model = model;
        }

        #endregion ctor

        #region dispose

        /// <summary>
        /// 开始工作
        /// </summary>
        public IModelExecuter Start(IBinarySerializer binarySerializer, MessagePacket message, MessageRoute route)
        {
            this.Model.BasicPublish(route.Exchange, route.RoutingKey, null, binarySerializer.Serialize(message));
            return this;
        }

        /// <summary>
        /// 释放繁忙状态
        /// </summary>
        public IModelExecuter Stop()
        {
            return this;
        }

        /// <summary>
        /// 执行与释放或重置非托管资源相关的应用程序定义的任务。
        /// </summary>
        public void Dispose()
        {
            if (this.Model == null)
                return;

            this.Model.Dispose();
        }

        #endregion dispose
    }
}