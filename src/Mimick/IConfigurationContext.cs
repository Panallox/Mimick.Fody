using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a configuration provider which resolves configuration values from the configuration sources.
    /// </summary>
    public interface IConfigurationContext
    {
        #region Properties

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <returns>The value of the configuration; otherwise, <c>null</c>.</returns>
        string this[string name]
        {
            get;
        }

        #endregion

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>The value of the configuration; otherwise, the <paramref name="orDefault"/> value.</returns>
        string Get(string name, string orDefault = null);

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <param name="name">The property name.</param>
        /// <param name="type">The property type.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>The value of the configuration; otherwise, the <paramref name="orDefault"/> value.</returns>
        /// <exception cref="InvalidCastException">If the property value cannot be converted to the expected type.</exception>
        object Get(string name, Type type, object orDefault = null);

        /// <summary>
        /// Gets a configuration value with the provided property name.
        /// </summary>
        /// <typeparam name="T">The property type.</typeparam>
        /// <param name="name">The property name.</param>
        /// <param name="orDefault">The optional value to produce if the configuration could not be found.</param>
        /// <returns>The value of the configuration; otherwise, the <paramref name="orDefault"/> value.</returns>
        /// <exception cref="InvalidCastException">If the property value cannot be converted to the expected type.</exception>
        T Get<T>(string name, T orDefault = default(T));

        /// <summary>
        /// Refreshes all configuration sources of the context, resulting in all internal caches being reloaded.
        /// </summary>
        void Refresh();
    }
}
