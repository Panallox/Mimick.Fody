using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a descriptor of a configuration source which has been registered in the framework. The descriptor contains
    /// a reference to the configuration source, and contains information which customises the behaviour of the configurations.
    /// </summary>
    public interface IConfigurationDescriptor
    {
        #region Properties

        /// <summary>
        /// Gets the duration of time, from when the configuration source is loaded, to maintain the configuration source before issuing
        /// a <see cref="IConfigurationSource.Refresh"/> invocation.
        /// </summary>
        TimeSpan ExpiresAt
        {
            get;
        }

        /// <summary>
        /// Gets the configuration source.
        /// </summary>
        IConfigurationSource Source
        {
            get;
        }

        #endregion
    }
}
