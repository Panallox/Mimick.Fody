using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common object operations.
    /// </summary>
    public static partial class Extensions
    {
        /// <summary>
        /// Invokes an action if the object value is not <c>null</c>.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <param name="action">The action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfNotNull(this object o, Action action)
        {
            if (o != null)
                action();
        }

        /// <summary>
        /// Invokes an action if the object value is not <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type returned from the action.</typeparam>
        /// <param name="o">The object to check.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The result of the action if the value is <c>null</c>; otherwise, the default value of <typeparamref name="T"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfNotNull<T>(this object o, Func<T> action) => o != null ? action() : default;

        /// <summary>
        /// Invokes an action if the object value is <c>null</c>.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <param name="action">The action to invoke.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void IfNull(this object o, Action action)
        {
            if (o == null)
                action();
        }

        /// <summary>
        /// Invokes an action if the object value is <c>null</c>.
        /// </summary>
        /// <typeparam name="T">The type returned from the action.</typeparam>
        /// <param name="o">The object to check.</param>
        /// <param name="action">The action to invoke.</param>
        /// <returns>The result of the action if the value is <c>null</c>; otherwise, the default value of <typeparamref name="T"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static T IfNull<T>(this object o, Func<T> action) => o == null ? action() : default;

        /// <summary>
        /// Gets whether the object value is not <c>null</c>.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns><c>true</c> if the object is not <c>null</c>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNotNull(this object o) => o != null;

        /// <summary>
        /// Gets whether the object value is <c>null</c>.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns><c>true</c> if the object is <c>null</c>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNull(this object o) => o == null;
    }
}
