using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods for validating objects and values.
    /// </summary>
    [DebuggerStepThrough]
    public static class Contract
    {
        /// <summary>
        /// Ensures that a provided value must be greater than a provided minimum.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The comparable value.</param>
        /// <param name="minimum">The minimum value.</param>
        /// <returns>The value.</returns>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T GreaterThan<T>(T value, T minimum) where T : IComparable<T> => value.CompareTo(minimum) > 0 ? value : throw new IndexOutOfRangeException();

        /// <summary>
        /// Ensures that a provided string cannot be <c>null</c>, blank or consist of only whitespaces.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The string.</returns>
        /// <exception cref="EmptyException">If the string is <c>null</c>, empty or contain only whitespaces.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string NotBlank(string str, string arg = null) => !string.IsNullOrWhiteSpace(str) ? str : throw new EmptyException(arg);

        /// <summary>
        /// Ensures that a provided string builder cannot be <c>null</c>, blank or consist of only whitespaces.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The string builder.</returns>
        /// <exception cref="EmptyException">If the string builder is <c>null</c>, empty or contain only whitespaces.</exception>
        public static StringBuilder NotBlank(StringBuilder builder, string arg = null)
        {
            if (builder != null)
            {
                for (int i = 0, count = builder.Length; i < count; i++)
                {
                    if (!char.IsWhiteSpace(builder[i]))
                        return builder;
                }
            }

            throw new EmptyException(arg);
        }

        /// <summary>
        /// Ensures that a provided array cannot be <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">The type of the elements.</typeparam>
        /// <param name="array">The array.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The array.</returns>
        /// <exception cref="EmptyException">If the array is <c>null</c> or empty.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T[] NotEmpty<T>(T[] array, string arg = null) => array != null && array.Length > 0 ? array : throw new EmptyException(arg);

        /// <summary>
        /// Ensures that a provided collection cannot be <c>null</c> or empty.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <typeparam name="U">The type of the elements.</typeparam>
        /// <param name="collection">The object.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The object.</returns>
        /// <exception cref="EmptyException">If the object is <c>null</c> or empty.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T NotEmpty<T, U>(T collection, string arg = null) where T : IList<U> => collection != null && collection.Count > 0 ? collection : throw new EmptyException(arg);

        /// <summary>
        /// Ensures that a provided string cannot be <c>null</c> or empty.
        /// </summary>
        /// <param name="str">The string.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The string.</returns>
        /// <exception cref="EmptyException">If the string is <c>null</c> or empty.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string NotEmpty(string str, string arg = null) => !string.IsNullOrEmpty(str) ? str : throw new EmptyException(arg);

        /// <summary>
        /// Ensures that a provided string builder cannot be <c>null</c> or empty.
        /// </summary>
        /// <param name="builder">The string builder.</param>
        /// <param name="arg">The optional argument name.</param>
        /// <returns>The string builder.</returns>
        /// <exception cref="EmptyException">If the string builder is <c>null</c> or empty.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static StringBuilder NotEmpty(StringBuilder builder, string arg = null) => builder != null && builder.Length > 0 ? builder : throw new EmptyException(arg);

        /// <summary>
        /// Ensures that a provided object cannot be <b>null</b>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="o">The object.</param>
        /// <returns>The object.</returns>
        /// <exception cref="NullReferenceException">If the object is <c>null</c>.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T NotNull<T>(T o) where T : class => o ?? throw new NullReferenceException();

        /// <summary>
        /// Ensures that a provided argument object cannot be <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type of the object.</typeparam>
        /// <param name="o">The object.</param>
        /// <param name="name">The argument name.</param>
        /// <returns>The object.</returns>
        /// <exception cref="ArgumentNullException">If the argument object is <c>null</c>.</exception>
#if NET45
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T NotNull<T>(T o, string name) where T : class => o ?? throw new ArgumentNullException(name);

    }
}
