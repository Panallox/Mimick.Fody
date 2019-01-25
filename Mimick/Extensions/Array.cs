using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common array operations.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Create a copy of the array.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <returns>The new array instance containing the copied values.</returns>
        public static T[] Copy<T>(this T[] array) => Copy(array, 0, array.Length);

        /// <summary>
        /// Create a copy of the array.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">The zero-based array index to begin copying from.</param>
        /// <returns>The new array instance containing the copied values.</returns>
        public static T[] Copy<T>(this T[] array, int index) => Copy(array, index, array.Length - index);

        /// <summary>
        /// Create a copy of the array.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">The zero-based array index to begin copying from.</param>
        /// <param name="count">The number of elements to copy.</param>
        /// <returns>The new array instance containing the copied values.</returns>
        public static T[] Copy<T>(this T[] array, int index, int count)
        {
            T[] target = new T[count];
            Array.Copy(array, index, target, 0, count);
            return target;
        }

        /// <summary>
        /// Fills all elements of the array with the provided value.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The array fill value.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(this T[] array, T value) => Fill(array, value, 0, array.Length);

        /// <summary>
        /// Fills all elements of the array with the provided value.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="value">The array fill value.</param>
        /// <param name="index">The zero-based array index to begin filling from.</param>
        /// <param name="count">The number of elements to fill.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Fill<T>(this T[] array, T value, int index, int count)
        {
            for (int i = 0; i < count; i++)
                array[i + index] = value;
        }

        /// <summary>
        /// Slices the array at the provided index, returning a new array containing the sliced elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">The zero-based index of the array to slice from.</param>
        /// <returns>A new array containing the sliced elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Slice<T>(this T[] array, int index) => Copy(array, index);

        /// <summary>
        /// Slices the array at the provided index, returning a new array containing the sliced elements.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="index">The zero-based index of the array to slice from.</param>
        /// <param name="count">The number of elements to slice.</param>
        /// <returns>A new array containing the sliced elements.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T[] Slice<T>(this T[] array, int index, int count) => Copy(array, index, count);
    }
}
