using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common object operations.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Checks whether the object value is <c>null</c>.
        /// </summary>
        /// <param name="o">The object to check.</param>
        /// <returns><c>true</c> if the object is <c>null</c>; otherwise, <c>false</c>.</returns>
        public static bool IsNull(this object o) => o == null;
    }
}
