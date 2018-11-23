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
    static class ReflectionHelper
    {
        /// <summary>
        /// Binding flags used to find members that are both instanced or static, and public and non-public.
        /// </summary>
        public const BindingFlags All = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;

        /// <summary>
        /// Binding flags used to find members that are instanced and public or non-public.
        /// </summary>
        public const BindingFlags AllInstanced = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
    }
}
