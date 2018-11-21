using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Configurations
{
    /// <summary>
    /// A configuration source class which loads values directly from an <see cref="IDictionary{TKey, TValue}"/> source.
    /// </summary>
    public sealed class KeyValueConfigurationSource : IConfigurationSource
    {
        private readonly IDictionary<string, string> values;

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyValueConfigurationSource"/> class.
        /// </summary>
        /// <param name="source">The source.</param>
        public KeyValueConfigurationSource(IDictionary<string, string> source) => values = source;

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        public void Load() { }

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        public void Refresh() { }

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>
        /// The configuration value; otherwise, <c>null</c> if the configuration could not be found.
        /// </returns>
        public string Resolve(string name) => values.TryGetValue(name, out var value) ? value : null;

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns>
        ///   <c>true</c> if the configuration is resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryResolve(string name, out string value) => values.TryGetValue(name, out value);
    }
}
