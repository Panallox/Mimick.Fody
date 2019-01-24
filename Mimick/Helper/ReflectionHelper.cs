using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods and constants for reflection operations.
    /// </summary>
    public static class ReflectionHelper
    {
        /// <summary>
        /// Binding flags used to find members that are both instanced or static, and public and non-public.
        /// </summary>
        public const BindingFlags All = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Binding flags used to find members that are instanced and public or non-public.
        /// </summary>
        public const BindingFlags AllInstanced = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        /// <summary>
        /// Gets an attribute from the provided member of the provided type, including any attributes inherited.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="member">The member.</param>
        /// <param name="inherit">Whether inherited attributes should also be selected.</param>
        /// <returns>The matched attribute value; otherwise, <c>null</c>.</returns>
        public static T GetAttributeInherited<T>(MemberInfo member, bool inherit = false) where T : Attribute
        {
            foreach (var attribute in member.GetCustomAttributes(inherit))
            {
                var type = attribute.GetType();

                if (IsSystem(type))
                    continue;

                if (attribute is T match)
                    return match;

                match = GetAttributeInherited<T>(type, inherit);

                if (match != null)
                    return match;
            }

            return null;
        }

        /// <summary>
        /// Gets a collection of methods from a provided type which have a provided attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="binding">The optional method bindings.</param>
        /// <returns>An <see cref="IEnumerable{MethodInfo}"/> value.</returns>
        public static IEnumerable<MethodInfo> GetMethodsWithAttribute<T>(Type type, BindingFlags binding = AllInstanced) where T : Attribute
            => type.GetMethods(binding).Where(m => GetAttributeInherited<T>(m, false) != null);

        /// <summary>
        /// Gets a collection of properties from a provided type which have a provided attribute.
        /// </summary>
        /// <typeparam name="T">The type of the attribute.</typeparam>
        /// <param name="type">The type to check.</param>
        /// <param name="binding">The optional method bindings.</param>
        /// <returns>An <see cref="IEnumerable{PropertyInfo}"/> value.</returns>
        public static IEnumerable<PropertyInfo> GetPropertiesWithAttribute<T>(Type type, BindingFlags binding = AllInstanced) where T : Attribute
            => type.GetProperties(binding).Where(p => GetAttributeInherited<T>(p, false) != null);

        /// <summary>
        /// Determines whether a constructor is both default (no required parameters) and accessible (public).
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns><c>true</c> if the constructor is the default; otherwise, <c>false</c>.</returns>
        public static bool IsDefaultAndAccessible(ConstructorInfo method)
        {
            if (method.IsAssembly || method.IsPrivate)
                return false;

            var parameters = method.GetParameters();

            return parameters.Length == 0 || parameters.All(p => p.IsOptional);
        }

        /// <summary>
        /// Determines whether a specified type should be considered a system type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><c>true</c> if the type should be considered a system type; otherwise, <c>false</c>.</returns>
        public static bool IsSystem(Type type)
        {
            var m = type.Module.Name;

            if (m == "System" || m == "mscorlib" || m == "netstandard" || m == "WindowsBase" || m == "testhost")
                return true;

            if (m.StartsWith("System.") || m.StartsWith("Microsoft.") || m.StartsWith("Windows."))
                return true;

            if (type.Namespace == null || type.Namespace == "System" || type.Namespace.StartsWith("System."))
                return true;

            return false;
        }
    }
}
