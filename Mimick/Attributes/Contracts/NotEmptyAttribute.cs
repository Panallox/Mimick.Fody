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
    /// Indicates that the associated parameter or property should not be <c>null</c> or empty. When applied to a method, all parameters will be validated.
    /// </summary>
    /// <remarks>
    /// The attribute will work against types which can be considered empty, including <see cref="string"/>, <see cref="ICollection"/>,
    /// <see cref="IEnumerable"/>, <see cref="Array"/>.
    /// </remarks>
    [CompilationOptions(Scope = AttributeScope.Singleton)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    [DebuggerStepThrough]
    public sealed class NotEmptyAttribute : ValidationAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotEmptyAttribute" /> class.
        /// </summary>
        public NotEmptyAttribute()
        {

        }

        /// <summary>
        /// Validate the value of the parameter or property.
        /// </summary>
        /// <param name="name">The parameter or property name.</param>
        /// <param name="type">The parameter or property type.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentException">The value cannot be null or empty</exception>
        public override void Validate(string name, Type type, object value)
        {
            var empty = false;

            if (value == null)
                empty = true;
            else if (value is string text)
                empty = text.Length == 0;
            else if (value is StringBuilder builder)
                empty = builder.Length == 0;
            else if (value is Array array)
                empty = array.Length == 0;
            else if (value is ICollection collection)
                empty = collection.Count == 0;
            else if (value is IEnumerable enumerable)
                empty = !enumerable.GetEnumerator().MoveNext();

            if (empty)
                throw new ArgumentException(name, "The value cannot be null or empty");
        }
    }
}
