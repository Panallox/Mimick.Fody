using System;
using System.Collections.Generic;
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

        private ComponentContext componentContext;
        private ConfigurationContext configurationContext;
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
        /// Initialize the framework context in preparation for usage. This method must be called before the framework can be used.
        /// </summary>
        public void Initialize()
        {
            if (initialized)
                throw new InvalidProgramException("Cannot initialize the framework more than once");

            initialized = true;

            configurationContext.Initialize();
            componentContext.Initialize();
            taskContext.Initialize();
        }
    }
}
