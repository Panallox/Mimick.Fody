using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a lifetime provider for a dependency.
    /// </summary>
    public interface IDependencyLifetime : IDisposable
    {
        /// <summary>
        /// Resolve the dependency using the mechanism configured by the implementation.
        /// </summary>
        /// <returns>The dependency instance.</returns>
        object Resolve();
    }
}
