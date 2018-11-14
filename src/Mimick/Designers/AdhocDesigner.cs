using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Designers
{
    /// <summary>
    /// A component designer class which instantiates new instances of a component for each resolution.
    /// </summary>
    internal class AdhocDesigner : IComponentDesigner
    {
        private readonly Func<object> constructor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdhocDesigner"/> class.
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        public AdhocDesigner(Func<object> ctor) => constructor = ctor;

        #region Properties

        /// <summary>
        /// Gets whether the component designer can be disposed.
        /// </summary>
        public bool IsDisposable => false;

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Gets a component instance from the designer.
        /// </summary>
        /// <returns>
        /// The component instance.
        /// </returns>
        public object GetComponent() => constructor();
    }
}
