using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// Indicates that the associated parameter or property should be validated. When applied to a member, all parameters
    /// are validated using the attribute.
    /// </summary>
    [CompilationOptions(Scope = AttributeScope.MultiInstanced)]
    [DebuggerStepThrough]
    public abstract class ValidationAttribute : Attribute, IInstanceAware, IParameterInterceptor, IPropertySetInterceptor
    {
        #region Properties

        /// <summary>
        /// Gets or sets the object instance which the attribute is associated with.
        /// </summary>
        public object Instance
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body and method interceptors.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public virtual void OnEnter(ParameterInterceptionArgs e) => Validate(e.Parameter.Name, e.Parameter.ParameterType, e.Value);

        /// <summary>
        /// Called when a property <c>set</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public virtual void OnException(PropertyInterceptionArgs e, Exception ex) => throw ex;

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public virtual void OnExit(PropertyInterceptionArgs e) { }

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// The value of the <see cref="PropertyInterceptionArgs.Value" /> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        public virtual void OnSet(PropertyInterceptionArgs e) => Validate(e.Property.Name, e.Property.PropertyType, e.Value);

        /// <summary>
        /// Validate the value of the parameter or property.
        /// </summary>
        /// <param name="name">The parameter or property name.</param>
        /// <param name="type">The parameter or property type.</param>
        /// <param name="value">The value.</param>
        public abstract void Validate(string name, Type type, object value);
    }
}
