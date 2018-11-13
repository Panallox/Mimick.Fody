using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated declaring class should implement the <see cref="IDisposable"/> interface and automatically
    /// call the associated method upon disposal.
    /// </summary>
    /// <remarks>
    /// The attribute can be applied to more than one method, and each will execute in no specific order.
    /// </remarks>
    [CompilationImplements(Interface = typeof(IDisposableAndTracked))]
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class DisposableAttribute : Attribute, IDisposableAndTracked, IInstanceAware
    {
        #region Properties

        /// <summary>
        /// Gets or sets the object instance which the attribute is associated with.
        /// </summary>
        public object Instance { get; set; }

        /// <summary>
        /// Gets or sets whether the object has been disposed.
        /// </summary>
        public bool IsDisposed { get; set; }

        #endregion

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">
        ///   <c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        public void Dispose(bool disposing)
        {
            if (IsDisposed)
                return;

            var methods = Instance.GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(m => m.GetCustomAttribute<DisposableAttribute>() != null);

            foreach (var method in methods)
                method.Invoke(Instance, null);

            IsDisposed = true;
        }
    }

    /// <summary>
    /// An interface representing the dispose state of an object.
    /// </summary>
    public interface IDisposableAndTracked : IDisposable
    {
        /// <summary>
        /// Gets or sets whether the object has been disposed.
        /// </summary>
        bool IsDisposed { get; set; }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        void Dispose(bool disposing);
    }
}
