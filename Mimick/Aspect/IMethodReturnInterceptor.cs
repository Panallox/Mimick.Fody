using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a method return value interceptor which evaluates code against a method return value prior
    /// to the method returning.
    /// </summary>
    /// <remarks>
    /// A method return interceptor is called at the final stages of method execution, following any method or parameter interceptors. If
    /// an interceptor has changed the value being returned, then this value will be reflected in the event arguments.
    /// </remarks>
    public interface IMethodReturnInterceptor
    {
        /// <summary>
        /// Called when a method is invoked and is returning.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnReturn(MethodReturnInterceptionArgs e);
    }
}
