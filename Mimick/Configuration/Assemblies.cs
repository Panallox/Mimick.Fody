using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods and properties for configuring candidate assemblies.
    /// </summary>
    public sealed class Assemblies
    {
        private readonly List<Assembly> assemblies;

        /// <summary>
        /// Initializes a new instance of the <see cref="Assemblies" /> class.
        /// </summary>
        internal Assemblies() => assemblies = new List<Assembly>();

        #region Properties

        /// <summary>
        /// Gets the executing assembly.
        /// </summary>
        public static Assembly This
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        #endregion

        /// <summary>
        /// Adds an assembly to the configuration, which will be recognised as a configuration and dependency source.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns></returns>
        public Assemblies Add(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            assemblies.AddIfMissing(assembly);
            return this;
        }

        /// <summary>
        /// Adds a collection of assemblies to the configuration, which will be recognised as a configuration and dependency sources.
        /// </summary>
        /// <param name="assemblies">The assemblies.</param>
        /// <returns></returns>
        public Assemblies Add(params Assembly[] assemblies)
        {
            foreach (var assembly in assemblies)
                Add(assembly);

            return this;
        }

        /// <summary>
        /// Gets a collection of assemblies where the name matches a provided pattern. The pattern
        /// supports using the <c>*</c> symbol.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        /// <returns>A collection of <see cref="Assembly"/> values.</returns>
        public static Assembly[] Find(string pattern)
        {
            var regex = new Regex(string.Join(".*", pattern.Split('*').Select(p => Regex.Escape(p))));
            return AppDomain.CurrentDomain.GetAssemblies().Where(a => regex.IsMatch(a.GetName().Name)).ToArray();
        }

        /// <summary>
        /// Gets the assembly for a provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The <see cref="Assembly"/>.</returns>
        public static Assembly Of(Type type) => type.Assembly;

        /// <summary>
        /// Gets the assembly for a provided type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The <see cref="Assembly"/>.</returns>
        public static Assembly Of<T>() => Of(typeof(T));
    }
}
