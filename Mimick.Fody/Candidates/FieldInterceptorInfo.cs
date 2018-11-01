using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on a field which has been identified as a candidate for interceptor weaving.
    /// </summary>
    public class FieldInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field.
        /// </summary>
        public FieldDefinition Field { get; set; }

        /// <summary>
        /// Gets or sets the interceptors.
        /// </summary>
        public CustomAttribute[] Interceptors { get; set; }

        #endregion
    }
}
