using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Behaviours
{
    /// <summary>
    /// A class expected to implement the <see cref="IFreezable"/> interface.
    /// </summary>
    [Freezable]
    public class FreezableAttributes
    {
        /// <summary>
        /// Gets or sets a unique identifier.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets a name.
        /// </summary>
        public string Name { get; set; }
    }
}
