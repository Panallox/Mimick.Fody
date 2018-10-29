using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing extension methods for common reflection operations.
    /// </summary>
    static class ReflectionExtensions
    {
        /// <summary>
        /// Determines whether the constructor is both default (no required parameters) and accessible (public).
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns><c>true</c> if the constructor is the default; otherwise, <c>false</c>.</returns>
        public static bool IsDefaultAndAccessible(this ConstructorInfo method)
        {
            if (method.IsAssembly || method.IsPrivate)
                return false;

            var parameters = method.GetParameters();

            return parameters.Length == 0 || parameters.All(p => p.IsOptional);
        }

        /// <summary>
        /// Determines whether the specified type should be considered a system type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type should be considered a system type; otherwise, <c>false</c>.</returns>
        public static bool IsSystem(this Type type)
        {
            var m = type.Module.Name;

            if (m == "System" || m == "mscorlib" || m == "netstandard" || m == "WindowsBase" || m == "testhost")
                return true;

            if (m.StartsWith("System.") || m.StartsWith("Microsoft.") || m.StartsWith("Windows."))
                return true;

            if (type.Namespace.StartsWith("System."))
                return true;

            return false;
        }
    }
}
