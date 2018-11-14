using Mimick.Designers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing a default implementation of the component context.
    /// </summary>
    sealed class ComponentContext : IComponentContext
    {
        /// <summary>
        /// The entries across all implementations.
        /// </summary>
        private readonly IList<ComponentDescriptor> allEntries;

        /// <summary>
        /// The entries where one concrete type implements an interface type.
        /// </summary>
        private readonly IDictionary<Type, ComponentDescriptor> implementedEntries;

        /// <summary>
        /// The entries where a concrete type has been provided one or more names.
        /// </summary>
        private readonly IDictionary<string, ComponentDescriptor> namedEntries;

        /// <summary>
        /// The entries where a concrete type is mapped directly to the component.
        /// </summary>
        private readonly IDictionary<Type, ComponentDescriptor> typedEntries;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContext" /> class.
        /// </summary>
        public ComponentContext()
        {
            allEntries = new ReadWriteList<ComponentDescriptor>();
            implementedEntries = new ReadWriteDictionary<Type, ComponentDescriptor>();
            namedEntries = new ReadWriteDictionary<string, ComponentDescriptor>();
            typedEntries = new ReadWriteDictionary<Type, ComponentDescriptor>();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ComponentContext"/> class.
        /// </summary>
        ~ComponentContext()
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
        /// Registers all classes within the provided assembly which have been decorated with <see cref="ComponentAttribute" />.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        public void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            var candidates = assembly.GetTypes().Where(a => a.GetAttributeInherited<FrameworkAttribute>() != null);

            foreach (var candidate in candidates)
            {
                var decoration = candidate.GetAttributeInherited<ComponentAttribute>();

                if (decoration != null)
                    Register(candidate, decoration.Name).ToScope(decoration.Scope);
            }
        }

        /// <summary>
        /// Registers all classes within an assembly containing the provided type, which have been decorated with <see cref="ComponentAttribute" />.
        /// </summary>
        /// <typeparam name="T">The type of the target assembly.</typeparam>
        public void RegisterAssembly<T>() => RegisterAssembly(Assembly.GetAssembly(typeof(T)));

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete">The type.</typeparam>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register<TConcrete>() where TConcrete : class => Register(null, typeof(TConcrete), null);

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TConcrete"></typeparam>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register<TConcrete>(params string[] names) where TConcrete : class => Register(null, typeof(TConcrete), names);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register<TInterface, TConcrete>() where TConcrete : class, TInterface => Register(typeof(TInterface), typeof(TConcrete), null);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <typeparam name="TInterface">The interface type.</typeparam>
        /// <typeparam name="TConcrete">The concrete type.</typeparam>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register<TInterface, TConcrete>(params string[] names) where TConcrete : class, TInterface => Register(typeof(TInterface), typeof(TConcrete), names);

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register(Type type) => Register(null, type, null);

        /// <summary>
        /// Register a provided type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register(Type type, params string[] names) => Register(null, type, names);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register(Type interfaceType, Type concreteType) => Register(interfaceType, concreteType, null);

        /// <summary>
        /// Register a provided interface and concrete type within the component provider using the default singleton lifetime.
        /// </summary>
        /// <param name="interfaceType">The interface type.</param>
        /// <param name="concreteType">The concrete type.</param>
        /// <param name="names">An optional collection of identifiers which the components will be stored under.</param>
        /// <returns>
        /// A configurator which can be used to further configure the component state.
        /// </returns>
        public IComponentRegistration Register(Type interfaceType, Type concreteType, params string[] names)
        {
            var implements = new List<Type>(GetImplementedTypes(concreteType));
            
            if (names == null || names.Length == 0)
                names = implements.Concat(new[] { interfaceType, concreteType }).Where(t => t != null).Distinct().Select(t => t.Name).ToArray();

            var constructor = CreateConstructor(concreteType);
            var entry = new ComponentDescriptor(concreteType, constructor, interfaceType != null ? new[] { interfaceType } : Type.EmptyTypes, names);

            entry.Designer = new SingletonDesigner(constructor);

            foreach (var implementedType in implements)
            {
                if (implementedEntries.ContainsKey(implementedType))
                    continue;

                implementedEntries.Add(implementedType, entry);
            }

            if (interfaceType != null)
            {
                if (implementedEntries.TryGetValue(interfaceType, out var existing))
                    throw new ArgumentException($"Conflicting '{interfaceType.FullName}' component, adding '{concreteType.FullName}' against '{existing.Type.FullName}'");

                implementedEntries.Add(interfaceType, entry);
            }

            if (!typedEntries.ContainsKey(concreteType))
                typedEntries.Add(concreteType, entry);

            foreach (var name in names)
            {
                if (name == null)
                    continue;

                if (namedEntries.TryGetValue(name, out var existing))
                    throw new ArgumentException($"Conflicting named '{name}' component, adding '{concreteType.FullName}' against '{existing.Type.FullName}'");

                namedEntries.Add(name, entry);
            }

            allEntries.Add(entry);

            return new ComponentRegistration(new[] { entry });
        }

        /// <summary>
        /// Resolve a component of the provided type.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <returns>
        /// The resolved component instance.
        /// </returns>
        public T Resolve<T>() where T : class => Resolve(typeof(T), null) as T;

        /// <summary>
        /// Resolve a component of the provided type with the provided name.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="name">The component name.</param>
        /// <returns>
        /// The resolved component instance.
        /// </returns>
        public T Resolve<T>(string name) where T : class => Resolve(typeof(T), name) as T;

        /// <summary>
        /// Resolve a component with the provided name.
        /// </summary>
        /// <param name="name">The component name.</param>
        /// <returns>
        /// The resolved component instance.
        /// </returns>
        /// <exception cref="ArgumentNullException">name</exception>
        /// <exception cref="ArgumentException"></exception>
        public object Resolve(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (namedEntries.TryGetValue(name, out var named))
                return named.Designer.GetComponent();

            throw new ArgumentException($"Cannot resolve component with name '{name}'");
        }

        /// <summary>
        /// Resolve a component of the provided type.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        /// The resolve component instance.
        /// </returns>
        public object Resolve(Type type) => Resolve(type, null);

        /// <summary>
        /// Resolve a component of the provided type with the provided name.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="name">The component name.</param>
        /// <returns>
        /// The resolve component instance.
        /// </returns>
        public object Resolve(Type type, string name)
        {
            if (name != null && namedEntries.TryGetValue(name, out var named))
                return named.Designer.GetComponent();

            if (type.IsInterface && implementedEntries.TryGetValue(type, out var implemented))
                return implemented.Designer.GetComponent();

            if (typedEntries.TryGetValue(type, out var typed))
                return typed.Designer.GetComponent();

            if (implementedEntries.TryGetValue(type, out var optimisticImplemented))
                return optimisticImplemented.Designer.GetComponent();

            throw new ArgumentException($"Cannot resolve component for type '{type.FullName}'");
        }
    }
}
