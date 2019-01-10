using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing the configurations context of the framework, which is the central source for resolving configuration values.
    /// </summary>
    public interface IConfigurationContext : IDisposable
    {
        /// <summary>
        /// Register a configuration source within the configuration context.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>A configurator which can be used to further configure the source.</returns>
        IConfigurationRegistration Register(IConfigurationSource source);

        /// <summary>
        /// Register a configuration source within the configuration context of the provided type. The configuration source must have a default constructor.
        /// </summary>
        /// <typeparam name="T">The type of the configuration source.</typeparam>
        /// <returns>A configurator which can be used to further configure the source.</returns>
        IConfigurationRegistration Register<T>() where T : IConfigurationSource;

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>The configuration value; otherwise, the value of <paramref name="or"/>.</returns>
        string Resolve(string name, string or = null);

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name,
        /// and attempts to automatically convert the value into the provided type.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="type">The configuration type.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>The configuration value; otherwise, the value of <paramref name="or"/>.</returns>
        /// <exception cref="InvalidCastException">If the configuration value was found but could not be converted.</exception>
        object Resolve(string name, Type type, object or = null);

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name,
        /// and attempts to automatically convert the value into the provided type.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="name">The configuration name.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>The configuration value; otherwise, the value of <paramref name="or"/>.</returns>
        /// <exception cref="InvalidCastException">If the configuration value was found but could not be converted.</exception>
        T Resolve<T>(string name, T or = default(T));
    }
}
