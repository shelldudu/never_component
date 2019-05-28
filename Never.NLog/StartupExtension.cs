using Never.Exceptions;
using Never.IoC;
using Never.Logging;
using Never.Startups;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Logger = NLog.Logger;
using LogManager = NLog.LogManager;

namespace Never.NLog
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动NLog支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="configFile">配置文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseNLog(this ApplicationStartup startup, FileInfo configFile, string key = null, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (configFile != null && configFile.Exists)
            {
                if (!configFile.Name.IsEquals("nlog.config", StringComparison.OrdinalIgnoreCase))
                    throw new DomainException("文件名必须为nlog.config，windows下不区分大小写");

                LogFactory.CurrentAppDomain = new NLogAppDomain(configFile);
                LogManager.LoadConfiguration(configFile.FullName);
            }

            startup.ServiceRegister.RegisterType(typeof(NLoggerBuilder), typeof(ILoggerBuilder), key, lifeStyle);
            return startup;
        }
    }
}