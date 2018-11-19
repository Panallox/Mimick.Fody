using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An event arguments class containing information relevant to a method return interception event.
    /// </summary>
    public class MethodReturnInterceptionArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodReturnInterceptionArgs"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="returns">The returns.</param>
        /// <param name="method">The method.</param>
        public MethodReturnInterceptionArgs(object instance, object returns, MethodBase method)
        {
            Instance = instance;
            Method = method;
            Value = returns;
        }

        #region Properties

        /// <summary>
        /// Gets the object instance for which the method interception is occurring. If the method is <c>static</c>
        /// then this value will be <c>null</c>.
        /// </summary>
        public object Instance
        {
            get;
        }

        /// <summary>
        /// Gets the method which has been intercepted.
        /// </summary>
        public MethodBase Method
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value which has been returned from the method. If the method does not have a return value
        /// then the value here will be <c>null</c>.
        /// </summary>
        /// <remarks>
        /// The value assigned to this property must match that of the return type of the method. If the method does
        /// not return a value then any value assigned here will be discarded.
        /// </remarks>
        public object Value
        {
            get; set;
        }

        #endregion
    }
}
