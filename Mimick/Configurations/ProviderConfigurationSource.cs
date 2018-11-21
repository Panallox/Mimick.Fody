using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Mimick.Framework;

namespace Mimick.Configurations
{
    /// <summary>
    /// A configuration source class which is used internally to retrieve values from configuration classes which have been
    /// decorated with the <see cref="ProvideAttribute"/> decoration.
    /// </summary>
    internal class ProviderConfigurationSource : IConfigurationSource
    {
        private IDictionary<string, Tuple<IComponentDescriptor, ProviderHandler>> providers;

        /// <summary>
        /// Creates a provider handler delegate method for the provided method.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <returns>A provider handler delegate.</returns>
        private ProviderHandler CreateProviderHandler(MethodInfo method)
        {
            if (method.IsGenericMethodDefinition)
                throw new MemberAccessException($"Cannot register a configuration provider with generic arguments for '{method.DeclaringType.FullName}.{method.Name}'");

            foreach (var parameter in method.GetParameters())
            {
                if (!parameter.IsOptional)
                    throw new MemberAccessException($"Cannot register a configuration provider with non-optional parameters for '{method.DeclaringType.FullName}.{method.Name}'");
            }

            var instance = Expression.Parameter(typeof(object), "instance");
            var invocation = Expression.Lambda<ProviderHandler>(Expression.Convert(Expression.Call(Expression.Convert(instance, method.DeclaringType), method), typeof(object)), instance);

            return invocation.Compile();
        }
        
        /// <summary>
        /// Called when the configuration source has been requested and must prepare for resolution.
        /// </summary>
        public void Load()
        {
            var context = FrameworkContext.Current.ComponentContext as ComponentContext;

            providers = new Dictionary<string, Tuple<IComponentDescriptor, ProviderHandler>>();

            foreach (var entry in context.ConfigurationEntries)
            {
                var methods = entry.Type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.GetAttributeInherited<ProvideAttribute>() != null);
                var properties = entry.Type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(p => p.GetAttributeInherited<ProvideAttribute>() != null);

                foreach (var method in methods)
                {
                    var decoration = method.GetAttributeInherited<ProvideAttribute>();

                    if (decoration.Name == null)
                        throw new ArgumentException($"Cannot register a provided value without a name for '{entry.Type.FullName}.{method.Name}");

                    providers.Add(decoration.Name, new Tuple<IComponentDescriptor, ProviderHandler>(entry, CreateProviderHandler(method)));
                }

                foreach (var property in properties)
                {
                    var decoration = property.GetAttributeInherited<ProvideAttribute>();
                    
                    if (decoration.Name == null)
                        throw new ArgumentException($"Cannot register a provided value without a name for '{entry.Type.FullName}.{property.Name}");

                    providers.Add(decoration.Name, new Tuple<IComponentDescriptor, ProviderHandler>(entry, CreateProviderHandler(property.GetGetMethod())));
                }
            }
        }

        /// <summary>
        /// Called when the configuration source must be refreshed and all existing values reloaded into memory.
        /// </summary>
        public void Refresh()
        {
            providers.Clear();
            Load();
        }

        /// <summary>
        /// Resolve the value of a configuration with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <returns>
        /// The configuration value; otherwise, <c>null</c> if the configuration could not be found.
        /// </returns>
        public string Resolve(string name)
        {
            if (providers.TryGetValue(name, out var provider))
            {
                var component = provider.Item1;
                var handler = provider.Item2;

                var instance = component.Designer.GetComponent();
                var value = handler(instance);

                if (value == null)
                    return null;

                return value.ToString();
            }

            return null;
        }

        /// <summary>
        /// Attempt to resolve the value of a configuration with the provided name, and return whether it was resolved successfully.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="value">The configuration value.</param>
        /// <returns>
        ///   <c>true</c> if the configuration is resolved; otherwise, <c>false</c>.
        /// </returns>
        public bool TryResolve(string name, out string value) => (value = Resolve(name)) != null;

        /// <summary>
        /// A delegate method representing a handler which collects a configuration value from a provider.
        /// </summary>
        /// <param name="instance">The configuration class object instance.</param>
        /// <returns>The configuration value.</returns>
        delegate object ProviderHandler(object instance); 
    }
}
