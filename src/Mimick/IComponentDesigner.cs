using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a component designer responsible for managing instances of components within the framework.
    /// </summary>
    public interface IComponentDesigner : IDisposable
    {
        #region Properties

        /// <summary>
        /// Gets whether the component designer can be disposed.
        /// </summary>
        bool IsDisposable
        {
            get;
        }

        #endregion

        /// <summary>
        /// Gets a component instance from the designer.
        /// </summary>
        /// <returns>The component instance.</returns>
        object GetComponent();
    }
}
