using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a provider which registers and resolves dependencies in the framework.
    /// </summary>
    /// <remarks>
    /// The Mimick framework provides a default dependency provider which is thread-safe and configured for
    /// many reads, with support for swapping dependencies. If an implementing application already uses a dependency
    /// framework then a custom provider implementation can leverage the existing system.
    /// </remarks>
    public interface IDependencyContext : IDisposable
    {
        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The type.</typeparam>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register<TConcrete>() where TConcrete : class;

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register<TConcrete>(params string[] names) where TConcrete : class;

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register<TInterface, TConcrete>() where TConcrete : class, TInterface;

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register<TInterface, TConcrete>(params string[] names) where TConcrete : class, TInterface;

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register(Type type);

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register(Type type, params string[] names);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register(Type interfaceType, Type concreteType);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the dependency state.</returns>
        IDependencyConfigurator Register(Type interfaceType, Type concreteType, params string[] names);

        /// <summary>
        /// Resolve a dependency of the provided type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The resolved dependency instance.</returns>
        /// <exception cref="ArgumentException">If the dependency cannot be resolved.</exception>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve a dependency of the provided type with the provided name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The dependency name.</param>
        /// <returns>The resolved dependency instance.</returns>
        /// <exception cref="ArgumentException">If the dependency cannot be resolved.</exception>
        T Resolve<T>(string name) where T : class;

        /// <summary>
        /// Resolve a dependency with the provided name.
        /// </summary>
        /// <param name="name">The dependency name.</param>
        /// <returns>The resolved dependency instance.</returns>
        /// <exception cref="ArgumentException">If the dependency cannot be resolved.</exception>
        object Resolve(string name);
        
        /// <summary>
        /// Resolve a dependency of the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The resolve dependency instance.</returns>
        /// <exception cref="ArgumentException">If the dependency cannot be resolved.</exception>
        object Resolve(Type type);

        /// <summary>
        /// Resolve a dependency of the provided type with the provided name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The dependency name.</param>
        /// <returns>The resolve dependency instance.</returns>
        /// <exception cref="ArgumentException">If the dependency cannot be resolved.</exception>
        object Resolve(Type type, string name);
    }

}
