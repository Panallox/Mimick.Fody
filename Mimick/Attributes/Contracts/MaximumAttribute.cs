using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated parameter or property must have a value less than or equal to the provided amount. When applied to a
    /// method, all parameters are validated.
    /// </summary>
    /// <remarks>
    /// The attribute can only be applied to numeric values.
    /// </remarks>
    [CompilationOptions(Scope = AttributeScope.MultiSingleton)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [DebuggerStepThrough]
    public sealed class MaximumAttribute : ValidationAttribute, IMethodReturnInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MaximumAttribute(int value) => Value = value;

        #region Properties

        /// <summary>
        /// Gets or sets the minimum value.
        /// </summary>
        public double Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Called when a method is invoked and is returning.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnReturn(MethodReturnInterceptionArgs e) => Validate("", (e.Method as MethodInfo).ReturnType, e.Value);

        /// <summary>
        /// Validate the value of the parameter or property.
        /// </summary>
        /// <param name="name">The parameter or property name.</param>
        /// <param name="type">The parameter or property type.</param>
        /// <param name="value">The value.</param>
        public override void Validate(string name, Type type, object value)
        {
            if (value == null)
                return;

            if (!double.TryParse(value.ToString(), out var result))
                throw new ArgumentException(name, "The value is not a valid number");

            if (result > Value)
                throw new ArgumentOutOfRangeException(name, $"The value cannot be greater than {Value}");
        }
    }
}
