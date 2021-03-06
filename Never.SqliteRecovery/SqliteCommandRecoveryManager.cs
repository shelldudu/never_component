﻿using Never.Commands;
using Never.Commands.Recovery;
using Never.Events;
using Never.IoC;
using Never.Logging;
using System;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// 用于协助Command发布，失败后重试的机制
    /// </summary>
    public class SqliteCommandRecoveryManager : CommandRecoveryManager
    {
        #region ctor

        /// <summary>
        /// dozeTime = TimeSpan.FromMinutes(1)表示1分发一个命令，如果还有命令，则一直发（间隔为1秒），此时RepeatWork返回false
        /// waitTime = TimeSpan.Zero 当任务完成，则进入睡眠状态，直到被唤醒，此时RepeatWork返回true
        /// </summary>
        /// <param name="storager"></param>
        /// <param name="commandBus"></param>
        /// <param name="eventBus"></param>
        /// <param name="serviceLocator"></param>
        public SqliteCommandRecoveryManager(SqliteFailRecoveryStorager storager, IServiceLocator serviceLocator, ICommandBus commandBus, IEventBus eventBus)
            : base(storager, serviceLocator, commandBus, eventBus, null)
        {
        }

        #endregion ctor

        /// <summary>
        /// 初始化上下文
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        protected override HandlerCommunication GetCommandContext(RecoveryCommandModel model)
        {
            return new HandlerCommunication()
            {
                Worker = new RecoveryWorker()
                {
                    AdditionId = model.Id,
                    AdditionGuid = model.UniqueId,
                },
            };
        }
    }
}