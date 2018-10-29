using Mimick.Lifetime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mimick.Framework.DependencyFactory;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing a configurator which can change how a dependency is resolved.
    /// </summary>
    sealed class DependencyConfigurator : IDependencyConfigurator
    {
        private readonly DependencyEntry[] values;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyConfigurator" /> class.
        /// </summary>
        /// <param name="values">The values.</param>
        public DependencyConfigurator(DependencyEntry[] values) => this.values = values;

        /// <summary>
        /// Registers the dependency as ad-hoc, being allocated when required.
        /// </summary>
        public void Adhoc()
        {
            foreach (var value in values)
                value.Lifetime = new AdhocLifetime(value.Constructor);
        }

        /// <summary>
        /// Registers the dependency as a singleton instance within the dependency provider.
        /// </summary>
        public void Singleton()
        {
            foreach (var value in values)
                value.Lifetime = new SingletonLifetime(value.Constructor);
        }
    }
}
