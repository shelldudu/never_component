using Never.Attributes;
using Never.Logging;
using Never.Threading;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Never.Log4net
{
    /// <summary>
    /// 日志输出仓库
    /// </summary>
    public class Log4netLoggerBuilder : Never.Logging.LoggerBuilder
    {
        #region field

        /// <summary>
        /// 日志输出仓库
        /// </summary>
        protected readonly static IDictionary<string, ILogger> dict = null;

        #endregion field

        #region

        /// <summary>
        ///
        /// </summary>
        static Log4netLoggerBuilder()
        {
            dict = new Dictionary<string, ILogger>(400);
        }

        /// <summary>
        ///
        /// </summary>
        public Log4netLoggerBuilder()
        {
        }

        #endregion

        #region ILoggerBuilder成员

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="loggerName"></param>
        /// <returns></returns>
        [NotNull(Name = "loggerName")]
        public override ILogger Build(string loggerName)
        {
            if (dict.ContainsKey(loggerName))
                return dict[loggerName];

            var logger = new Log4netLogger(loggerName);
            dict[loggerName] = logger;
            return logger;
        }

        /// <summary>
        /// 获取实例
        /// </summary>
        /// <param name="loggerType"></param>
        /// <returns></returns>
        [NotNull(Name = "loggerType")]
        public override ILogger Build(Type loggerType)
        {
            return this.Build(loggerType.FullName);
        }

        #endregion
    }
}