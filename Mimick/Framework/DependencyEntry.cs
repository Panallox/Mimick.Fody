using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing an entry within the dependency provider.
    /// </summary>
    sealed class DependencyEntry
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyEntry"/> class.
        /// </summary>
        /// <param name="interfaceType">Type of the interface.</param>
        /// <param name="concreteType">Type of the concrete.</param>
        /// <param name="constructor">The constructor.</param>
        /// <param name="lifetime">The lifetime.</param>
        public DependencyEntry(Type interfaceType, Type concreteType, Func<object> constructor, IDependencyLifetime lifetime)
        {
            ConcreteType = concreteType;
            Constructor = constructor;
            InterfaceType = interfaceType;
            Lifetime = lifetime;
        }

        #region Properties

        /// <summary>
        /// Gets the concrete type.
        /// </summary>
        public Type ConcreteType
        {
            get;
        }

        /// <summary>
        /// Gets the constructor used to create a new instance of the dependency.
        /// </summary>
        public Func<object> Constructor
        {
            get;
        }

        /// <summary>
        /// Gets the interface type.
        /// </summary>
        public Type InterfaceType
        {
            get;
        }

        /// <summary>
        /// Gets or sets the dependency lifetime scope.
        /// </summary>
        public IDependencyLifetime Lifetime
        {
            get; set;
        }

        #endregion
    }
}
