using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An event arguments class containing information relevant to the interception of a parameter.
    /// </summary>
    public class ParameterInterceptionArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterInterceptionArgs" /> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="parameter">The parameter.</param>
        /// <param name="value">The value.</param>
        public ParameterInterceptionArgs(object instance, ParameterInfo parameter, object value)
        {
            Instance = instance;
            Parameter = parameter;
            Value = value;
        }

        #region Properties

        /// <summary>
        /// Gets the object instance for which the parameter interception is occurring. If the parent method is <c>static</c>
        /// then this value will be <c>null</c>.
        /// </summary>
        public object Instance
        {
            get;
        }

        /// <summary>
        /// Gets the parameter which has been intercepted.
        /// </summary>
        public ParameterInfo Parameter
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value of the argument which was supplied into the method during invocation.
        /// </summary>
        /// <remarks>
        /// The value assigned to this property must match the type of the parameter, and will not be copied
        /// back to the method arguments unless the option <see cref="CompilationOptionsAttribute.CopyArguments"/> is enabled.
        /// </remarks>
        public object Value
        {
            get; set;
        }

        #endregion
    }
}
