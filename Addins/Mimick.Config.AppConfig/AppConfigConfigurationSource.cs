using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A configuration source class which loads values from an application configuration source.
    /// </summary>
    public sealed class AppConfigConfigurationSource : IConfigurationSource
    {
        private readonly ReaderWriterLockSlim sync;

        private Configuration configuration;
        private FileInfo path;
        private AppConfigSource source;
        private ConfigurationUserLevel userLevel;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigConfigurationSource" /> class.
        /// </summary>
        /// <param name="level">The configuration user level.</param>
        public AppConfigConfigurationSource(ConfigurationUserLevel level = ConfigurationUserLevel.None)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(level);
            source = AppConfigSource.Default;
            sync = new ReaderWriterLockSlim();
            userLevel = level;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonConfigurationSource"/> class.
        /// </summary>
        /// <param name="exeName">The full path to the executable whose configuration must be loaded.</param>
        public AppConfigConfigurationSource(string exeName)
        {
            configuration = ConfigurationManager.OpenExeConfiguration(exeName);
            path = new FileInfo(exeName ?? throw new ArgumentNullException(nameof(exeName)));
            source = AppConfigSource.File;
            sync = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigConfigurationSource"/> class.
        /// </summary>
        /// <param name="config">The configuration.</param>
        /// <exception cref="System.ArgumentNullException">config</exception>
        public AppConfigConfigurationSource(Configuration config)
        {
            configuration = config ?? throw new ArgumentNullException(nameof(config));
            source = AppConfigSource.Direct;
            sync = new ReaderWriterLockSlim();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing) { }

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        /// <exception cref="ConfigurationException"></exception>
        public void Load()
        {
            sync.EnterWriteLock();

            try
            {
                switch (source)
                {
                    case AppConfigSource.Default:
                        configuration = ConfigurationManager.OpenExeConfiguration(userLevel);
                        break;

                    case AppConfigSource.File:
                        configuration = ConfigurationManager.OpenExeConfiguration(path.FullName);
                        break;
                }
            }
            catch (Exception ex)
            {
                throw new ConfigurationException($"Cannot load a {source.ToString().ToLower()} application configuration document", ex);
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
                return configuration.AppSettings.Settings[name]?.Value ?? configuration.ConnectionStrings.ConnectionStrings[name]?.ConnectionString;
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

        /// <summary>
        /// Indicates the source of an application configuration source.
        /// </summary>
        private enum AppConfigSource
        {
            /// <summary>
            /// The configuration source was loaded from the default application configuration file.
            /// </summary>
            Default,

            /// <summary>
            /// The configuration source was provided an actual configuration object.
            /// </summary>
            Direct,

            /// <summary>
            /// The configuration source was provided a file path.
            /// </summary>
            File,
        }
    }
}
