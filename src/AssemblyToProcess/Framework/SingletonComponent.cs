using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Framework
{
    /// <summary>
    /// A class representing a component which must persistent during the lifetime of the framework.
    /// </summary>
    [Component(Scope.Singleton)]
    public class SingletonComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonComponent"/> class.
        /// </summary>
        public SingletonComponent() => Guid = Guid.NewGuid();

        /// <summary>
        /// Gets the GUID associated with the component.
        /// </summary>
        public Guid Guid { get; }
    }
}
