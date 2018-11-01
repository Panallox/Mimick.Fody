using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods and properties for configurating the Mimick framework during application initialization.
    /// </summary>
    public sealed class FrameworkConfiguration
    {
        private readonly Assemblies assemblies;
        private readonly Configurations configurations;

        private bool withAppConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkConfiguration" /> class.
        /// </summary>
        FrameworkConfiguration()
        {
            assemblies = new Assemblies();
            configurations = new Configurations();
            withAppConfig = false;
        }

        #region Properties

        /// <summary>
        /// Gets the configuration of the assemblies.
        /// </summary>
        internal Assemblies AssembliesConfig => assemblies;

        /// <summary>
        /// Gets the configuration of the configurations.
        /// </summary>
        internal Configurations ConfigurationsConfig => configurations;

        #endregion


        /// <summary>
        /// Begins a new framework configuration.
        /// </summary>
        /// <returns>A <see cref="FrameworkConfiguration"/> value.</returns>
        public static FrameworkConfiguration Begin() => new FrameworkConfiguration();

        /// <summary>
        /// Configure the assemblies of the framework.
        /// </summary>
        /// <param name="configuration">The configuration instructions.</param>
        /// <returns></returns>
        public FrameworkConfiguration Assemblies(Action<Assemblies> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            configuration(assemblies);
            return this;
        }

        /// <summary>
        /// Configure the configuration sources of the framework.
        /// </summary>
        /// <param name="configuration">The configuration instructions.</param>
        /// <returns></returns>
        public FrameworkConfiguration Configurations(Action<Configurations> configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException("configuration");

            configuration(configurations);
            return this;
        }
    }
}
