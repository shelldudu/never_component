using Never.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logger = NLog.Logger;
using LogManager = NLog.LogManager;

namespace Never.NLog
{
    /// <summary>
    ///
    /// </summary>
    public class NLogger : Never.Logging.ILogger
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly Logger logger = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        /// <param name="loggerName"></param>
        [NotNull(Name = "loggerName")]
        public NLogger(string loggerName)
        {
            logger = LogManager.GetLogger(loggerName);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="loggerType"></param>
        [NotNull(Name = "loggerType")]
        public NLogger(Type loggerType)
        {
            logger = LogManager.GetCurrentClassLogger(loggerType);
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
            logger.Debug(exception, message);
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
            logger.Error(exception, message);
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
            logger.Fatal(exception, message);
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
            logger.Info(exception, message);
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
            logger.Warn(exception, message);
        }

        #endregion ILogger成员
    }
}