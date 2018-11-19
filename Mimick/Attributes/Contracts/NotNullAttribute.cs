using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated parameter or property should not be <c>null</c>. When applied to a method, all parameters will be validated.
    /// </summary>
    [CompilationOptions(Scope = AttributeScope.Singleton)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [DebuggerStepThrough]
    public sealed class NotNullAttribute : ValidationAttribute, IMethodReturnInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullAttribute" /> class.
        /// </summary>
        public NotNullAttribute()
        {

        }

        /// <summary>
        /// Called when a method is invoked and is returning.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <exception cref="ArgumentNullException">Cannot return a null value</exception>
        public void OnReturn(MethodReturnInterceptionArgs e)
        {
            if (e.Value == null)
                throw new ArgumentNullException("", "Cannot return a null value");
        }

        /// <summary>
        /// Validate the value of the parameter or property.
        /// </summary>
        /// <param name="name">The parameter or property name.</param>
        /// <param name="type">The parameter or property type.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public override void Validate(string name, Type type, object value)
        {
            if (value == null)
                throw new ArgumentNullException(name);
        }
    }
}
