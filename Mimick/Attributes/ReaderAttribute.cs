using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method or property should be invoked concurrently, requiring a read lock when the method is entered. When applied
    /// to a property, the <c>get</c> method of the property is encapsulated with the read lock.
    /// </summary>
    [CompilationImplements(Interface = typeof(IRequireSynchronization))]
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property)]
    public sealed class ReaderAttribute : Attribute, IInstanceAware, IRequireInitialization, IRequireSynchronization, IMethodInterceptor, IPropertyGetInterceptor
    {
        #region Properties

        /// <summary>
        ///   Gets or sets the synchronization context used for concurrent read and write locks.
        /// </summary>
        ReaderWriterLockSlim IRequireSynchronization.SynchronizationContext
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the object instance which the attribute is associated with.
        /// </summary>
        object IInstanceAware.Instance
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Gets the lock mechanism used when synchronizing.
        /// </summary>
        /// <returns>A <see cref="ReaderWriterLockSlim"/> value.</returns>
        private ReaderWriterLockSlim GetSynchronizationLock() => ((IRequireSynchronization)((IInstanceAware)this).Instance).SynchronizationContext;

        /// <summary>
        /// Initialize the attribute.
        /// </summary>
        public void Initialize()
        {
            var context = (IRequireSynchronization)this;

            if (context.SynchronizationContext == null)
                context.SynchronizationContext = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
        }

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnEnter(MethodInterceptionArgs e) => GetSynchronizationLock().EnterReadLock();

        /// <summary>
        /// Called when a method has been invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(MethodInterceptionArgs e, Exception ex) => throw ex;

        /// <summary>
        /// Called when a property <c>get</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(PropertyInterceptionArgs e, Exception ex) => throw ex;

        /// <summary>
        /// Called when a method has been invoked, and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(MethodInterceptionArgs e) => GetSynchronizationLock().ExitReadLock();

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(PropertyInterceptionArgs e) => GetSynchronizationLock().ExitReadLock();

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// If the property has a generated backing field, the value of the field will be loaded into the
        /// <see cref="P:Mimick.Aspect.PropertyInterceptionArgs.Value" /> property. If the value of this property is changed
        /// during the interception, the updated value will be copied into the backing field.
        /// </remarks>
        public void OnGet(PropertyInterceptionArgs e) => GetSynchronizationLock().EnterReadLock();
    }
}
