using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StructureMap;
using IStructureMapContainer = StructureMap.IContainer;

namespace Never.IoC.StructureMap
{
    /// <summary>
    /// 注册组件接口实现对象
    /// </summary>
    public sealed class StructureMapServiceRegister : IServiceRegister
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IStructureMapContainer builder = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static StructureMapServiceRegister()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        public StructureMapServiceRegister(IStructureMapContainer builder)
        {
            this.builder = builder;
        }

        #endregion ctor

        #region prop

        /// <summary>
        ///
        /// </summary>
        internal IStructureMapContainer LifetimeScope { get; set; }

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

            return this.LifetimeScope.TryGetInstance(serviceType) != null;
        }

        /// <summary>
        /// 是否注册了组件
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public bool Contain<TService>()
        {
            return Contain(typeof(TService));
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

        #endregion IServiceRegister成员

        #region component

        /// <summary>
        ///
        /// </summary>
        /// <param name="implementation"></param>
        /// <param name="service"></param>
        /// <param name="key"></param>
        /// <param name="lifeStyle"></param>
        public void AddComponent(Type implementation, Type service, string key = null, ComponentLifeStyle lifeStyle = ComponentLifeStyle.Singleton)
        {
            if (this.LifetimeScope != null)
                throw new Exception("containerbuilder is builded");

            this.builder.Configure(x =>
            {
                var @object = x.For(service).PerLifeStyle(lifeStyle).Use(implementation);
                if (!string.IsNullOrEmpty(key))
                    @object.Named(key);
            });
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

            this.builder.Configure(x =>
            {
                var @object = x.For(service).PerLifeStyle(ComponentLifeStyle.Singleton).Use(instance);
                if (!string.IsNullOrEmpty(key))
                    @object.Named(key);
            });
        }

        #endregion component
    }
}