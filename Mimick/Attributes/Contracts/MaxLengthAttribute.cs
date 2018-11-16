using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated parameter or property must have a total length of less than or equal to the provided amount. When applied to a
    /// method, all parameters are validated.
    /// </summary>
    /// <remarks>
    /// The attribute can be applied to <see cref="string"/>, <see cref="StringBuilder"/>, <see cref="IList" />, <see cref="ICollection"/>, <see cref="IEnumerable"/>
    /// and <see cref="Array"/> types. If the value of the parameter or property is <c>null</c> the value will pass validation.
    /// </remarks>
    [CompilationOptions(Scope = AttributeScope.MultiSingleton)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    [DebuggerStepThrough]
    public sealed class MaxLengthAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MaxLengthAttribute"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public MaxLengthAttribute(int value) => Value = value;

        #region Properties

        /// <summary>
        /// Gets the maximum length value.
        /// </summary>
        public int Value
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
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public override void Validate(string name, Type type, object value)
        {
            if (value == null)
                return;

            var valid = true;

            if (value is string text)
                valid = text.Length <= Value;
            else if (value is StringBuilder builder)
                valid = builder.Length <= Value;
            else if (value is IList list)
                valid = list.Count <= Value;
            else if (value is ICollection collection)
                valid = collection.Count <= Value;
            else if (value is Array array)
                valid = array.Length <= Value;
            else if (value is IEnumerable enumerable)
                valid = enumerable.Count() <= Value;

            if (!valid)
                throw new ArgumentOutOfRangeException(name, $"The value cannot exceed a length or count of {Value}");
        }
    }
}
