using Mono.Cecil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on a method which has been identified as a candidate for interceptor weaving.
    /// </summary>
    public class MethodInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the method.
        /// </summary>
        public MethodDefinition Method { get; set; }

        /// <summary>
        /// Gets or sets the method interceptors.
        /// </summary>
        public CustomAttribute[] MethodInterceptors { get; set; }

        /// <summary>
        /// Gets or sets the parameter interceptors.
        /// </summary>
        public CustomAttribute[] ParameterInterceptors { get; set; }

        #endregion
    }
}
