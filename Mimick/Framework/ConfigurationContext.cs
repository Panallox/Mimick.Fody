using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Configurations;

namespace Mimick.Framework
{
    /// <summary>
    /// A class representing the configuration context of the framework which maintains all configuration sources.
    /// </summary>
    sealed class ConfigurationContext : IConfigurationContext
    {
        private readonly IList<ConfigurationDescriptor> sources;

        private volatile bool disposed;
        private volatile bool initialized;

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationContext" /> class.
        /// </summary>
        public ConfigurationContext()
        {
            sources = new List<ConfigurationDescriptor>();
            Register(new ProviderConfigurationSource());
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ConfigurationContext"/> class.
        /// </summary>
        ~ConfigurationContext() => Dispose(false);

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
            if (disposing && !disposed && initialized)
            {
                foreach (var descriptor in sources)
                {
                    descriptor.Source.Dispose();
                    descriptor.LastUpdated = DateTime.Now;
                }

                disposed = true;
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            if (initialized)
                throw new InvalidOperationException("Cannot initialize the configuration context more than once");

            initialized = true;

            foreach (var descriptor in sources)
            {
                descriptor.Source.Load();
                descriptor.LastUpdated = DateTime.Now;
            }
        }

        /// <summary>
        /// Register a configuration source within the configuration context.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns>
        /// A configurator which can be used to further configure the source.
        /// </returns>
        public IConfigurationRegistration Register(IConfigurationSource source)
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            if (source == null)
                throw new ArgumentNullException("source");

            if (initialized)
                throw new InvalidOperationException("Cannot register new configuration sources once the context has been initialized");

            var descriptor = new ConfigurationDescriptor(source);
            var registration = new ConfigurationRegistration(descriptor);

            sources.AddIfMissing(descriptor);

            return registration;
        }

        /// <summary>
        /// Register a configuration source within the configuration context of the provided type. The configuration source must have a default constructor.
        /// </summary>
        /// <typeparam name="T">The type of the configuration source.</typeparam>
        /// <returns>
        /// A configurator which can be used to further configure the source.
        /// </returns>
        public IConfigurationRegistration Register<T>() where T : IConfigurationSource
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            return Register(Activator.CreateInstance<T>());
        }

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>
        /// The configuration value; otherwise, the value of <paramref name="or" />.
        /// </returns>
        public string Resolve(string name, string or = null)
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            if (name == null)
                throw new ArgumentNullException("name");

            foreach (var descriptor in sources)
            {
                if (descriptor.IsUpdateRequired())
                {
                    descriptor.Source.Refresh();
                    descriptor.LastUpdated = DateTime.Now;
                }

                if (descriptor.Source.TryResolve(name, out var result))
                    return result;
            }

            return or;
        }

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name,
        /// and attempts to automatically convert the value into the provided type.
        /// </summary>
        /// <param name="name">The configuration name.</param>
        /// <param name="type">The configuration type.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>
        /// The configuration value; otherwise, the value of <paramref name="or" />.
        /// </returns>
        public object Resolve(string name, Type type, object or = null)
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            var value = Resolve(name);

            if (name == null)
                return or;

            return TypeHelper.Convert(value, type);
        }

        /// <summary>
        /// Resolves the value of a configuration from the configuration context with the provided name,
        /// and attempts to automatically convert the value into the provided type.
        /// </summary>
        /// <typeparam name="T">The configuration type.</typeparam>
        /// <param name="name">The configuration name.</param>
        /// <param name="or">The value to return if the configuration could not be resolved.</param>
        /// <returns>
        /// The configuration value; otherwise, the value of <paramref name="or" />.
        /// </returns>
        public T Resolve<T>(string name, T or = default) => (T)Resolve(name, typeof(T), or);
    }
}
