using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Lifetime
{
    /// <summary>
    /// A class representing a dependency which persists once for the duration of an application.
    /// </summary>
    sealed class SingletonLifetime : IDependencyLifetime
    {
        private readonly Lazy<object> instance;

        private volatile bool disposed;

        /// <summary>
        /// Initializes a new instance of the <see cref="SingletonLifetime" /> class.
        /// </summary>
        /// <param name="constructor">The constructor.</param>
        public SingletonLifetime(Func<object> constructor) => instance = new Lazy<object>(constructor);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (!disposed && instance.IsValueCreated && instance.Value is IDisposable disposable)
            {
                lock (instance)
                {
                    if (!disposed)
                    {
                        disposed = true;
                        disposable.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Resolve the dependency using the mechanism configured by the implementation.
        /// </summary>
        /// <returns>
        /// The dependency instance.
        /// </returns>
        public object Resolve() => instance.Value;
    }
}
