using StructureMap;
using StructureMap.Configuration.DSL;
using StructureMap.Pipeline;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using IStructureMapContainer = StructureMap.IContainer;

namespace Never.IoC.StructureMap
{
    internal class StructureMapLifetimeScope : ILifetimeScope
    {
        internal readonly IStructureMapContainer scope = null;

        public StructureMapLifetimeScope(IStructureMapContainer scope)
        {
            this.scope = scope;
        }

        public event EventHandler OnDisposed;

        public ILifetimeScope BeginLifetimeScope()
        {
            return new StructureMapLifetimeScope(this.scope.CreateChildContainer());
        }

        public void Dispose()
        {
            this.scope.Dispose();
            if (this.OnDisposed != null)
                this.OnDisposed(this, EventArgs.Empty);
        }

        public object Resolve(Type serviceType, string key)
        {
            return key.IsNullOrEmpty() ? this.scope.GetInstance(serviceType) : this.scope.GetInstance(serviceType, key);
        }

        public object[] ResolveAll(Type serviceType)
        {
            return this.scope.GetInstance(typeof(IEnumerable<>).MakeGenericType(serviceType)) as object[];
        }

        public object ResolveOptional(Type serviceType)
        {
            return this.scope.TryGetInstance(serviceType);
        }
    }
}