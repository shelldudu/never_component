using Never.IoC;
using StructureMap;
using StructureMap.Configuration.DSL.Expressions;
using StructureMap.Pipeline;
using StructureMap.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Never.IoC.StructureMap
{
    /// <summary>
    /// 依赖注入容器扩展
    /// </summary>
    public static class DependencyContainerExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="registration"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static GenericFamilyExpression PerLifeStyle(this GenericFamilyExpression registration, ComponentLifeStyle lifeStyle)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");

            switch (lifeStyle)
            {
                case ComponentLifeStyle.Scoped:
                    {
#if !NET461
                        return registration.LifecycleIs(new ThreadLocalStorageLifecycle());

#else
                        return System.Web.Hosting.HostingEnvironment.IsHosted ? registration.LifecycleIs(new UniquePerRequestLifecycle()) : registration.LifecycleIs(new ThreadLocalStorageLifecycle());
#endif
                    }
                case ComponentLifeStyle.Singleton:
                    {
                        return registration.LifecycleIs(Lifecycles.Singleton);
                    }
                default:
                case ComponentLifeStyle.Transient:
                    {
                        return registration.LifecycleIs(Lifecycles.Transient);
                    }
            }
        }
    }
}