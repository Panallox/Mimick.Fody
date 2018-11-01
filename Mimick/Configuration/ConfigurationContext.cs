using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Configuration
{
    /// <summary>
    /// A class representing the configuration context, used to resolve configuration properties.
    /// </summary>
    /// <remarks>
    /// <para>The configuration sources within the context are processed in order. Any configuration sources
    /// which must be processed earlier in the chain should be added to the context earlier.</para>
    /// </remarks>
    sealed class ConfigurationContext : IConfigurationContext
    {
        private readonly List<IConfigurationSource> sources;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationContext" /> class.
        /// </summary>
        public ConfigurationContext()
        {
            sources = new List<IConfigurationSource>();
        }

        #region Properties

        /// <summary>
        /// Gets the <see cref="System.String"/> with the specified name.
        /// </summary>
        /// <value>
        /// The <see cref="System.String"/>.
        /// </value>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public string this[string name] => Get(name);

        #endregion

        /// <summary>
        /// Appends a new configuration source to the end of the configuration source pool.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <exception cref="InvalidOperationException">If the configuration source could not be initialized.</exception>
        public void Add(IConfigurationSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            source.Initialize();
            sources.Add(source);
        }

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>
        /// The value of the configuration; otherwise, the <paramref name="orDefault" /> value.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        public string Get(string name, string orDefault = null)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            foreach (var source in sources)
            {
                var result = source.Resolve(name);

                if (result != null)
                    return result;
            }

            return orDefault;
        }

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>
        /// The value of the configuration; otherwise, the <paramref name="orDefault" /> value.
        /// </returns>
        public object Get(string name, Type type, object orDefault = null)
        {
            var match = Get(name);

            if (match == null)
                return orDefault;

            return TypeHelper.Convert(match, type);
        }

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>
        /// The value of the configuration; otherwise, the <paramref name="orDefault" /> value.
        /// </returns>
        public T Get<T>(string name, T orDefault = default(T)) => (T)Get(name, typeof(T), orDefault);
    }
}
