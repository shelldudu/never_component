using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Never.IoC.Injections;
using Autofac;
using Autofac.Core;
using IAutofacContainer = Autofac.IContainer;
using IAutofacLifetimeScope = Autofac.ILifetimeScope;

namespace Never.IoC.Autofac
{
    internal class AutofacLifetimeScope : ILifetimeScope
    {
        internal readonly IAutofacLifetimeScope scope = null;

        public AutofacLifetimeScope(IAutofacLifetimeScope scope)
        {
            this.scope = scope;
        }

        public event EventHandler OnDisposed;

        public ILifetimeScope BeginLifetimeScope()
        {
            return new AutofacLifetimeScope(this.scope.BeginLifetimeScope());
        }

        public void Dispose()
        {
            this.scope.Dispose();
            if (this.OnDisposed != null)
                this.OnDisposed(this, EventArgs.Empty);
        }

        public object Resolve(Type serviceType, string key)
        {
            return key.IsNullOrEmpty() ? this.scope.Resolve(serviceType) : this.scope.ResolveKeyed(key, serviceType);
        }

        public object[] ResolveAll(Type serviceType)
        {
            return this.scope.Resolve(typeof(IEnumerable<>).MakeGenericType(serviceType)) as object[];
        }

        public object ResolveOptional(Type serviceType)
        {
            return this.scope.ResolveOptional(serviceType);
        }
    }
}