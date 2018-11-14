using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Framework
{
    /// <summary>
    /// A class representing a component which must be allocated on each reference.
    /// </summary>
    [Component(Scope.Adhoc)]
    public class AdhocComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdhocComponent"/> class.
        /// </summary>
        public AdhocComponent() => Guid = Guid.NewGuid();

        /// <summary>
        /// Gets the GUID associated with the component.
        /// </summary>
        public Guid Guid { get; }
    }
}
