using System;
using System.Collections.Generic;
using System.Reflection;
using StructureMap;
using IStructureMapContainer = StructureMap.Container;

namespace Never.IoC.StructureMap
{
    /// <summary>
    /// 启动扩展
    /// </summary>
    public static class StartupExtension
    {
        /// <summary>
        /// 启动StructureMap支持
        /// </summary>
        /// <param name="startup"></param>
        /// <returns></returns>
        public static ApplicationStartup UseStructureMap(this ApplicationStartup startup)
        {
            return UseStructureMap(startup, null, null);
        }

        /// <summary>
        /// 启动StructureMap支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        /// <returns></returns>
        public static ApplicationStartup UseStructureMap(this ApplicationStartup startup, Action<ConfigurationExpression, ITypeFinder, IEnumerable<Assembly>> onIniting)
        {
            return UseStructureMap(startup, onIniting, null);
        }

        /// <summary>
        /// 启动StructureMap支持
        /// </summary>
        /// <param name="startup"></param>
        /// <param name="onIniting">在icontainer初始化环境的时候执行的</param>
        /// <param name="onStarting">在start方法后最后一个startservice执行的</param>
        /// <returns></returns>
        public static ApplicationStartup UseStructureMap(this ApplicationStartup startup, Action<ConfigurationExpression, ITypeFinder, IEnumerable<Assembly>> onIniting, Action<ConfigurationExpression, ITypeFinder, IEnumerable<Assembly>> onStarting)
        {
            if (startup.Items.ContainsKey("UseStructureMap"))
                return startup;

            var ioc = new StructureMapContainer(startup.FilteringAssemblyProvider);
            if (onIniting != null)
                ioc.OnIniting += (s, e) => { var builder = e.Collector as IStructureMapContainer; builder.Configure(x => { onIniting.Invoke(x, e.TypeFinder, e.Assemblies); }); };

            ioc.Init();

            if (onStarting != null)
                ioc.OnStarting += (s, e) => { var builder = e.Collector as IStructureMapContainer; builder.Configure(x => { onStarting.Invoke(x, e.TypeFinder, e.Assemblies); }); };

            startup.InsertIntoFinalStartService((x) => ioc.Startup());
            startup.ReplaceServiceLocator(ioc.ServiceLocator);
            startup.ReplaceServiceRegister(ioc.ServiceRegister);
            ContainerContext.Current = ioc;
            startup.Items["UseStructureMap"] = "t";
            return startup;
        }
    }
}