using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using IAutofacContainer = Autofac.IContainer;
using IAutofacLifetimeScope = Autofac.ILifetimeScope;

namespace Never.IoC.Autofac
{
    /// <summary>
    /// 跟踪者
    /// </summary>
    public class AutofacLifetimeScopeTracker : ILifetimeScopeTracker
    {
        #region field

        /// <summary>
        /// 这个tab用来标识当前http请求中autofac的IContainer实例
        /// </summary>
        public static readonly object HttpRequestTag = "AutofacWebRequest";

        /// <summary>
        /// 读写锁
        /// </summary>
        private static readonly ReaderWriterLockSlim rwslock = null;

        /// <summary>
        /// TLS
        /// </summary>
        private readonly System.Threading.ThreadLocal<IAutofacLifetimeScope> threadLocal = null;

        #endregion field

        #region ctor

        /// <summary>
        ///
        /// </summary>
        static AutofacLifetimeScopeTracker()
        {
            rwslock = new ReaderWriterLockSlim();
        }

        /// <summary>
        ///
        /// </summary>
        public AutofacLifetimeScopeTracker()
        {
            threadLocal = new ThreadLocal<IAutofacLifetimeScope>();
        }

        #endregion ctor

        #region lifescope

#if !NET461
#else

        /// <summary>
        ///
        /// </summary>
        private IAutofacLifetimeScope LifetimeScope
        {
            get
            {
                IAutofacLifetimeScope life = null;
                if (System.Web.HttpContext.Current == null)
                    return life;

                rwslock.EnterReadLock();
                life = System.Web.HttpContext.Current.Items[typeof(IAutofacLifetimeScope)] as IAutofacLifetimeScope;
                rwslock.ExitReadLock();

                return life;
            }

            set
            {
                rwslock.EnterWriteLock();
                try
                {
                    System.Web.HttpContext.Current.Items[typeof(IAutofacLifetimeScope)] = value;
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
        /// <param name="container"></param>
        /// <returns></returns>
        private IAutofacLifetimeScope InitializeLifetimeScope(IAutofacLifetimeScope container)
        {
            if (threadLocal.Value != null)
                return threadLocal.Value;

            return threadLocal.Value = container.BeginLifetimeScope(HttpRequestTag);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public IAutofacLifetimeScope GetLifetimeScope(IAutofacLifetimeScope container)
        {
#if !NET461
            return InitializeLifetimeScope(container);
#else
            if (System.Web.HttpContext.Current == null)
                return InitializeLifetimeScope(container);

            return LifetimeScope ?? (LifetimeScope = InitializeLifetimeScope(container));
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
            var scope = parent as AutofacLifetimeScope;
            return new AutofacLifetimeScope(GetLifetimeScope(scope.scope));
        }

        #endregion lifescope
    }
}