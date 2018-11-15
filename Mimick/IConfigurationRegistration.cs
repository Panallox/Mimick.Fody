using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface containing options to configure the registration of a configuration within the framework.
    /// </summary>
    public interface IConfigurationRegistration
    {
        /// <summary>
        /// Sets the duration of time after which the configuration source must be reloaded into memory using the
        /// <see cref="IConfigurationSource.Refresh"/> method. The expiration is set to <see cref="Timeout.InfiniteTimeSpan"/> by default.
        /// </summary>
        /// <param name="duration">The duration of time.</param>
        /// <returns></returns>
        IConfigurationRegistration Expires(TimeSpan duration);
    }
}
