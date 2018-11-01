using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated class should be loaded, constructed and processed as a configuration source when
    /// the Mimick framework is initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConfigurationAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigurationAttribute" /> class.
        /// </summary>
        public ConfigurationAttribute()
        {

        }
    }
}
