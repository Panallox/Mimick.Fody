using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Lifetime
{
    /// <summary>
    /// A class representing a dependency which is created each time it's required.
    /// </summary>
    sealed class AdhocLifetime : IDependencyLifetime
    {
        private readonly Func<object> constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdhocLifetime"/> class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public AdhocLifetime(Func<object> constructor) => this.constructor = constructor;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // untracked
        }

        /// <summary>
        /// Resolve the dependency using the mechanism configured by the implementation.
        /// </summary>
        /// <returns>
        /// The dependency instance.
        /// </returns>
        public object Resolve() => constructor();
    }
}
