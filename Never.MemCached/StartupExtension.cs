using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Never.Caching;
using Never.Startups;

namespace Never.MemCached
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动memcached支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="servers">服务列表</param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseMemCached(this ApplicationStartup startup, string[] servers, string key = null)
        {
            if (startup.ServiceRegister == null)
                return startup;

            var mem = new Memcached(servers);
            startup.ServiceRegister.RegisterInstance(mem, typeof(ICaching), key);
            return startup;
        }
    }
}
