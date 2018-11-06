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
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    [DebuggerStepThrough]
    public sealed class NotNullAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNullAttribute" /> class.
        /// </summary>
        public NotNullAttribute()
        {

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
