using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Configuration
{
    /// <summary>
    /// A class representing a configuration source that reads from the standard <c>app.config</c> file.
    /// </summary>
    class AppConfigConfigurationSource : IConfigurationSource
    {
        private System.Configuration.Configuration configuration;

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
            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
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
            var settings = configuration.AppSettings.Settings;
            var match = settings[name];
            return match?.Value;
        }
    }
}
