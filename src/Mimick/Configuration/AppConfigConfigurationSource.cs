using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Configuration
{
    /// <summary>
    /// A class representing a configuration source that reads from the standard <c>app.config</c> file.
    /// </summary>
    class AppConfigConfigurationSource : IConfigurationSource
    {
        private readonly ReaderWriterLockSlim locking = new ReaderWriterLockSlim();

        private System.Configuration.Configuration configuration;
        private Dictionary<string, string> values;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Initialize the configuration source by performing any preparatory operations.
        /// </summary>
        public void Initialize()
        {
            locking.EnterWriteLock();

            try
            {
                configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                values = new Dictionary<string, string>();

                foreach (KeyValueConfigurationElement element in configuration.AppSettings.Settings)
                    values.Add(element.Key, element.Value);
            }
            finally
            {
                locking.ExitWriteLock();
            }
        }

        /// <summary>
        /// Resolve a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>
        /// The property value; otherwise, <c>null</c> if the property could not be resolved from this source.
        /// </returns>
        public string Resolve(string name)
        {
            locking.EnterReadLock();

            try
            {
                return values.TryGetValue(name, out var value) ? value : null;
            }
            finally
            {
                locking.ExitReadLock();
            }
        }

        /// <summary>
        /// Refresh the configuration source causing any cached values to be reloaded from the source.
        /// </summary>
        public void Refresh() => Initialize();
    }
}
