using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Never.Commands;
using Never.Events;
using Never.IoC;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// db4o发布事件
    /// </summary>
    internal class SqliteEventBus : EventBus, IEventBus
    {
        private readonly SqliteEventRecoveryManager eventRecovery = null;
        private readonly SqliteCommandRecoveryManager commandRecovery = null;

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteEventBus"/> class.
        /// </summary>
        /// <param name="commandBus"></param>
        /// <param name="serviceLocator"></param>
        /// <param name="recoveryStorager">The recovery storager.</param>
        public SqliteEventBus(ICommandBus commandBus,
            IServiceLocator serviceLocator,
            SqliteFailRecoveryStorager recoveryStorager) : base(commandBus, serviceLocator)
        {
            this.commandRecovery = new SqliteCommandRecoveryManager(recoveryStorager, serviceLocator, commandBus, this);
            this.eventRecovery = new SqliteEventRecoveryManager(recoveryStorager, serviceLocator, commandBus, this);
            this.commandRecovery.Start();
            this.eventRecovery.Start();
        }

        #endregion ctor

        #region push

        public override void Push(ICommandContext context, IEnumerable<IEvent[]> events)
        {
            if (events == null)
                return;

            foreach (var splits in events)
            {
                if (splits.IsNullOrEmpty())
                    continue;

                var task = Task.Factory.StartNew(() => { });
                foreach (var split in splits)
                    task.ContinueWith((t) => this.PublishEvent(split, context));
            }
        }

        #endregion push

        #region invoke

        public override void MakeEventHandlerInvoke<TEvent>(TEvent e, EventExcutingElement element)
        {
            try
            {
                base.MakeEventHandlerInvoke(e, element);
            }
            catch (Exception ex)
            {
                this.eventRecovery.EnqueueEvent(e, element, ex);
                throw new Exception("", ex);
            }
        }

        protected override void OnSendingHandlerCommand(IEventContext context, ICommand command)
        {
            try
            {
                base.OnSendingHandlerCommand(context, command);
            }
            catch (Exception ex)
            {
                this.commandRecovery.EnqueueCommand(command, context, ex);
                throw new Exception("", ex);
            }
        }

        #endregion invoke
    }
}