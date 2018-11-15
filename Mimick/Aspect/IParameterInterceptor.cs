using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a method invocation parameter interceptor which provides additional code before the execution of a method.
    /// </summary>
    public interface IParameterInterceptor
    {
        /// <summary>
        /// Called when a method has been invoked, and executes before the method body and method interceptors.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnEnter(ParameterInterceptionArgs e);
    }
}
