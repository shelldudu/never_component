using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Never.IoC;
using Never.Serialization;
using Never.Startups;

namespace Never.JsonNet
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动Newtonsoft.Json支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="key">IoC容器中的key</param>
        /// <param name="lifeStyle">生命周期</param>
        /// <returns></returns>
        public static ApplicationStartup UseJsonNet(this ApplicationStartup startup, string key = "ioc.ser.jsonnet", ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseJsonNet" + key))
                return startup;

            startup.ServiceRegister.RegisterType(typeof(JsonNetSerializer), typeof(IJsonSerializer), key, lifeStyle);
            startup.Items["UseJsonNet" + key] = "t";

            return startup;
        }

        /// <summary>
        /// 启动Newtonsoft.Json支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="key">IoC容器中的key</param>
        /// <returns></returns>
        public static ApplicationStartup UseJsonNetWithRegisterToSerializeEnvironment(this ApplicationStartup startup, string key = "ioc.ser.jsonnet")
        {
            if (startup.ServiceRegister == null)
                return startup;

            if (startup.Items.ContainsKey("UseJsonNetWithRegisterToSerializeEnvironment" + key))
                return startup;

            startup.ServiceRegister.RegisterInstance(new JsonNetSerializer(), typeof(IJsonSerializer), key);
            startup.Items["UseJsonNetWithRegisterToSerializeEnvironment" + key] = "t";

            return startup;
        }
    }
}