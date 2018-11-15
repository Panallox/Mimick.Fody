using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on interceptors or attributes which should be applied to the constructors of a type.
    /// </summary>
    public class ConstructorInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the initializers which should be introduced to all instanced constructors.
        /// </summary>
        public MethodDefinition[] Initializers
        {
            get; set;
        }

        #endregion
    }
}
