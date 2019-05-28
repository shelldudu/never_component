using Never.Commands;
using Never.Commands.Recovery;
using Never.CommandStreams;
using Never.Events;
using Never.EventStreams;
using Never.IoC;
using System;
using System.Collections.Generic;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// sqlite命令总线
    /// </summary>
    [System.ComponentModel.Browsable(false)]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public sealed class SqliteCommandBus : RecoveryCommandBus, ICommandBus
    {
        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqliteCommandBus"/> class.
        /// </summary>
        /// <param name="serviceLocator">命令提供者</param>
        /// <param name="recoveryStorager">事件存储</param>
        /// <param name="eventStorager">领域事件的保存方案</param>
        /// <param name="commandStorager">命令储存</param>
        public SqliteCommandBus(IServiceLocator serviceLocator,
            IEventStorager eventStorager,
            ICommandStorager commandStorager,
            SqliteFailRecoveryStorager recoveryStorager)
            : base(serviceLocator, eventStorager, commandStorager, (x, y) => new SqliteEventBus(x, y, recoveryStorager))
        {
        }

        #endregion ctor
    }
}