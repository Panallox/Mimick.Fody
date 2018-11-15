using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An event arguments class containing information relevant to a method interception event.
    /// </summary>
    public class MethodInterceptionArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MethodInterceptionArgs"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <param name="arguments">The arguments.</param>
        /// <param name="returns">The returns.</param>
        /// <param name="method">The method.</param>
        public MethodInterceptionArgs(object instance, object[] arguments, object returns, MethodBase method)
        {
            Arguments = arguments;
            Cancel = false;
            Instance = instance;
            Method = method;
            Return = returns;
        }

        #region Properties

        /// <summary>
        /// Gets the arguments which were supplied into the method during invocation. The property cannot be changed, but
        /// the individual argument values can be changed.
        /// </summary>
        /// <remarks>
        /// The values assigned to the arguments array must match those defined against the method. If an invalid value is
        /// assigned then the application will throw an exception.
        /// </remarks>
        public object[] Arguments
        {
            get;
        }

        /// <summary>
        /// Gets or sets whether the method invocation should terminate immediately.
        /// </summary>
        public bool Cancel
        {
            get; set;
        }

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
        /// Gets or sets the value which must be returned from the method. If the method does not have a return value
        /// then the value assigned here will be ignored.
        /// </summary>
        /// <remarks>
        /// The value assigned to this property must match that of the return type of the method. If the method does
        /// not return a value then any value assigned here will be discarded.
        /// </remarks>
        public object Return
        {
            get; set;
        }

        #endregion
    }
}
