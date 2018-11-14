using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Configurations
{
    /// <summary>
    /// A configuration source class which loads values from the <c>app.config</c> configuration file.
    /// </summary>
    /// <seealso cref="Mimick.IConfigurationSource" />
    public sealed class AppConfigurationSource : IConfigurationSource
    {
        private readonly ReaderWriterLockSlim sync;

        private System.Configuration.Configuration configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigurationSource" /> class.
        /// </summary>
        public AppConfigurationSource()
        {
            configuration = null;
            sync = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        public void Load()
        {
            sync.EnterWriteLock();

            try
            {
                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            }
            finally
            {
                sync.ExitWriteLock();
            }
        }

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        public void Refresh() => Load();

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>
        /// The configuration value; otherwise, <c>null</c> if the configuration could not be found.
        /// </returns>
        public string Resolve(string name)
        {
            sync.EnterReadLock();

            try
            {
                return configuration.AppSettings.Settings[name]?.Value;
            }
            finally
            {
                sync.ExitReadLock();
            }
        }

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns>
        ///   <c>true</c> if the configuration is resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryResolve(string name, out string value) => (value = Resolve(name)) != null;
    }
}
