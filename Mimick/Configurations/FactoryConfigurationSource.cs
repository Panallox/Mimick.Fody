using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Configurations
{
    /// <summary>
    /// A configured source class which loads values using a factory.
    /// </summary>
    public sealed class FactoryConfigurationSource : IConfigurationSource
    {
        private readonly Func<string, string> factory;

        public FactoryConfigurationSource(Func<string, string> factory) => this.factory = factory ?? throw new ArgumentNullException(nameof(factory));

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }

        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        public void Load()
        {

        }

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        public void Refresh()
        {

        }

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>
        /// The configuration value; otherwise, <c>null</c> if the configuration could not be found.
        /// </returns>
        public string Resolve(string name) => factory(name);

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
