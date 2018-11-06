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
    /// Indicates that the associated class should implement the <see cref="INotifyPropertyChanged"/> interface and
    /// automatically introduce the behaviour to all properties.
    /// </summary>
    [CompilationImplements(Interface = typeof(INotifyPropertyChanged))]
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public sealed class PropertyChangedAttribute : Attribute, INotifyPropertyChanged, IPropertySetInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyChangedAttribute" /> class.
        /// </summary>
        public PropertyChangedAttribute()
        {

        }

        #region Events

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

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
        public void OnExit(PropertyInterceptionArgs e)
        {
            var ignored = e.Property.GetCustomAttribute<IgnoreChangeAttribute>();

            if (ignored == null)
                PropertyChanged?.Invoke(e.Instance, new PropertyChangedEventArgs(e.Property.Name));
        }

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// The value of the <see cref="PropertyInterceptionArgs.Value" /> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        public void OnSet(PropertyInterceptionArgs e) { }
    }

    /// <summary>
    /// Indicates that the associated property should not generate a <see cref="INotifyPropertyChanged.PropertyChanged"/> event.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class IgnoreChangeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IgnoreChangeAttribute" /> class.
        /// </summary>
        public IgnoreChangeAttribute()
        {

        }
    }
}
