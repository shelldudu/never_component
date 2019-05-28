using Never.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Never.Log4net
{
    /// <summary>
    /// 基于log4net实现的log日志
    /// </summary>
    public sealed class Log4netLogger : Never.Logging.ILogger
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly log4net.ILog logger = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="loggerName"></param>
        [NotNull(Name = "loggerName")]
        public Log4netLogger(string loggerName)
        {
            logger = log4net.LogManager.GetLogger("UseLog4net", loggerName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="loggerType"></param>
        [NotNull(Name = "loggerType")]
        public Log4netLogger(Type loggerType)
        {
            logger = log4net.LogManager.GetLogger("UseLog4net", loggerType);
        }

        #endregion ctor

        #region ILogger成员

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Debug(string message)
        {
            logger.Debug(message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Debug(string message, Exception exception)
        {
            logger.Debug(message, exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Error(string message)
        {
            logger.Error(message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Error(string message, Exception exception)
        {
            logger.Error(message, exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Fatal(string message, Exception exception)
        {
            logger.Fatal(message, exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Info(string message)
        {
            logger.Info(message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Info(string message, Exception exception)
        {
            logger.Info(message, exception);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="message"></param>
        /// <param name="exception"></param>
        public void Warn(string message, Exception exception)
        {
            logger.Warn(message, exception);
        }

        #endregion ILogger成员
    }
}