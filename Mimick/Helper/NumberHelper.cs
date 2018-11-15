using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing helper methods for numeric operations.
    /// </summary>
    public static class NumberHelper
    {
        /// <summary>
        /// Consumes a numeric value, cast to a <see cref="double"/>, to the appropriate smallest storage size.
        /// </summary>
        /// <param name="value">The value to convert.</param>
        /// <returns>The converted value.</returns>
        public static object Shrink(double value)
        {
            if (Math.Truncate(value) != value)
            {
                if (value >= float.MinValue && value <= float.MaxValue)
                    return (float)value;
            }
            else if (value >= sbyte.MinValue && value <= sbyte.MaxValue)
                return (sbyte)value;
            else if (value >= byte.MinValue && value <= byte.MaxValue)
                return (byte)value;
            else if (value >= short.MinValue && value <= short.MaxValue)
                return (short)value;
            else if (value >= ushort.MinValue && value <= ushort.MaxValue)
                return (ushort)value;
            else if (value >= int.MinValue && value <= int.MaxValue)
                return (int)value;
            else if (value >= uint.MinValue && value <= uint.MaxValue)
                return (uint)value;
            else if (value >= long.MinValue && value <= long.MaxValue)
                return (long)value;
            else if (value >= ulong.MinValue && value <= ulong.MaxValue)
                return (ulong)value;

            return value;
        }
    }
}
