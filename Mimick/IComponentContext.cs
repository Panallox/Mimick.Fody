using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a provider which registers and resolves components in the framework.
    /// </summary>
    /// <remarks>
    /// The Mimick framework provides a default component provider which is thread-safe and configured for
    /// many reads, with support for swapping components. If an implementing application already uses a component
    /// framework then a custom provider implementation can leverage the existing system.
    /// </remarks>
    public interface IComponentContext : IDisposable
    {
        /// <summary>
        /// Registers all classes within the provided assembly which have been decorated with <see cref="ComponentAttribute"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        void RegisterAssembly(Assembly assembly);

        /// <summary>
        /// Registers all classes within an assembly containing the provided type, which have been decorated with <see cref="ComponentAttribute"/>.
        /// </summary>
        /// <typeparam name="T">The type of the target assembly.</typeparam>
        void RegisterAssembly<T>();

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The type.</typeparam>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register<TConcrete>() where TConcrete : class;

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register<TConcrete>(params string[] names) where TConcrete : class;

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register<TInterface, TConcrete>() where TConcrete : class, TInterface;

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register<TInterface, TConcrete>(params string[] names) where TConcrete : class, TInterface;

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register(Type type);

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register(Type type, params string[] names);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register(Type interfaceType, Type concreteType);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>A configurator which can be used to further configure the component state.</returns>
        IComponentRegistration Register(Type interfaceType, Type concreteType, params string[] names);

        /// <summary>
        /// Register a provided object instance within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        void Register(object instance);

        /// <summary>
        /// Register a provided object instance within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="instance">The object instance.</param>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        void Register(object instance, params string[] names);

        /// <summary>
        /// Resolve a component of the provided type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>The resolved component instance.</returns>
        /// <exception cref="ArgumentException">If the component cannot be resolved.</exception>
        T Resolve<T>() where T : class;

        /// <summary>
        /// Resolve a component of the provided type with the provided name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The component name.</param>
        /// <returns>The resolved component instance.</returns>
        /// <exception cref="ArgumentException">If the component cannot be resolved.</exception>
        T Resolve<T>(string name) where T : class;

        /// <summary>
        /// Resolve a component with the provided name.
        /// </summary>
        /// <param name="name">The component name.</param>
        /// <returns>The resolved component instance.</returns>
        /// <exception cref="ArgumentException">If the component cannot be resolved.</exception>
        object Resolve(string name);
        
        /// <summary>
        /// Resolve a component of the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>The resolve component instance.</returns>
        /// <exception cref="ArgumentException">If the component cannot be resolved.</exception>
        object Resolve(Type type);

        /// <summary>
        /// Resolve a component of the provided type with the provided name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The component name.</param>
        /// <returns>The resolve component instance.</returns>
        /// <exception cref="ArgumentException">If the component cannot be resolved.</exception>
        object Resolve(Type type, string name);
    }

}
