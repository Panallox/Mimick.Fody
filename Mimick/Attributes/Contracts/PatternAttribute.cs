using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated parameter or property must not be <c>null</c> and must match a provided regular expression.
    /// When applied to a method all parameters are validated.
    /// </summary>
    /// <remarks>
    /// The attribute can be applied to any parameter type, and the argument will be converted into a <see cref="string"/> before the pattern is validated. If
    /// attempting to validate a pattern against a class object, ensure that the <see cref="object.ToString"/> method has been overridden to produce a value
    /// which can be validated.
    /// </remarks>
    [CompilationOptions(Scope = AttributeScope.MultiSingleton)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    [DebuggerStepThrough]
    public sealed class PatternAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PatternAttribute"/> class.
        /// </summary>
        /// <param name="pattern">The regular expression pattern which must be matched.</param>
        public PatternAttribute(string pattern) => Pattern = new Regex(pattern);

        #region Properties

        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        public Regex Pattern
        {
            get;
        }

        #endregion

        /// <summary>
        /// Validate the value of the parameter or property.
        /// </summary>
        /// <param name="name">The parameter or property name.</param>
        /// <param name="type">The parameter or property type.</param>
        /// <param name="value">The value.</param>
        public override void Validate(string name, Type type, object value)
        {
            if (value == null)
                throw new ArgumentNullException(name);

            var text = value as string ?? value.ToString();

            if (!Pattern.IsMatch(text))
                throw new ArgumentException($"The value does not match the expected format", name);
        }
    }
}
