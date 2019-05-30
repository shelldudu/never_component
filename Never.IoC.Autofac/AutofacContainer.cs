using Autofac;
using Never.IoC.Providers;
using Never.Startups;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 引擎接口实现对象
    /// </summary>
    public sealed class AutofacContainer : IContainer, IContainerStartup
    {
        #region field

        /// <summary>
        /// 构建者
        /// </summary>
        private readonly ContainerBuilder builder = null;

        /// <summary>
        /// 注册者
        /// </summary>
        private readonly AutofacServiceRegister register = null;

        /// <summary>
        ///
        /// </summary>
        private IServiceRegister serviceRegister = null;

        /// <summary>
        ///
        /// </summary>
        private IServiceLocator serviceLocator = null;

        /// <summary>
        ///
        /// </summary>
        private ITypeFinder typeFinder = null;

        /// <summary>
        /// 跟踪器
        /// </summary>
        private ILifetimeScopeTracker scopeTracker = null;

        /// <summary>
        /// 创建者
        /// </summary>
        private IServiceActivator serviceActivator = null;

        /// <summary>
        /// 程序集过滤
        /// </summary>
        private readonly IFilteringAssemblyProvider filteringAssemblyProvider = null;

        /// <summary>
        /// 过滤的程序集
        /// </summary>
        private Assembly[] assemblies;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static AutofacContainer()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public AutofacContainer(IFilteringAssemblyProvider filteringAssemblyProvider)
        {
            this.builder = new ContainerBuilder();
            this.register = new AutofacServiceRegister(this.builder);
            this.filteringAssemblyProvider = filteringAssemblyProvider;
            this.scopeTracker = new AutofacLifetimeScopeTracker();
        }

        #endregion ctor

        #region property

        /// <summary>
        /// 是否已经初始化环境了
        /// </summary>
        public bool IsInitEnvironment { get; private set; }

        /// <summary>
        /// 是否已经启动了
        /// </summary>
        public bool IsStarted { get; private set; }

        #endregion property

        #region IContainer成员

        /// <summary>
        /// 在系统启动过路中初始化组件
        /// </summary>
        public event EventHandler<IContainerStartupEventArgs> OnIniting;

        /// <summary>
        /// 在系统启动最后的组件
        /// </summary>
        public event EventHandler<IContainerStartupEventArgs> OnStarting;

        /// <summary>
        ///
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Init()
        {
            if (this.IsInitEnvironment)
                return;

            this.IsInitEnvironment = true;

            //注入引擎
            this.register.AddComponentInstance(this, typeof(IContainer), "Autofac.Container");

            //注入容器
            this.register.AddComponentInstance(this.builder, typeof(ContainerBuilder), "Autofac.ContainerBuilder");

            //注入类型查找
            this.register.AddComponentInstance(this.typeFinder = new DefaultTypeFinder(), typeof(ITypeFinder), "Autofac.TypeFinder");

            //注入注册
            this.register.AddComponentInstance(this.serviceRegister = this.register, typeof(IServiceRegister), "Autofac.ServiceRegister");

            //获取程序集
            this.assemblies = this.filteringAssemblyProvider.GetAssemblies();

            this.OnIniting?.Invoke(this, new IContainerStartupEventArgs(this.typeFinder, this.assemblies, this.builder));

            return;
        }

        /// <summary>
        /// 开始启动
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void Startup()
        {
            if (this.IsStarted)
                return;

            if (!this.IsInitEnvironment)
                throw new Exception("call init before on start");

            this.OnStarting?.Invoke(this, new IContainerStartupEventArgs(this.typeFinder, this.assemblies, this.builder));

            //注入解决
            this.register.AddComponentCallback<IServiceLocator>(() => this.serviceLocator, "Autofac.ServiceLocator");

            //注入解决
            this.register.AddComponentCallback<IServiceActivator>(() => this.serviceActivator, "Autofac.ServiceActivator");

            //注入生命周期管理
            var scope = this.builder.Build();
            var locator = new AutofacServiceLocator(scope) { ScopeTracker = this.scopeTracker };
            this.serviceLocator = locator;
            this.serviceActivator = locator;

            ((AutofacServiceRegister)this.serviceRegister).LifetimeScope = scope;

            this.IsStarted = true;
        }

        /// <summary>
        ///
        /// </summary>
        public IServiceRegister ServiceRegister
        {
            get
            {
                return this.serviceRegister;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public IServiceLocator ServiceLocator
        {
            get
            {
                return this.serviceLocator;
            }
        }

        /// <summary>
        ///
        /// </summary>
        public ITypeFinder TypeFinder
        {
            get
            {
                return this.typeFinder;
            }
        }

        /// <summary>
        /// 服务创建器
        /// </summary>
        public IServiceActivator ServiceActivator
        {
            get
            {
                return this.serviceActivator;
            }
        }

        /// <summary>
        /// 跟踪者
        /// </summary>
        public ILifetimeScopeTracker ScopeTracker
        {
            get => this.scopeTracker;
            set => this.scopeTracker = value;
        }

        #endregion IContainer成员
    }
}