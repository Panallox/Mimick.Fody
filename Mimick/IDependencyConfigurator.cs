using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a configuration for one or more dependencies which have been registered.
    /// </summary>
    public interface IDependencyConfigurator
    {
        /// <summary>
        /// Registers the dependency as ad-hoc, being allocated when required.
        /// </summary>
        void Adhoc();

        /// <summary>
        /// Registers the dependency as a singleton instance within the dependency provider.
        /// </summary>
        void Singleton();
    }
}
