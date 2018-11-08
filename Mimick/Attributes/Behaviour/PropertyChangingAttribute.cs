using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated class should implement the <see cref="INotifyPropertyChanging"/> interface and
    /// automatically introduce the behaviour to all properties.
    /// </summary>
    [CompilationImplements(Interface = typeof(INotifyPropertyChanging))]
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PropertyChangingAttribute : Attribute, INotifyPropertyChanging, IPropertySetInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangingAttribute" /> class.
        /// </summary>
        public PropertyChangingAttribute()
        {

        }

        #region Events

        /// <summary>
        /// Occurs when a property value is changing.
        /// </summary>
        public event PropertyChangingEventHandler PropertyChanging;

        #endregion

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
        /// <remarks>
        /// The value of the <see cref="PropertyInterceptionArgs.Value" /> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        public void OnSet(PropertyInterceptionArgs e)
        {
            var ignored = e.Property.GetCustomAttribute<IgnoreChangingAttribute>();

            if (ignored == null)
                PropertyChanging?.Invoke(e.Instance, new PropertyChangingEventArgs(e.Property.Name));
        }
    }


    /// <summary>
    /// Indicates that the associated property should not generate a <see cref="INotifyPropertyChanging.PropertyChanging"/> event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreChangingAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreChangingAttribute" /> class.
        /// </summary>
        public IgnoreChangingAttribute()
        {

        }
    }
}
