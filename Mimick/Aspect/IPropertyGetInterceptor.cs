using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a property <c>get</c> method interceptor which encapsulates a method body with additional code.
    /// </summary>
    public interface IPropertyGetInterceptor
    {
        /// <summary>
        /// Called when a property <c>get</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        void OnException(PropertyInterceptionArgs e, Exception ex);

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        void OnExit(PropertyInterceptionArgs e);

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// If the property has a generated backing field, the value of the field will be loaded into the
        /// <see cref="PropertyInterceptionArgs.Value"/> property. If the value of this property is changed
        /// during the interception, the updated value will be copied into the backing field.
        /// </remarks>
        void OnGet(PropertyInterceptionArgs e);
    }
}
