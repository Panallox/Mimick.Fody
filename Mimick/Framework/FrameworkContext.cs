using Mimick.Configuration;
using Mimick.Core;
using Mimick.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class representing the 
    /// </summary>
    public sealed class FrameworkContext : IFrameworkContext
    {
        /// <summary>
        /// The singleton instance of the framework context.
        /// </summary>
        private static IFrameworkContext instance = null;

        /// <summary>
        /// The synchronization root used when creating the context.
        /// </summary>
        private static readonly object syncRoot = new object();

        /// <summary>
        /// Initializes the <see cref="FrameworkContext" /> class.
        /// </summary>
        static FrameworkContext()
        {
            TypeDescriptor.AddAttributes(typeof(Guid), new TypeConverterAttribute(typeof(GuidTypeConverter)));
        }

        private FrameworkConfiguration configuration;

        /// <summary>
        /// Prevents a default instance of the <see cref="FrameworkContext" /> class from being created.
        /// </summary>
        FrameworkContext(FrameworkConfiguration configurator)
        {
            configuration = configurator;

            InitializeConfigurations();
            InitializeDependencies();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FrameworkContext"/> class.
        /// </summary>
        ~FrameworkContext()
        {
            Dispose(false);
        }

        #region Properties

        /// <summary>
        /// Gets the application instance of the framework context.
        /// </summary>
        public static IFrameworkContext Instance
        {
            get
            {
                lock (syncRoot)
                {
                    return instance ?? throw new NotSupportedException($"Cannot access the framework until configuration has been completed");
                }
            }
        }

        /// <summary>
        /// Gets the configuration context managing the application configuration values.
        /// </summary>
        public IConfigurationContext Configurations
        {
            get; private set;
        }
        
        /// <summary>
        /// Gets the dependency context managing the dependency instances.
        /// </summary>
        public IDependencyContext Dependencies
        {
            get; private set;
        }

        #endregion

        /// <summary>
        /// Initialize and configure the framework using the provided configuration settings.
        /// </summary>
        /// <param name="configuration">The configuration settings.</param>
        public static void Configure(FrameworkConfiguration configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            if (instance == null)
            {
                lock (syncRoot)
                {
                    if (instance == null)
                        instance = new FrameworkContext(configuration);
                }
            }
        }
        
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
                Dependencies.Dispose();
        }

        /// <summary>
        /// Initializes the configurations of the framework.
        /// </summary>
        private void InitializeConfigurations()
        {
            var context = new ConfigurationContext();

            foreach (var source in configuration.ConfigurationsConfig.Sources)
            {
                source.Initialize();
                context.Add(source);
            }

            Configurations = context;
        }

        /// <summary>
        /// Initializes the dependencies of the framework.
        /// </summary>
        private void InitializeDependencies()
        {
            var assemblies = configuration.AssembliesConfig.All;
            var types = assemblies.SelectMany(a => a.GetTypes()).Where(a => a.GetAttributeInherited<FrameworkAttribute>(false) != null);

            Dependencies = new DependencyContext();

            foreach (var type in types)
            {
                var component = type.GetAttributeInherited<ComponentAttribute>();

                if (component != null)
                {
                    var configurator = Dependencies.Register(type);

                    if (component.Scope == Scope.Adhoc)
                        configurator.Adhoc();
                }
            }
        }
    }
}
