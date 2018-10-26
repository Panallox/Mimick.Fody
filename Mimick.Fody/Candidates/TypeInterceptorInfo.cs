using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on a type which has been identified as a candidate for interceptor weaving.
    /// </summary>
    public class TypeInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the methods.
        /// </summary>
        public MethodInterceptorInfo[] Methods { get; set; }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public TypeDefinition Type { get; set; }

        #endregion
    }
}
