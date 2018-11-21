using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method or property provides a configuration value with the provided name. This method works
    /// on members of types which have been decorated with the <see cref="ConfigurationAttribute"/> decoration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class ProvideAttribute : FrameworkAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProvideAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the configuration which the member provides.</param>
        public ProvideAttribute(string name) => Name = name;

        #region Properties

        /// <summary>
        /// Gets the name of the configuration which the member provides.
        /// </summary>
        public string Name
        {
            get;
        }

        #endregion
    }
}
