using System;
using System.Collections.Generic;
using System.Reflection;
using Autofac;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动autofac支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseAutofac(this ApplicationStartup startup)
        {
            return UseAutofac(startup, null, null);
        }

        /// <summary>
        /// 启动autofac支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        /// <returns></returns>
        public static ApplicationStartup UseAutofac(this ApplicationStartup startup, Action<ContainerBuilder, ITypeFinder, IEnumerable<Assembly>> onIniting)
        {
            return UseAutofac(startup, onIniting, null);
        }

        /// <summary>
        /// 启动autofac支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        /// <param name="onStarting">在start方法后最后一个startservice执行的</param>
        /// <returns></returns>
        public static ApplicationStartup UseAutofac(this ApplicationStartup startup, Action<ContainerBuilder, ITypeFinder, IEnumerable<Assembly>> onIniting, Action<ContainerBuilder, ITypeFinder, IEnumerable<Assembly>> onStarting)
        {
            if (startup.Items.ContainsKey("UseAutofac"))
                return startup;

            var ioc = new AutofacContainer(startup.FilteringAssemblyProvider);
            if (onIniting != null)
                ioc.OnIniting += (s, e) => { onIniting.Invoke((ContainerBuilder)e.Collector, e.TypeFinder, e.Assemblies); };

            ioc.Init();

            if (onStarting != null)
                ioc.OnStarting += (s, e) => { onStarting.Invoke((ContainerBuilder)e.Collector, e.TypeFinder, e.Assemblies); };

            startup.InsertIntoFinalStartService((x) => ioc.Startup());
            startup.ReplaceServiceLocator(ioc.ServiceLocator);
            startup.ReplaceServiceRegister(ioc.ServiceRegister);
            ContainerContext.Current = ioc;
            startup.Items["UseAutofac"] = "t";
            return startup;
        }
    }
}