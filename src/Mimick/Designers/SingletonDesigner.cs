using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Designers
{
    /// <summary>
    /// A component designer class which creates and maintains a single instance of a component.
    /// </summary>
    sealed class SingletonDesigner : IComponentDesigner
    {
        private readonly Lazy<object> instance;
        private volatile bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonDesigner"/> class.
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        public SingletonDesigner(Func<object> ctor) => instance = new Lazy<object>(ctor);

        #region Properties

        /// <summary>
        /// Gets whether the component designer can be disposed.
        /// </summary>
        public bool IsDisposable => true;

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed)
            {
                lock (instance)
                {
                    if (!disposed && instance.IsValueCreated && instance.Value is IDisposable disposable)
                    {
                        disposed = true;
                        disposable.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Gets a component instance from the designer.
        /// </summary>
        /// <returns>
        /// The component instance.
        /// </returns>
        public object GetComponent() => instance.Value;
    }
}
