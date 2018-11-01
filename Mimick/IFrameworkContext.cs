using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing the framework context for the Mimick framework. A framework context is created once per application,
    /// and persists until the application has been terminated, or the <c>Dispose</c> method is called.
    /// </summary>
    public interface IFrameworkContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the dependency context managing the dependency instances.
        /// </summary>
        IDependencyContext Dependencies
        {
            get;
        }

        #endregion
    }
}
