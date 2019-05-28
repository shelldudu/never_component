using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Never.IoC.Providers;
using Never.Startups;
using IStructureMapContainer = StructureMap.Container;

namespace Never.IoC.StructureMap
{
    /// <summary>
    /// 引擎接口实现对象
    /// </summary>
    public sealed class StructureMapContainer : IContainer, IContainerStartup
    {
        #region field

        /// <summary>
        ///
        /// </summary>
        private readonly IStructureMapContainer builder = null;

        /// <summary>
        /// 注册者
        /// </summary>
        private readonly StructureMapServiceRegister register = null;

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
        static StructureMapContainer()
        {
        }

        /// <summary>
        ///
        /// </summary>
        public StructureMapContainer(IFilteringAssemblyProvider filteringAssemblyProvider)
        {
            this.builder = new IStructureMapContainer();
            this.register = new StructureMapServiceRegister(this.builder);
            this.filteringAssemblyProvider = filteringAssemblyProvider;
            this.scopeTracker = new StructureMapLifetimeScopeTracker();
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
            this.register.AddComponentInstance(this, typeof(IContainer), "StructureMap.Container");
            //注入容器
            this.register.AddComponentInstance(this.builder, typeof(IStructureMapContainer), "StructureMap.ContainerBuilder");

            //注入类型查找
            this.register.AddComponentInstance(this.typeFinder = new DefaultTypeFinder(), typeof(ITypeFinder), "StructureMap.TypeFinder");

            //注入注册
            this.register.AddComponentInstance(this.serviceRegister = this.register, typeof(IServiceRegister), "StructureMap.ServiceRegister");

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

            //注入生命周期管理
            this.builder.BuildUp(this);
            var scope = this.builder;
            var locator = new StructureMapServiceLocator(scope);
            this.serviceLocator = locator;
            this.serviceActivator = locator;

            ((StructureMapServiceRegister)this.serviceRegister).LifetimeScope = scope;

            //注入解决
            this.register.AddComponentInstance(this.serviceLocator, typeof(IServiceLocator), "StructureMap.ServiceLocator");
            this.register.AddComponentInstance(this.serviceActivator, typeof(IServiceActivator), "StructureMap.ServiceActivator");

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