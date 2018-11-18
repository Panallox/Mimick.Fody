using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on a property which has been identified as a candidate for interceptor weaving.
    /// </summary>
    public class PropertyInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the field which was replaced by the property, used when routing field replacements.
        /// </summary>
        public FieldDefinition Field { get; set; }
        
        /// <summary>
        /// Gets or sets the interceptors.
        /// </summary>
        public CustomAttribute[] Interceptors { get; set; }

        /// <summary>
        /// Gets or sets the property.
        /// </summary>
        public PropertyDefinition Property { get; set; }

        #endregion
    }
}
