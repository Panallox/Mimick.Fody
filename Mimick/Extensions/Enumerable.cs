using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common enumeration operations.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Counts the number of elements in a non-generic enumerable collection.
        /// </summary>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <returns>The number of elements in the collection.</returns>
        public static int Count(this IEnumerable enumerable)
        {
            if (enumerable == null)
                return 0;

            var count = 0;
            var enumerator = enumerable.GetEnumerator();

            while (enumerator.MoveNext())
                count++;

            return count;
        }

        /// <summary>
        /// Filters the enumerable collection by values which are not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <returns>The enumerable collection.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IEnumerable<T> Exists<T>(this IEnumerable<T> enumerable) => enumerable.Where(e => e != null);

        /// <summary>
        /// Converts the enumerable collection to an immutable read-only list.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <returns>An <see cref="IReadOnlyList{T}"/> value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IReadOnlyList<T> ToReadOnly<T>(this IEnumerable<T> enumerable) => new ReadOnlyList<T>(enumerable);

        /// <summary>
        /// Joins the content of an enumerable collection into a <see cref="string"/> value using the provided delimiter.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="enumerable">The enumerable collection.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>The resulting string value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ToString<T>(this IEnumerable<T> enumerable, string delimiter) => string.Join(delimiter, enumerable);
    }
}
