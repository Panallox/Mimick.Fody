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
    /// Indicates that the associated property, parameter or method return value should be trimmed. When applied to a method, all parameters will be trimmed where supported.
    /// </summary>
    /// <remarks>
    /// This attribute will work against types which can be trimmed, including <see cref="string"/> and <see cref="StringBuilder"/>. If the value is <c>null</c> or the value type
    /// is not supported then the attribute will not perform any trim operation.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue)]
    [DebuggerStepThrough]
    public class TrimAttribute : ActionAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrimAttribute" /> class.
        /// </summary>
        public TrimAttribute()
        {

        }

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
                return s.Trim();
            else if (value is StringBuilder o)
            {
                var i = 0;
                var length = o.Length;

                if (length == 0)
                    return o;

                while (i < length && char.IsWhiteSpace(o[i]))
                    i++;

                if (i > 0)
                {
                    o = o.Remove(0, i);
                    length = o.Length;

                    if (length == 0)
                        return o;
                }

                i = length - 1;

                while (i >= 0 && char.IsWhiteSpace(o[i]))
                    i--;

                if (i != length - 1)
                    o = o.Remove(i, length - i);

                return o;
            }

            return value;
        }
    }
}
