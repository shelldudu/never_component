using NLog.Internal.Fakeables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Never.NLog
{
    /// <summary>
    ///  NLog配置环境
    /// </summary>
    public sealed class NLogAppDomain : IAppDomain
    {
        #region field

        /// <summary>
        /// The log configuration
        /// </summary>
        private readonly FileInfo configFile = null;

        /// <summary>
        /// The domain unload
        /// </summary>
        private EventHandler<EventArgs> domainUnload = null;

        /// <summary>
        /// The process exit
        /// </summary>
        private EventHandler<EventArgs> processExit = null;

        #endregion field

        #region ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="NLogAppDomain"/> class.
        /// </summary>
        /// <param name="configFile">The log configuration path.</param>
        public NLogAppDomain(FileInfo configFile)
        {
            this.configFile = configFile;
        }

        #endregion ctor

        #region IAppDomain

        /// <summary>
        /// Gets an integer that uniquely identifies the application domain within the process.
        /// </summary>
        int IAppDomain.Id
        {
            get
            {
                return this.GetHashCode();
            }
        }
        /// <summary>
        /// Gets or sets the base directory that the assembly resolver uses to probe for assemblies.
        /// </summary>
        string IAppDomain.BaseDirectory
        {
            get
            {
                return AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        /// <summary>
        /// Gets or sets the name of the configuration file for an application domain.
        /// </summary>
        string IAppDomain.ConfigurationFile
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets or set the friendly name.
        /// </summary>
        string IAppDomain.FriendlyName
        {
            get
            {
                return configFile.FullName;
            }
        }

        /// <summary>
        /// Gets or sets the list of directories under the application base directory that are probed for private assemblies.
        /// </summary>
        IEnumerable<string> IAppDomain.PrivateBinPath
        {
            get
            {
                return new[] { configFile.DirectoryName };
            }
        }

        /// <summary>
        /// Domain unloaded event.
        /// </summary>
        event EventHandler<EventArgs> IAppDomain.DomainUnload
        {
            add
            {
                this.domainUnload += value;
            }

            remove
            {
                this.domainUnload -= value;
            }
        }

        /// <summary>
        /// Process exit event.
        /// </summary>
        event EventHandler<EventArgs> IAppDomain.ProcessExit
        {
            add
            {
                this.processExit += value;
            }

            remove
            {
                this.processExit -= value;
            }
        }

        IEnumerable<Assembly> IAppDomain.GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
        }

        #endregion IAppDomain
    }
}