using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated class should implement the <see cref="IFreezable"/> interface to prevent external changes to
    /// properties of the object instance once frozen.
    /// </summary>
    /// <remarks>
    /// This attribute implements the <see cref="IFreezable"/> interface and places protection on all property setters to prevent
    /// modification once the object has been frozen. This attribute does not prevent modification to fields within the instance,
    /// but does prevent external changes.
    /// </remarks>
    [CompilationImplements(Interface = typeof(IFreezable))]
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class FreezableAttribute : Attribute, IFreezable, IPropertySetInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FreezableAttribute" /> class.
        /// </summary>
        public FreezableAttribute()
        {

        }

        #region Properties

        /// <summary>
        /// Gets whether the object instance has been frozen.
        /// </summary>
        public bool IsFrozen
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Freezes the object instance and prevents further modifications to the fields and properties of the instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">The object has already been frozen</exception>
        public void Freeze()
        {
            if (IsFrozen)
                throw new InvalidOperationException("The object has already been frozen");

            IsFrozen = true;
        }

        /// <summary>
        /// Called when a property <c>set</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(PropertyInterceptionArgs e, Exception ex) => throw ex;

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(PropertyInterceptionArgs e) { }

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <exception cref="InvalidOperationException">The object has been frozen and cannot receive updates</exception>
        /// <remarks>
        /// The value of the <see cref="PropertyInterceptionArgs.Value" /> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        public void OnSet(PropertyInterceptionArgs e)
        {
            if (IsFrozen)
                throw new InvalidOperationException("The object has been frozen and cannot receive updates");
        }
    }
}
