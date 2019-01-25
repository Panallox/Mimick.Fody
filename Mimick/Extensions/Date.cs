using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common date operations.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Gets whether the date is set in the future.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if the date is set in the future; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsFuture(this DateTime date) => date > DateTime.Now;

        /// <summary>
        /// Gets whether the date is set in the past.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns><c>true</c> if the date is set in the past; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPast(this DateTime date) => date < DateTime.Now;
    }
}
