using Autofac;
using Autofac.Builder;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 依赖注入容器扩展
    /// </summary>
    public static class DependencyContainerExtension
    {
        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TStyle"></typeparam>
        /// <param name="registration"></param>
        /// <param name="lifetimeScopeTags"></param>
        /// <returns></returns>
        /// <remarks>此方法是模拟autofac.mvc里面实现的,其中也适用于autofac.webapi</remarks>
        public static IRegistrationBuilder<TLimit, TActivatorData, TStyle> InstancePerHttpRequest<TLimit, TActivatorData, TStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TStyle> registration, params object[] lifetimeScopeTags)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");

            object[] array = new object[]
            {
                AutofacLifetimeScopeTracker.HttpRequestTag
            }.Concat(lifetimeScopeTags).ToArray<object>();

            return registration.InstancePerMatchingLifetimeScope(array);
        }

        /// <summary>
        ///
        /// </summary>
        /// <typeparam name="TLimit"></typeparam>
        /// <typeparam name="TActivatorData"></typeparam>
        /// <typeparam name="TRegistrationStyle"></typeparam>
        /// <param name="registration"></param>
        /// <param name="lifeStyle"></param>
        /// <returns></returns>
        public static IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> PerLifeStyle<TLimit, TActivatorData, TRegistrationStyle>(this IRegistrationBuilder<TLimit, TActivatorData, TRegistrationStyle> registration, ComponentLifeStyle lifeStyle)
        {
            if (registration == null)
                throw new ArgumentNullException("registration");

            switch (lifeStyle)
            {
                case ComponentLifeStyle.Scoped:
                    {
#if !NET461
                        return registration.InstancePerLifetimeScope();
#else
                        return System.Web.Hosting.HostingEnvironment.IsHosted ? registration.InstancePerHttpRequest() : registration.InstancePerLifetimeScope();
#endif
                    }
                case ComponentLifeStyle.Singleton:
                    {
                        return registration.SingleInstance();
                    }
                default:
                case ComponentLifeStyle.Transient:
                    {
                        return registration.InstancePerDependency();
                    }
            }
        }
    }
}