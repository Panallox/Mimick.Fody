using Mimick.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// A class containing methods and properties for configuring configuration sources.
    /// </summary>
    public sealed class Configurations
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Configurations" /> class.
        /// </summary>
        internal Configurations() => Sources = new List<IConfigurationSource>();

        #region Properties

        /// <summary>
        /// Gets the configuration source used to read from the application configuration file.
        /// </summary>
        public static IConfigurationSource AppConfig => new AppConfigConfigurationSource();

        /// <summary>
        /// Gets the collection of configuration sources.
        /// </summary>
        internal List<IConfigurationSource> Sources
        {
            get;
        }

        #endregion

        /// <summary>
        /// Adds a configuration source to the configuration.
        /// </summary>
        /// <param name="source">The configuration source.</param>
        /// <returns></returns>
        public Configurations Add(IConfigurationSource source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            Sources.AddIfMissing(source);
            return this;
        }
    }
}
