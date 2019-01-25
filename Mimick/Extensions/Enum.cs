using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common enum operations.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Gets a description for the enumerator value based on a <see cref="DescriptionAttribute"/> associated with the enum.
        /// </summary>
        /// <param name="value">The enumerator value.</param>
        /// <returns>The description value associated with the enumerator value; otherwise, <c>null</c>.</returns>
        public static string GetDescription(this Enum value) => value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(DescriptionAttribute), false).Cast<DescriptionAttribute>().FirstOrDefault()?.Description;
    }
}
