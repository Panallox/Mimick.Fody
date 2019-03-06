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
    /// A class containing methods for freezing objects, and checking whether objects are frozen.
    /// </summary>
    public static class Freezable
    {
        /// <summary>
        /// Gets an <see cref="IFreezable"/> reference from a provided target object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns>An <see cref="IFreezable"/> value.</returns>
        /// <exception cref="ArgumentNullException">If the target object is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the target object is not a freezable type.</exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static IFreezable GetFreezable(object target)
        {
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            return target as IFreezable ?? throw new ArgumentException($"The object type {target.GetType().FullName} does not implement the IFreezable interface");
        }

        /// <summary>
        /// Freeze a provided object.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <exception cref="ArgumentNullException">If the target object is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the target object is not a freezable type.</exception>
        /// <exception cref="FrozenException">If the target object has already been frozen.</exception>
        public static void Freeze(object target) => GetFreezable(target).Freeze();

        /// <summary>
        /// Gets whether a provided object has been frozen.
        /// </summary>
        /// <param name="target">The target object.</param>
        /// <returns><c>true</c> if the object has been frozen; otherwise, <c>false</c>.</returns>
        /// <exception cref="ArgumentNullException">If the target object is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">If the target object is not a freezable type.</exception>
        public static bool IsFrozen(object target) => GetFreezable(target).IsFrozen;
    }
}
