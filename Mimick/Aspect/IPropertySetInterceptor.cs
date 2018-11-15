using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a property <c>set</c> method interceptor which encapsulates a method body with additional code.
    /// </summary>
    public interface IPropertySetInterceptor
    {
        /// <summary>
        /// Called when a property <c>set</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        void OnException(PropertyInterceptionArgs e, Exception ex);

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnExit(PropertyInterceptionArgs e);

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// The value of the <see cref="PropertyInterceptionArgs.Value"/> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        void OnSet(PropertyInterceptionArgs e);
    }
}
