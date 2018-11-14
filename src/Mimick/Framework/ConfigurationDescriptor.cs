using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class implementation of the configuration descriptor which manages information on a configuration source of the framework.
    /// </summary>
    sealed class ConfigurationDescriptor : IConfigurationDescriptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationDescriptor"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public ConfigurationDescriptor(IConfigurationSource source)
        {
            ExpiresAt = Timeout.InfiniteTimeSpan;
            LastUpdated = DateTime.MinValue;
            Source = source;
        }

        #region Properties

        /// <summary>
        /// Gets the duration of time, from when the configuration source is loaded, to maintain the configuration source before issuing
        /// a <see cref="IConfigurationSource.Refresh" /> invocation.
        /// </summary>
        public TimeSpan ExpiresAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the time when the configuration was last updated.
        /// </summary>
        public DateTime LastUpdated
        {
            get; set;
        }

        /// <summary>
        /// Gets the configuration source.
        /// </summary>
        public IConfigurationSource Source
        {
            get;
        }

        #endregion

        /// <summary>
        /// Determines whether the configuration source needs to be reloaded after reaching the maximum expiry period.
        /// </summary>
        /// <returns><c>true</c> if the source must be updated; otherwise, <c>false</c>.</returns>
        public bool IsUpdateRequired() => ExpiresAt != Timeout.InfiniteTimeSpan && (LastUpdated + ExpiresAt) > DateTime.Now;
    }
}
