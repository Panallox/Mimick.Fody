using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// A class representing a constant value parsed from a value expression.
    /// </summary>
    internal sealed class Constant
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Constant"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public Constant(object value)
        {
            if (value == null)
                throw new ArgumentNullException("value", "A constant must have a value");

            Type = System.Type.GetTypeCode(value.GetType());
            Value = value;
        }

        #region Properties

        /// <summary>
        /// Gets the type of the constant.
        /// </summary>
        public TypeCode Type
        {
            get;
        }

        /// <summary>
        /// Gets the value of the constant.
        /// </summary>
        public object Value
        {
            get;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Value == null ? "<null>" : Value.ToString();
    }
}
