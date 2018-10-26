using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing the core information required during a weaving operation.
    /// </summary>
    public class WeaveContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeaveContext"/> class.
        /// </summary>
        /// <param name="module">The module.</param>
        public WeaveContext(ModuleDefinition module)
        {
            Module = module;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the candidate types which have been identified as eligible for scanning.
        /// </summary>
        public WeaveCandidates Candidates { get; set; }

        /// <summary>
        /// Gets the module definition.
        /// </summary>
        public ModuleDefinition Module { get; }

        /// <summary>
        /// Gets or sets the imported type and method references.
        /// </summary>
        public WeaveReferences Refs { get; set; }

        #endregion
    }
}
