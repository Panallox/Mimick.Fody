using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a source which loads and resolves configuration values.
    /// </summary>
    public interface IConfigurationSource
    {
        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        void Load();

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        void Refresh();

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>The configuration value; otherwise, <c>null</c> if the configuration could not be found.</returns>
        string Resolve(string name);

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns><c>true</c> if the configuration is resolved; otherwise, <c>false</c>.</returns>
        bool TryResolve(string name, out string value);
    }
}
