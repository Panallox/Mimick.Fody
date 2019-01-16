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
    /// Indicates that the associated property, parameter or return value should have values replaced depending on a provided pattern and replacement
    /// value. When applied to a method all parameters will be replaced where supported.
    /// </summary>
    /// <remarks>
    /// This attribute will work against types which can be replaced, including <see cref="string"/> and <see cref="StringBuilder"/>. If the value is <c>null</c> or the value type
    /// is not supported then the attribute will not perform any replace operation.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [CompilationOptions(CopyArguments = true, Scope = AttributeScope.MultiSingleton)]
    [DebuggerStepThrough]
    public class ReplaceAttribute : ActionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceAttribute" /> class.
        /// </summary>
        /// <param name="regex">The regular expression pattern used to match the values to replace.</param>
        /// <param name="replacement">The replacement value which should replace the matches.</param>
        public ReplaceAttribute(string regex, string replacement) : this(regex, replacement, RegexOptions.Compiled)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ReplaceAttribute" /> class.
        /// </summary>
        /// <param name="regex">The regular expression pattern used to match the values to replace.</param>
        /// <param name="replacement">The replacement value which should replace the matches.</param>
        /// <param name="options">The regular expression options.</param>
        public ReplaceAttribute(string regex, string replacement, RegexOptions options)
        {
            Regex = new Regex(regex, options);
            Replacement = replacement;
        }

        #region Properties

        /// <summary>
        /// Gets the regular expression pattern.
        /// </summary>
        public Regex Regex
        {
            get;
        }

        /// <summary>
        /// Gets the replacement value.
        /// </summary>
        public string Replacement
        {
            get;
        }

        #endregion

        /// <summary>
        /// Applies the action to the parameter, property or return value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The value type.</param>
        /// <returns>
        /// The actioned value.
        /// </returns>
        protected override object Apply(object value, Type type)
        {
            if (value == null)
                return null;

            if (value is string s)
                return Regex.Replace(s, Replacement);
            else if (value is StringBuilder b)
            {
                var temp = Regex.Replace(b.ToString(), Replacement);
                return new StringBuilder(temp);
            }

            return value;
        }
    }
}
