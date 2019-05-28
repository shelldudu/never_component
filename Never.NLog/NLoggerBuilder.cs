using Never.Attributes;
using Never.Logging;
using Never.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Never.NLog
{
    /// <summary>
    ///
    /// </summary>
    public class NLoggerBuilder : Never.Logging.LoggerBuilder
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
        static NLoggerBuilder()
        {
            dict = new Dictionary<string, ILogger>(400);
        }

        /// <summary>
        ///
        /// </summary>
        public NLoggerBuilder()
        {
        }

        #endregion

        #region LoggerBuilder成员

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

            var logger = new NLogger(loggerName);
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