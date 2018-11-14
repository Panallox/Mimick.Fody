using Mimick.Lifetime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing a default implementation of the dependency context.
    /// </summary>
    sealed class DependencyContext : IDependencyContext
    {
        /// <summary>
        /// The entries across all implementations.
        /// </summary>
        private readonly IList<DependencyEntry> allEntries;

        /// <summary>
        /// The entries where one concrete type implements an interface type.
        /// </summary>
        private readonly IDictionary<Type, DependencyEntry> implementedEntries;

        /// <summary>
        /// The entries where a concrete type has been provided one or more names.
        /// </summary>
        private readonly IDictionary<string, DependencyEntry> namedEntries;

        /// <summary>
        /// The entries where a concrete type is mapped directly to the dependency.
        /// </summary>
        private readonly IDictionary<Type, DependencyEntry> typedEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyContext" /> class.
        /// </summary>
        public DependencyContext()
        {
            allEntries = new ReadWriteList<DependencyEntry>();
            implementedEntries = new ReadWriteDictionary<Type, DependencyEntry>();
            namedEntries = new ReadWriteDictionary<string, DependencyEntry>();
            typedEntries = new ReadWriteDictionary<Type, DependencyEntry>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DependencyContext"/> class.
        /// </summary>
        ~DependencyContext()
        {
            Dispose(false);
        }
        
        /// <summary>
        /// Creates a constructor method which can be used to create a new instance of the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A method used to create a new instance.</returns>
        /// <exception cref="MissingMethodException">If a default constructor cannot be found.</exception>
        private Func<object> CreateConstructor(Type type)
        {
            var candidates = type.GetConstructors().Where(c => c.IsDefaultAndAccessible()).ToArray();

            if (candidates.Length != 1)
                throw new MissingMethodException($"Cannot find a non-internal unique constructor for type '{type.FullName}'");

            var constructor = candidates.First();
            var method = new DynamicMethod(string.Empty, typeof(object), null);
            var il = method.GetILGenerator();

            il.Emit(OpCodes.Nop);
            il.Emit(OpCodes.Newobj, constructor);
            il.Emit(OpCodes.Ret);

            return (Func<object>)method.CreateDelegate(typeof(Func<object>));
        }

        /// <summary>
        /// Gets a collection of implemented type definitions for a provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="Type"/> array containing the results.</returns>
        private Type[] GetImplementedTypes(Type type) => type.GetInterfaces().Where(i => !i.IsSystem()).ToArray();

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var e in allEntries)
                    e.Dispose();
            }
        }

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The type.</typeparam>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register<TConcrete>() where TConcrete : class => Register(null, typeof(TConcrete), null);

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete"></typeparam>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register<TConcrete>(params string[] names) where TConcrete : class => Register(null, typeof(TConcrete), names);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register<TInterface, TConcrete>() where TConcrete : class, TInterface => Register(typeof(TInterface), typeof(TConcrete), null);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register<TInterface, TConcrete>(params string[] names) where TConcrete : class, TInterface => Register(typeof(TInterface), typeof(TConcrete), names);

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register(Type type) => Register(null, type, null);

        /// <summary>
        /// Register a provided type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register(Type type, params string[] names) => Register(null, type, names);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register(Type interfaceType, Type concreteType) => Register(interfaceType, concreteType, null);

        /// <summary>
        /// Register a provided interface and concrete type within the dependency provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <param name="names">An optional collection of identifiers which the dependencies will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the dependency state.
        /// </returns>
        public IDependencyConfigurator Register(Type interfaceType, Type concreteType, params string[] names)
        {
            var implements = new List<Type>(GetImplementedTypes(concreteType));
            
            if (names == null || names.Length == 0)
                names = implements.Concat(new[] { interfaceType, concreteType }).Where(t => t != null).Distinct().Select(t => t.Name).ToArray();

            var constructor = CreateConstructor(concreteType);
            var entry = new DependencyEntry(interfaceType, concreteType, constructor, new SingletonLifetime(constructor));

            foreach (var implementedType in implements)
            {
                if (implementedEntries.ContainsKey(implementedType))
                    continue;

                implementedEntries.Add(implementedType, entry);
            }

            if (interfaceType != null)
            {
                if (implementedEntries.TryGetValue(interfaceType, out var existing))
                    throw new ArgumentException($"Conflicting '{interfaceType.FullName}' dependency, adding '{concreteType.FullName}' against '{existing.ConcreteType.FullName}'");

                implementedEntries.Add(interfaceType, entry);
            }

            if (!typedEntries.ContainsKey(concreteType))
                typedEntries.Add(concreteType, entry);

            foreach (var name in names)
            {
                if (name == null)
                    continue;

                if (namedEntries.TryGetValue(name, out var existing))
                    throw new ArgumentException($"Conflicting named '{name}' dependency, adding '{concreteType.FullName}' against '{existing.ConcreteType.FullName}'");

                namedEntries.Add(name, entry);
            }

            allEntries.Add(entry);

            return new DependencyConfigurator(new[] { entry });
        }

        /// <summary>
        /// Resolve a dependency of the provided type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>
        /// The resolved dependency instance.
        /// </returns>
        public T Resolve<T>() where T : class => Resolve(typeof(T), null) as T;

        /// <summary>
        /// Resolve a dependency of the provided type with the provided name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The dependency name.</param>
        /// <returns>
        /// The resolved dependency instance.
        /// </returns>
        public T Resolve<T>(string name) where T : class => Resolve(typeof(T), name) as T;

        /// <summary>
        /// Resolve a dependency with the provided name.
        /// </summary>
        /// <param name="name">The dependency name.</param>
        /// <returns>
        /// The resolved dependency instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        /// <exception cref="ArgumentException"></exception>
        public object Resolve(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (namedEntries.TryGetValue(name, out var named))
                return named.Lifetime.Resolve();

            throw new ArgumentException($"Cannot resolve dependency with name '{name}'");
        }

        /// <summary>
        /// Resolve a dependency of the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The resolve dependency instance.
        /// </returns>
        public object Resolve(Type type) => Resolve(type, null);

        /// <summary>
        /// Resolve a dependency of the provided type with the provided name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The dependency name.</param>
        /// <returns>
        /// The resolve dependency instance.
        /// </returns>
        public object Resolve(Type type, string name)
        {
            if (name != null && namedEntries.TryGetValue(name, out var named))
                return named.Lifetime.Resolve();

            if (type.IsInterface && implementedEntries.TryGetValue(type, out var implemented))
                return implemented.Lifetime.Resolve();

            if (typedEntries.TryGetValue(type, out var typed))
                return typed.Lifetime.Resolve();

            if (implementedEntries.TryGetValue(type, out var optimisticImplemented))
                return optimisticImplemented.Lifetime.Resolve();

            throw new ArgumentException($"Cannot resolve dependency for type '{type.FullName}'");
        }
    }
}
