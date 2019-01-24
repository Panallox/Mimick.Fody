using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common string operations.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Gets whether the current value matches any of the provided comparison values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="values">The values to compare against the current value.</param>
        /// <returns><c>true</c> if the value matches one of the comparison values; otherwise, <c>false</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsAny(this string value, params string[] values) => IsAny(value, StringComparison.Ordinal, values);

        /// <summary>
        /// Gets whether the current value matches any of the provided comparison values.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="comparison">The comparison mode to use.</param>
        /// <param name="values">The values to compare against the current value.</param>
        /// <returns><c>true</c> if the value matches one of the comparison values; otherwise, <c>false</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsAny(this string value, StringComparison comparison, params string[] values) => values.Any(e => string.Equals(value, e, comparison));

        /// <summary>
        /// Gets whether the value is <c>null</c> or contains only whitespaces.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is <c>null</c> or contains only whitespaces; otherwise, <c>false</c>/</returns>
        public static bool IsBlank(this string value)
        {
#if NET461
            if (value == null)
                return true;

            for (int i = 0, count = value.Length; i < count; i++)
            {
                if (!char.IsWhiteSpace(value[i]))
                    return false;
            }

            return true;
#else
            return string.IsNullOrWhiteSpace(value);
#endif
        }

        /// <summary>
        /// Gets whether the value is <c>null</c> or contains no characters.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is <c>null</c> or contains no characters; otherwise, <c>false</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsEmpty(this string value)
        {
#if NET461
            return value == null || value.Length == 0;
#else
            return string.IsNullOrEmpty(value);
#endif
        }

        /// <summary>
        /// Gets whether the value is not <c>null</c> and does not contain only whitespaces.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is not <c>null</c> and does not contain only whitespaces; otherwise, <c>false</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNotBlank(this string value) => !IsBlank(value);

        /// <summary>
        /// Gets whether the value is not <c>null</c> and contains at-least one character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the value is not <c>null</c> and contains at-least one character; otherwise, <c>false</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static bool IsNotEmpty(this string value) => !IsEmpty(value);

        /// <summary>
        /// Gets whether the value is not <c>null</c> and contains a numeric value. 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsNumeric(this string value)
        {
            if (value == null || value.Length == 0)
                return false;

            var i = 0;
            var count = value.Length;

            if (value[0] == '-')
                i++;

            while (i < count)
            {
                if (!char.IsDigit(value[i++]))
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Retrieves a substring of the current value from the start of the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>The substring of the current value.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Left(this string value, int length) => value.Substring(0, length);

        /// <summary>
        /// Repeats the value a provided number of times.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="count">The count of the number of times the value should repeat.</param>
        /// <returns>A value containing the repeated values.</returns>
        public static string Repeat(this string value, int count)
        {
            if (count < 1)
                throw new ArgumentOutOfRangeException("count", "The count must be greater than zero");

            if (count == 1)
                return string.Copy(value);

            var builder = new StringBuilder(value.Length * count);

            while (count-- > 0)
                builder.Append(value);

            return builder.ToString();
        }

        /// <summary>
        /// Retrieves a substring of the current value from the end of the string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="length">The length of the substring.</param>
        /// <returns>The substring of the current value.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string Right(this string value, int length) => value.Substring(value.Length - length);

        /// <summary>
        /// Converts the current value into a byte array.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="encoding">The optional encoding to use.</param>
        /// <returns>A byte array value.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static byte[] ToByteArray(this string value, Encoding encoding = null) => (encoding ?? Encoding.ASCII).GetBytes(value);

        /// <summary>
        /// Converts the current value into an enumerator value.
        /// </summary>
        /// <typeparam name="T">The type of the enumerator.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>An enumerator value matching a <typeparamref name="T"/> value.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static T ToEnum<T>(this string value) => (T)Enum.Parse(typeof(T), value);

        /// <summary>
        /// Converts the current value into a <see cref="FileInfo"/> where the value represents the file path.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A <see cref="FileInfo"/> value.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static FileInfo ToFileInfo(this string value) => new FileInfo(value);

        /// <summary>
        /// Trims the current value and returns <c>null</c> if the trimmed value is empty.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The trimmed value; otherwise, <c>null</c>.</returns>
#if NET461
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
#endif
        public static string TrimToNull(this string value) => value == null || (value = value.Trim()).Length == 0 ? null : value;
    }
}
