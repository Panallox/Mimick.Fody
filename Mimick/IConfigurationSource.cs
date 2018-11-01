using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a configuration source which holds values which can be resolved.
    /// </summary>
    public interface IConfigurationSource : IDisposable
    {
        /// <summary>
        /// Initialize the configuration source by performing any preparatory operations.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the configuration source could not be initialized.</exception>
        void Initialize();

        /// <summary>
        /// Resolve a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The property value; otherwise, <c>null</c> if the property could not be resolved from this source.</returns>
        string Resolve(string name);
    }
}
