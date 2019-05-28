using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using IStructureMapContainer = StructureMap.IContainer;

namespace Never.IoC.StructureMap
{
    /// <summary>
    /// 跟踪者
    /// </summary>
    public class StructureMapLifetimeScopeTracker : ILifetimeScopeTracker
    {
        #region field

        /// <summary>
        /// 读写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim rwslock = null;

        /// <summary>
        /// TLS
        /// </summary>
        private readonly System.Threading.ThreadLocal<IStructureMapContainer> threadLocal = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static StructureMapLifetimeScopeTracker()
        {
            rwslock = new ReaderWriterLockSlim();
        }

        /// <summary>
        ///
        /// </summary>
        public StructureMapLifetimeScopeTracker()
        {
            threadLocal = new ThreadLocal<IStructureMapContainer>();
        }

        #endregion ctor

        #region lifescope

#if !NET461
#else

        /// <summary>
        ///
        /// </summary>
        private IStructureMapContainer LifetimeScope
        {
            get
            {
                IStructureMapContainer life = null;
                if (System.Web.HttpContext.Current == null)
                    return life;

                rwslock.EnterReadLock();
                life = System.Web.HttpContext.Current.Items[typeof(IStructureMapContainer)] as IStructureMapContainer;
                rwslock.ExitReadLock();

                return life;
            }

            set
            {
                rwslock.EnterWriteLock();
                try
                {
                    System.Web.HttpContext.Current.Items[typeof(IStructureMapContainer)] = value;
                }
                catch
                {
                    throw;
                }
                finally
                {
                    rwslock.ExitWriteLock();
                }
            }
        }

#endif

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        private IStructureMapContainer InitializeLifetimeScope(IStructureMapContainer builder)
        {
            if (threadLocal.Value != null)
                return threadLocal.Value;

            return threadLocal.Value = builder.CreateChildContainer();
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public IStructureMapContainer GetLifetimeScope(IStructureMapContainer builder)
        {
#if !NET461
            return InitializeLifetimeScope(builder);
#else
            if (System.Web.HttpContext.Current == null)
                return InitializeLifetimeScope(builder);

            return LifetimeScope ?? (LifetimeScope = InitializeLifetimeScope(builder));
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        public void CleanScope()
        {
#if !NET461
            threadLocal.Value?.Dispose();
#else
            LifetimeScope?.Dispose();
#endif
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public ILifetimeScope StartScope(ILifetimeScope parent)
        {
            var scope = parent as StructureMapLifetimeScope;
            return new StructureMapLifetimeScope(GetLifetimeScope(scope.scope));
        }

        #endregion lifescope
    }
}