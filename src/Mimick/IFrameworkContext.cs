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
    public interface IFrameworkContext
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

        #endregion

        /// <summary>
        /// Initialize the framework context in preparation for usage. This method must be called before the framework can be used.
        /// </summary>
        void Initialize();
    }
}
