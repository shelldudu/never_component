using Autofac;
using Never.IoC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IAutofacContainer = Autofac.IContainer;
using IAutofacLifetimeScope = Autofac.ILifetimeScope;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 注册组件接口实现对象
    /// </summary>
    public sealed class AutofacServiceLocator : IServiceLocator, IServiceActivator
    {
        #region field

        /// <summary>
        /// 容器
        /// </summary>
        private readonly IAutofacLifetimeScope rootScope = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static AutofacServiceLocator()
        {
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="rootScope"></param>
        public AutofacServiceLocator(IAutofacLifetimeScope rootScope)
        {
            this.rootScope = rootScope;
        }

        #endregion ctor

        #region IServiceLocator成员

        /// <summary>
        /// 跟踪者
        /// </summary>
        public ILifetimeScopeTracker ScopeTracker { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public ILifetimeScope BeginLifetimeScope()
        {
            return new AutofacLifetimeScope(this.rootScope.BeginLifetimeScope());
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        private ILifetimeScope BeginLifetimeScope(IAutofacLifetimeScope scope)
        {
            if (this.ScopeTracker == null)
                throw new ArgumentNullException("scopeTracker is null");

            return this.ScopeTracker.StartScope(new AutofacLifetimeScope(scope));
        }

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <returns></returns>
        public TService[] ResolveAll<TService>()
        {
            return this.BeginLifetimeScope(this.rootScope).ResolveAll(typeof(TService)) as TService[];
        }

        /// <summary>
        /// 返回所有T对象的实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public object[] ResolveAll(Type serviceType)
        {
            return this.BeginLifetimeScope(this.rootScope).ResolveAll(serviceType);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public TService Resolve<TService>()
        {
            return (TService)this.BeginLifetimeScope(this.rootScope).Resolve(typeof(TService), string.Empty);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public TService Resolve<TService>(string key)
        {
            return (TService)this.BeginLifetimeScope(this.rootScope).Resolve(typeof(TService), key);
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public TService ResolveOptional<TService>()
        {
            TService service = default(TService);
            this.ResolveOptional<TService>(string.Empty, out service);
            return service;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public object ResolveOptional(Type serviceType)
        {
            object service = default(object);
            this.ResolveOptional(serviceType, string.Empty, out service);
            return service;
        }

        /// <summary>
        /// 返回T对象的实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <param name="service"></param>
        /// <returns></returns>
        private bool ResolveOptional<TService>(string key, out TService service)
        {
            var scope = this.BeginLifetimeScope(this.rootScope);
            try
            {
                service = (TService)scope.Resolve(typeof(TService), key);
                return true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                service = (TService)scope.ResolveOptional(typeof(TService));
                return true;
            }
            catch (Exception ex)
            {
            }

            service = default(TService);
            return false;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType"></param>
        /// <param name="service"></param>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        private bool ResolveOptional(Type serviceType, string key, out object service)
        {
            var scope = this.BeginLifetimeScope(this.rootScope);
            try
            {
                service = scope.Resolve(serviceType, key);
                return true;
            }
            catch (Exception ex)
            {
            }

            try
            {
                service = scope.ResolveOptional(serviceType);
                return true;
            }
            catch (Exception ex)
            {
            }

            service = default(object);
            return false;
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public object Resolve(Type serviceType)
        {
            return this.BeginLifetimeScope(this.rootScope).Resolve(serviceType, string.Empty);
        }

        /// <summary>
        /// 返回某对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public object Resolve(Type serviceType, string key)
        {
            return this.BeginLifetimeScope(this.rootScope).Resolve(serviceType, key);
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public bool TryResolve<TService>(ref TService instance)
        {
            return this.TryResolve<TService>(ref instance, string.Empty);
        }

        /// <summary>
        /// 尝试返回已注册的T对象实体
        /// </summary>
        /// <typeparam name="TService">服务类型</typeparam>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public bool TryResolve<TService>(ref TService instance, string key)
        {
            try
            {
                return this.ResolveOptional<TService>(key, out instance);
            }
            catch
            {
            }
            return false;
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务对象</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public bool TryResolve(Type serviceType, ref object instance)
        {
            return this.TryResolve(serviceType, ref instance, string.Empty);
        }

        /// <summary>
        /// 尝试返回已注册的serviceType对象实体
        /// </summary>
        /// <param name="serviceType">服务类型</param>
        /// <param name="instance">服务对象</param>
        /// <param name="key">key</param>
        /// <param name="scopeTracker">跟踪者</param>
        /// <returns></returns>
        public bool TryResolve(Type serviceType, ref object instance, string key)
        {
            try
            {
                return this.ResolveOptional(serviceType, key, out instance);
            }
            catch
            {
            }

            return false;
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <typeparam name="TService"></typeparam>
        /// <returns></returns>
        public ActivatorService<TService> CreateService<TService>()
        {
            var service = this.ResolveOptional<TService>();
            return new ActivatorService<TService>(service, new Disposer());
        }

        /// <summary>
        /// 创建对象
        /// </summary>
        /// <param name="serviceType"></param>
        /// <returns></returns>
        public ActivatorService<object> CreateService(Type serviceType)
        {
            var service = this.ResolveOptional(serviceType);
            return new ActivatorService<object>(service, new Disposer());
        }

        #endregion IServiceLocator成员
    }
}