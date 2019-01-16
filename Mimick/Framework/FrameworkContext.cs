using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Framework;

namespace Mimick
{
    /// <summary>
    /// A class representing the current framework context of the application, implemented as a singleton pattern.
    /// </summary>
    public sealed class FrameworkContext : IFrameworkContext
    {
        /// <summary>
        /// The singleton implementation which maintains the current framework context instance.
        /// </summary>
        private static readonly Lazy<FrameworkContext> current = new Lazy<FrameworkContext>(() => new FrameworkContext());

        private IComponentContext componentContext;
        private ConfigurationContext configurationContext;
        private volatile bool disposed;
        private volatile bool initialized;
        private TaskContext taskContext;

        /// <summary>
        /// Prevents a default instance of the <see cref="FrameworkContext" /> class from being created.
        /// </summary>
        private FrameworkContext()
        {
            componentContext = new ComponentContext();
            configurationContext = new ConfigurationContext();
            initialized = false;
            taskContext = new TaskContext();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="FrameworkContext"/> class.
        /// </summary>
        ~FrameworkContext() => Dispose(false);

        #region Properties

        /// <summary>
        /// Gets the current framework context associated with the application.
        /// </summary>
        public static IFrameworkContext Current => current.Value;

        /// <summary>
        /// Gets the component context responsible for maintaining and resolving components.
        /// </summary>
        public IComponentContext ComponentContext => componentContext;

        /// <summary>
        /// Gets the configuration context responsible for maintaining and resolving configurations.
        /// </summary>
        public IConfigurationContext ConfigurationContext => configurationContext;

        /// <summary>
        /// Gets the task context responsible for maintaining timed and asynchronous tasks.
        /// </summary>
        public ITaskContext TaskContext => taskContext;

        #endregion

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
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                taskContext.Dispose();
                componentContext.Dispose();
                configurationContext.Dispose();

                disposed = true;
            }
        }

        /// <summary>
        /// Initialize the framework context in preparation for usage. This method must be called before the framework can be used.
        /// </summary>
        public void Initialize()
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            if (initialized)
                throw new InvalidOperationException("Cannot initialize the framework more than once");

            initialized = true;

            componentContext.Register<IComponentContext>(componentContext);
            componentContext.Register<IConfigurationContext>(configurationContext);
            componentContext.Register<IFrameworkContext>(this);
            componentContext.Register<ITaskContext>(taskContext);

            configurationContext.Initialize();
            componentContext.Initialize();
            taskContext.Initialize();
        }

        /// <summary>
        /// Update the framework context with a new component context manager, allowing for custom dependency container implementations.
        /// </summary>
        /// <param name="context">The component context.</param>
        /// <exception cref="ObjectDisposedException">this</exception>
        /// <exception cref="InvalidOperationException">Cannot change component context once the framework is initialized</exception>
        public void SetComponentContext(IComponentContext context)
        {
            if (disposed)
                throw new ObjectDisposedException("this");

            if (initialized)
                throw new InvalidOperationException("Cannot change component context once the framework is initialized");

            componentContext = context;
        }
    }
}
