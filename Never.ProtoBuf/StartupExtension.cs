using Never.IoC;
using Never.Serialization;
using Never.Startups;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Never.ProtoBuf
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动ProtoBuf支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseProtoBufSerializer(this ApplicationStartup startup)
        {
            return UseProtoBufSerializer(startup, string.Empty);
        }
        /// <summary>
        /// 启动ProtoBuf支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseProtoBufSerializer(this ApplicationStartup startup, string key)
        {
            return UseProtoBufSerializer(startup, key, ComponentLifeStyle.Singleton);
        }
        /// <summary>
        /// 启动ProtoBuf支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseProtoBufSerializer(this ApplicationStartup startup, string key, ComponentLifeStyle lifeStyle)
        {
            if (startup.ServiceRegister == null)
                return startup;

            startup.ServiceRegister.RegisterType(typeof(ProtoBufSerializer), typeof(IBinarySerializer), key, lifeStyle);
            return startup;
        }
    }
}