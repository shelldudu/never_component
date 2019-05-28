using Never.Commands;
using Never.Commands.Recovery;
using Never.Events;
using Never.Security;
using Never.Serialization;
using Never.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Never.SqliteRecovery
{
    /// <summary>
    /// sqlite恢复保存者
    /// </summary>
    public class SqliteFailRecoveryStorager : IFailRecoveryStorager, IWorkTigger
    {
        #region field

        /// <summary>
        /// The command file
        /// </summary>
        private readonly FileInfo commandFile = null;

        /// <summary>
        /// The event file
        /// </summary>
        private readonly FileInfo eventFile = null;

        /// <summary>
        /// 定时时间
        /// </summary>
        public TimeSpan Timer { get; set; }

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="commandFile">命令数据库文件</param>
        /// <param name="eventFile">事件数据库文件</param>
        public SqliteFailRecoveryStorager(FileInfo commandFile, FileInfo eventFile)
        {
            this.commandFile = commandFile;
            this.eventFile = eventFile;
            if (!this.commandFile.Exists)
                throw new FileNotFoundException("commandFile is not exists");

            if (!this.eventFile.Exists)
                throw new FileNotFoundException("eventFile is not exists");

            this.Timer = TimeSpan.FromSeconds(10);
        }

        /// <summary>
        /// 保存一个命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="communication">通讯上下文</param>
        /// <param name="exception">异常信息</param>
        public void Enqueue(HandlerCommunication communication, Exception exception, ICommand command)
        {
            this.Enqueue(communication.Worker, exception, command);
        }

        /// <summary>
        /// 保存一个命令
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        public void Enqueue(IEventContext context, Exception exception, ICommand command)
        {
            this.Enqueue(context.Worker, exception, command);
        }

        /// <summary>
        /// 保存一个命令
        /// </summary>
        public void Enqueue(IWorker worker, Exception exception, ICommand command)
        {
            if (command == null)
                return;

            var type = command.GetType();
            var id = 0;
            if (worker is RecoveryWorker)
                id = ((RecoveryWorker)worker).AdditionId;

            var model = new SqliteReflectComandModel()
            {
                CommandContent = SerializeEnvironment.JsonSerializer.SerializeObject(command),
                CommandType = SerializeEnvironment.JsonSerializer.Serialize(type),
                ExceptionMessage = exception.GetMessage(),
                Id = id
            };

            using (var sql = new SqliteExecuter(string.Concat("data source=", this.commandFile.FullName)))
            {
                if (id > 0)
                {
                    var exits = sql.Update("update r_cmd set Disabled = 0 ,ActionCount = ActionCount + 1 ,ActionContinue = (case when ActionCount>=30 then 0 else 1 end),ExceptionMessage = @ExceptionMessage where Id = @Id; ", new { Id = model.Id, ExceptionMessage = model.ExceptionMessage });
                    if (exits > 0)
                        return;
                }

                sql.Insert("insert into r_cmd(CommandType,CommandContent,ExceptionMessage,Disabled,ActionCount,ActionContinue,CreateDate) values (@CommandType,@CommandContent,@ExceptionMessage,@Disabled,@ActionCount,@ActionContinue,@CreateDate)",
                    new
                    {
                        CommandType = model.CommandType,
                        CommandContent = model.CommandContent,
                        ExceptionMessage = model.ExceptionMessage,
                        Disabled = 0,
                        ActionCount = 0,
                        ActionContinue = 1,
                        CreateDate = DateTime.Now
                    });
            }
        }

        /// <summary>
        /// 返回命令处理
        /// </summary>
        /// <returns></returns>
        public RecoveryCommandModel DequeueCommand()
        {
            SqliteReflectComandModel model = null;
            using (var sql = new SqliteExecuter(string.Concat("data source=", this.commandFile.FullName)))
            {
                model = sql.QueryForObject<SqliteReflectComandModel>("select a.* from r_cmd as a where a.Disabled = 0 and a.ActionContinue = 1 order by id asc limit 1;", new { });
                if (model != null)
                    sql.Update("update r_cmd set Disabled = 1 where Id = @Id;", new { @Id = model.Id });
            }

            if (model == null)
                return default(RecoveryCommandModel);

            var type = Type.GetType(model.CommandType.Trim('"'));
            var command = SerializeEnvironment.JsonSerializer.DeserializeObject(model.CommandContent, type) as ICommand;
            return new RecoveryCommandModel()
            {
                Id = model.Id,
                UniqueId = Guid.Empty,
                Command = command
            };
        }

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        /// <param name="eventHandlerType">事件处理者</param>
        public void Enqueue(IEventContext context, Exception exception, IEvent @event, Type eventHandlerType)
        {
            this.Enqueue(context.Worker, exception, @event, eventHandlerType);
        }

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        /// <param name="context">上下文</param>
        /// <param name="exception">异常信息</param>
        /// <param name="event">事件</param>
        /// <param name="eventHandlerType">事件处理者</param>
        public void Enqueue(ICommandContext context, Exception exception, IEvent @event, Type eventHandlerType)
        {
            this.Enqueue(context.Worker, exception, @event, eventHandlerType);
        }

        /// <summary>
        /// 保存事件，用于重试
        /// </summary>
        public void Enqueue(IWorker worker, Exception exception, IEvent @event, Type eventHandlerType)
        {
            if (@event == null)
                return;

            var id = 0;
            if (worker is RecoveryWorker)
                id = ((RecoveryWorker)worker).AdditionId;

            var type = @event.GetType();
            var model = new SqliteReflectEventModel()
            {
                Id = id,
                ExceptionMessage = exception.GetMessage(),
                EventHandlerType = SerializeEnvironment.JsonSerializer.Serialize(eventHandlerType),
                EventContent = SerializeEnvironment.JsonSerializer.SerializeObject(@event),
                EventType = SerializeEnvironment.JsonSerializer.Serialize(type),
            };

            using (var sql = new SqliteExecuter(string.Concat("data source=", this.eventFile.FullName)))
            {
                if (id > 0)
                {
                    var exits = sql.Update("update r_event set Disabled = 0 ,ActionCount = ActionCount + 1 ,ActionContinue = (case when ActionCount>=30 then 0 else 1 end),ExceptionMessage = @ExceptionMessage where Id = @Id; ", new { Id = model.Id, ExceptionMessage = model.ExceptionMessage });
                    if (exits > 0)
                        return;
                }

                sql.Insert("insert into r_event(EventHandlerType,EventContent,EventType,ExceptionMessage,Disabled,ActionCount,ActionContinue,CreateDate) values (@EventHandlerType,@EventContent,@EventType,@ExceptionMessage,@Disabled,@ActionCount,@ActionContinue,@CreateDate)",
                    new
                    {
                        EventHandlerType = model.EventHandlerType,
                        EventContent = model.EventContent,
                        EventType = model.EventType,
                        ExceptionMessage = model.ExceptionMessage,
                        Disabled = 0,
                        ActionCount = 0,
                        ActionContinue = 1,
                        CreateDate = DateTime.Now
                    });
            }
        }


        /// <summary>
        /// 返回事件处理
        /// </summary>
        /// <returns></returns>
        public RecoveryEventModel DequeueEvent()
        {
            SqliteReflectEventModel model = null;
            using (var sql = new SqliteExecuter(string.Concat("data source=", this.eventFile.FullName)))
            {
                model = sql.QueryForObject<SqliteReflectEventModel>("select a.* from r_event as a where a.Disabled = 0 and a.ActionContinue = 1 order by id asc limit 1;", new { });
                if (model != null)
                    sql.Update("update r_event set Disabled = 1 where Id = @Id;", new { @Id = model.Id });
            }

            if (model == null)
                return default(RecoveryEventModel);

            var @event = SerializeEnvironment.JsonSerializer.DeserializeObject(model.EventContent, Type.GetType(model.EventType.Trim('"'))) as IEvent;
            return new RecoveryEventModel()
            {
                Id = model.Id,
                UniqueId = Guid.Empty,
                EventHandlerType = Type.GetType(model.EventHandlerType.Trim('"')),
                Event = @event
            };
        }

        #endregion ctor

        #region nested

        /// <summary>
        /// 存储在Sqlite内部的命令数据对象
        /// </summary>
        public class SqliteReflectComandModel
        {
            /// <summary>
            /// 命令对象
            /// </summary>
            public string CommandType { get; set; }

            /// <summary>
            /// 命令序列化后的内容
            /// </summary>
            public string CommandContent { get; set; }

            /// <summary>
            /// 消息内容
            /// </summary>
            public string ExceptionMessage { get; set; }

            /// <summary>
            /// Id
            /// </summary>
            public int Id { get; set; }
        }

        /// <summary>
        /// 存储在Sqlite内部的事件数据对象
        /// </summary>
        public class SqliteReflectEventModel
        {
            /// <summary>
            /// Id
            /// </summary>
            public int Id { get; set; }

            /// <summary>
            /// 事件类型
            /// </summary>
            public string EventType { get; set; }

            /// <summary>
            /// 事件内容（序列化后的）
            /// </summary>
            public string EventContent { get; set; }

            /// <summary>
            /// 消息内容
            /// </summary>
            public string ExceptionMessage { get; set; }

            /// <summary>
            /// 事件处理类型
            /// </summary>
            public string EventHandlerType { get; set; }
        }

        #endregion nested
    }
}