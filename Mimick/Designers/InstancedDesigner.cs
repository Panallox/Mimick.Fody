using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Designers
{
    /// <summary>
    /// A component designer class which holds a reference to an instance of a component.
    /// </summary>
    internal class InstancedDesigner : IComponentDesigner
    {
        private readonly object component;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstancedDesigner"/> class.
        /// </summary>
        /// <param name="instance">The instance.</param>
        public InstancedDesigner(object instance) => component = instance;

        #region Properties

        /// <summary>
        /// Gets whether the component designer can be disposed.
        /// </summary>
        public bool IsDisposable => component is IDisposable;

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() => ((IDisposable)component).Dispose();

        /// <summary>
        /// Gets a component instance from the designer.
        /// </summary>
        /// <returns>
        /// The component instance.
        /// </returns>
        public object GetComponent() => component;
    }
}
