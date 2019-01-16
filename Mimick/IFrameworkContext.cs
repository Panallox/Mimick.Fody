using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing the current instance of the framework context active within the application.
    /// </summary>
    public interface IFrameworkContext : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets the component context responsible for maintaining and resolving components.
        /// </summary>
        IComponentContext ComponentContext
        {
            get;
        }

        /// <summary>
        /// Gets the configuration context responsible for maintaining and resolving configurations.
        /// </summary>
        IConfigurationContext ConfigurationContext
        {
            get;
        }

        /// <summary>
        /// Gets the task context responsible for maintaining timed and asynchronous tasks.
        /// </summary>
        ITaskContext TaskContext
        {
            get;
        }

        #endregion

        /// <summary>
        /// Initialize the framework context in preparation for usage. This method must be called before the framework can be used.
        /// </summary>
        /// <exception cref="InvalidOperationException">If the framework context has been initialized previously.</exception>
        /// <exception cref="ObjectDisposedException">If the framework context has been disposed.</exception>
        void Initialize();

        /// <summary>
        /// Update the framework context with a new component context manager, allowing for custom dependency container implementations.
        /// </summary>
        /// <param name="context">The component context.</param>
        /// <exception cref="ArgumentNullException">If the component context is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">If the framework context has been initialized.</exception>
        /// <exception cref="ObjectDisposedException">If the framework context has been disposed.</exception>
        void SetComponentContext(IComponentContext context);
    }
}
