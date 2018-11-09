using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mono.Cecil;

namespace Mimick.Fody
{
    /// <summary>
    /// A class containing information on a parameter interceptor, at either the method or parameter level.
    /// </summary>
    public class ParameterInterceptorInfo
    {
        #region Properties

        /// <summary>
        /// Gets or sets the interceptor attributes.
        /// </summary>
        public CustomAttribute[] Attributes
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the parameter index, where a value of <c>-1</c> indicates all parameters.
        /// </summary>
        public int Index
        {
            get; set;
        }

        #endregion
    }
}
