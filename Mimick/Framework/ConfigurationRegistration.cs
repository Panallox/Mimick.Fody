using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class providing options for configuring a configuration registration.
    /// </summary>
    sealed class ConfigurationRegistration : IConfigurationRegistration
    {
        private readonly ConfigurationDescriptor descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationRegistration"/> class.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        public ConfigurationRegistration(ConfigurationDescriptor configuration) => descriptor = configuration;

        /// <summary>
        /// Sets the duration of time after which the configuration source must be reloaded into memory using the
        /// <see cref="IConfigurationSource.Refresh" /> method. The expiration is set to <see cref="Timeout.Infinite" /> by default.
        /// </summary>
        /// <param name="duration">The duration of time.</param>
        /// <returns></returns>
        public IConfigurationRegistration Expires(TimeSpan duration)
        {
            descriptor.ExpiresAt = duration;
            return this;
        }
    }
}
