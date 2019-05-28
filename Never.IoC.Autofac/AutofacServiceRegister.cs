using Autofac;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAutofacContainer = Autofac.IContainer;
using IAutoLifetimeScope = Autofac.ILifetimeScope;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 注册组件接口实现对象
    /// </summary>
    public sealed class AutofacServiceRegister : IServiceRegister
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly ContainerBuilder builder = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static AutofacServiceRegister()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        public AutofacServiceRegister(ContainerBuilder builder)
        {
            this.builder = builder;
        }

        #endregion ctor

        #region prop

        /// <summary>
        ///
        /// </summary>
        internal IAutoLifetimeScope LifetimeScope { get; set; }

        #endregion prop

        #region IServiceRegister成员

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <returns></returns>
        public bool Contain(Type serviceType)
        {
            if (serviceType == null)
                return false;

            if (this.LifetimeScope == null)
                return false;

            return this.LifetimeScope.IsRegistered(serviceType);
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public bool Contain<TService>()
        {
            return this.Contain(typeof(TService));
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        public void RegisterInstance(object instance, Type serviceType)
        {
            this.AddComponentInstance(instance, serviceType, string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <param name="instance">服务对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        public void RegisterInstance(object instance, Type serviceType, string key)
        {
            this.AddComponentInstance(instance, serviceType, key);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        public void RegisterInstance<TService>(TService instance)
        {
            this.AddComponentInstance(instance, typeof(TService), string.Empty);
        }

        /// <summary>
        /// 注册对象实例映射关系
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        public void RegisterInstance<TService>(TService instance, string key)
        {
            this.AddComponentInstance(instance, typeof(TService), string.Empty);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        public void RegisterObject(Type serviceType)
        {
            this.AddComponent(serviceType, serviceType, string.Empty);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        public void RegisterObject<TService>()
        {
            this.AddComponent(typeof(TService), typeof(TService), string.Empty);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        public void RegisterType(Type implementationType, Type serviceType)
        {
            this.AddComponent(implementationType, serviceType, string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <param name="implementationType">继承TSservice对象的具体对象</param>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public void RegisterType(Type implementationType, Type serviceType, string key, ComponentLifeStyle lifeStyle)
        {
            this.AddComponent(implementationType, serviceType, key, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        public void RegisterType<TImplementation, TService>()
        {
            this.AddComponent(typeof(TImplementation), typeof(TService), string.Empty, ComponentLifeStyle.Transient);
        }

        /// <summary>
        /// 注册中间件与接口映射关系
        /// </summary>
        /// <typeparam name="TImplementation">继承TSservice对象的具体对象</typeparam>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="lifeStyle">生命周期</param>
        public void RegisterType<TImplementation, TService>(string key, ComponentLifeStyle lifeStyle)
        {
            this.AddComponent(typeof(TImplementation), typeof(TService), key, ComponentLifeStyle.Transient);
        }

        #endregion IServiceRegister成员

        #region component

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        public void AddComponent(Type implementation, Type service, string key, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            var serviceTypes = new List<Type> { service };
            if (service.IsGenericType && implementation.IsGenericType)
            {
                var temp = this.builder.RegisterGeneric(implementation)
                    .As(serviceTypes.ToArray())
                    .PropertiesAutowired()
                    .PerLifeStyle(lifeStyle);

                if (!string.IsNullOrEmpty(key))
                    temp.Keyed(key, service);
            }
            else
            {
                var temp = this.builder.RegisterType(implementation)
                    .As(serviceTypes.ToArray())
                    .PropertiesAutowired()
                    .PerLifeStyle(lifeStyle);

                if (!string.IsNullOrEmpty(key))
                    temp.Keyed(key, service);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="services"></param>
        /// <param name="lifeStyle"></param>
        public void AddComponent(Type implementation, Type[] services, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            if (implementation.IsGenericType)
            {
                var temp = this.builder.RegisterGeneric(implementation)
                    .As(services.ToArray())
                    .PropertiesAutowired()
                    .PerLifeStyle(lifeStyle);
            }
            else
            {
                var temp = this.builder.RegisterType(implementation)
                    .As(services.ToArray())
                    .PropertiesAutowired()
                    .PerLifeStyle(lifeStyle);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="service"></param>
        /// <param name="key"></param>
        public void AddComponentInstance(object instance, Type service, string key)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            var temp = this.builder.RegisterInstance(instance).As(service).PropertiesAutowired().PerLifeStyle(ComponentLifeStyle.Singleton);
            if (!string.IsNullOrEmpty(key))
                temp.Keyed(key, service);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="callback"></param>
        /// <param name="key"></param>
        public void AddComponentCallback<T>(Func<T> callback, string key)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            var temp = this.builder.Register<T>((a) => { return callback(); });
            if (!string.IsNullOrEmpty(key))
                temp.Keyed(key, typeof(T));
        }


        /// <summary>
        ///
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="service"></param>
        public void AddComponentInstance(object instance, Type[] service)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            this.builder.RegisterInstance(instance).As(service).PropertiesAutowired().PerLifeStyle(ComponentLifeStyle.Singleton);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="service"></param>
        /// <param name="properties"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        public void AddComponentWithParameters(Type implementation, Type service, IDictionary<string, object> properties, string key, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            var serviceTypes = new List<Type> { service };
            var temp = this.builder.RegisterType(implementation)
                .As(serviceTypes.ToArray())
                .PropertiesAutowired()
                .WithParameters(properties.Select(y => new NamedParameter(y.Key, y.Value)))
                .PerLifeStyle(lifeStyle);

            if (!string.IsNullOrEmpty(key))
                temp.Keyed(key, service);
        }


        #endregion component
    }
}