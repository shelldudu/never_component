using Never.Logging;
using Never.QuartzNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Never.QuartzNET
{
    /// <summary>
    /// 写日志行为
    /// </summary>
    public class LoggerHealthReport : MemoryHealthReport, IJobHealthReport
    {
        #region ctor

        /// <summary>
        ///
        /// </summary>
        public LoggerHealthReport() : this(Never.Logging.LoggerBuilder.Empty)
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="loggerBuilder"></param>
        protected LoggerHealthReport(ILoggerBuilder loggerBuilder)
        {
            this.loggerBuilder = loggerBuilder;
        }

        #endregion ctor

        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly ILoggerBuilder loggerBuilder = null;

        #endregion field

        /// <summary>
        /// 日志
        /// </summary>
        public ILoggerBuilder LoggerBuilder
        {
            get
            {
                return this.loggerBuilder;
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="detail"></param>
        public override void CtorErrorReport(IJobExcuteDetail detail)
        {
            base.CtorErrorReport(detail);
            var job = new
            {
                JobType = detail.JobType,
                JobName = detail.JobName,
                Exception = detail.Exception,
                Heartbeat = detail.Heartbeat,
                JobDescn = detail.JobDescn,
                RunTime = detail.RunTime,
                JobCronSchedule = detail.JobCronSchedule,
            };

            if (detail.Exception.IsNotNullOrEmpty())
                this.loggerBuilder.Build(typeof(LoggerHealthReport)).Error(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(job));
            else
                this.loggerBuilder.Build(typeof(LoggerHealthReport)).Warn(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(job));
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="detail"></param>
        public override void Report(IJobExcuteDetail detail)
        {
            base.Report(detail);
            var job = new
            {
                JobType = detail.JobType,
                JobName = detail.JobName,
                Exception = detail.Exception,
                Heartbeat = detail.Heartbeat,
                JobDescn = detail.JobDescn,
                RunTime = detail.RunTime,
                JobCronSchedule = detail.JobCronSchedule,
            };

            if (detail.Exception.IsNotNullOrEmpty())
                this.loggerBuilder.Build(typeof(LoggerHealthReport)).Error(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(job));
            else
                this.loggerBuilder.Build(typeof(LoggerHealthReport)).Warn(Never.Serialization.SerializeEnvironment.JsonSerializer.Serialize(job));
        }
    }
}