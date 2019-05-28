using Never.IoC;
using Never.Logging;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Never.Log4net
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动log4net支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="file">log4net配置文件</param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseLog4net(this ApplicationStartup startup, FileInfo file, string key = null, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (startup.ServiceRegister == null)
                return startup;

            //log4net日志配置
            if (file == null)
                throw new ArgumentNullException("the log4net file is null");

            var factory = log4net.LogManager.CreateRepository("UseLog4net");
            log4net.Config.XmlConfigurator.ConfigureAndWatch(factory, file);

            startup.ServiceRegister.RegisterType(typeof(Log4netLoggerBuilder), typeof(ILoggerBuilder), key, lifeStyle);
            return startup;
        }
    }
}