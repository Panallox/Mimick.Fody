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
    /// Indicates that the associated property, parameter or return value should be converted to lower-case. When applied to a method all parameters
    /// will be converted where supported.
    /// </summary>
    /// <remarks>
    /// This attribute will work against types which can be cased, including <see cref="string"/> and <see cref="StringBuilder"/>. If the value is <c>null</c> or the value type
    /// is not supported then the attribute will not perform any casing operation.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [DebuggerStepThrough]
    public class LowerCaseAttribute : ActionAttribute
    {
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
                return s.ToLower();
            else if (value is StringBuilder o)
            {
                for (int i = 0, length = o.Length; i < length; i++)
                    o[i] = char.ToLower(o[i]);
            }

            return value;
        }
    }
}
