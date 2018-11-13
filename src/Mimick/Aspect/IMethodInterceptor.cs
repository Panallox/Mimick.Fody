using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a method invocation interceptor which encapsulates a method body with additional code.
    /// </summary>
    /// <remarks>
    /// A method interceptor can be associated with constructors, method declarations, and properties. If the interceptor
    /// is associated with a property then the interception occurs against both the <c>get</c> and <c>set</c> methods.
    /// </remarks>
    public interface IMethodInterceptor
    {
        /// <summary>
        /// Called when a method has been invoked, and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnEnter(MethodInterceptionArgs e);

        /// <summary>
        /// Called when a method has been invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        void OnException(MethodInterceptionArgs e, Exception ex);

        /// <summary>
        /// Called when a method has been invoked, and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnExit(MethodInterceptionArgs e);
    }
}
