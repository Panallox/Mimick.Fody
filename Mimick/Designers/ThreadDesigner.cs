using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Mimick.Designers
{
    /// <summary>
    /// A component designer class which creates and maintains single instances of components on a per-thread basis.
    /// </summary>
    sealed class ThreadDesigner : IComponentDesigner
    {
        private readonly Func<object> constructor;
        private readonly string name;

        private volatile bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadDesigner"/> class.
        /// </summary>
        /// <param name="ctor">The ctor.</param>
        public ThreadDesigner(Func<object> ctor)
        {
            constructor = ctor;
            name = Guid.NewGuid().ToString();
        }

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
                lock (constructor)
                {
                    var slot = Thread.GetNamedDataSlot(name.ToString());

                    if (slot == null)
                        return;

                    var instance = Thread.GetData(slot);

                    if (!disposed && instance != null && instance is IDisposable disposable)
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
        public object GetComponent()
        {
            var slot = Thread.GetNamedDataSlot(name);

            if (slot == null)
                slot = Thread.AllocateNamedDataSlot(name);

            var instance = Thread.GetData(slot);

            if (instance == null)
            {
                instance = constructor();
                Thread.SetData(slot, instance);
            }

            return instance;
        }
    }
}
