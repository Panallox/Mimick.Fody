using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Framework
{
    /// <summary>
    /// A class representing a component which is registered when picked up from a configuration provider.
    /// </summary>
    public class ConfiguredComponent
    {
    }

    /// <summary>
    /// A configuration class which configures components.
    /// </summary>
    [Configuration]
    public class ConfiguredComponentConfiguration
    {
        /// <summary>
        /// Gets a configured component.
        /// </summary>
        /// <returns>The configured component.</returns>
        [Component]
        public ConfiguredComponent GetConfiguredComponent() => new ConfiguredComponent();
    }
}
