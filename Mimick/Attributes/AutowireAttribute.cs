using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated field, property or parameter should be injected with a value stored within the
    /// dependency container of the context.
    /// </summary>
    /// <remarks>
    /// When applied at the method level, the injection will occur across all parameters of the method.
    /// </remarks>
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class AutowireAttribute : Attribute, IParameterInterceptor, IPropertyGetInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AutowireAttribute" /> class.
        /// </summary>
        public AutowireAttribute() => Name = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutowireAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the dependency.</param>
        public AutowireAttribute(string name) => Name = name;

        #region Properties

        /// <summary>
        /// Gets the optional name qualifier of the dependency.
        /// </summary>
        public string Name
        {
            get;
        }

        #endregion

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body and method interceptors.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnEnter(ParameterInterceptionArgs e) => e.Value = Resolve(e.Parameter.ParameterType);

        /// <summary>
        /// Called when a property <c>get</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(PropertyInterceptionArgs e, Exception ex) => throw ex;

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(PropertyInterceptionArgs e)
        {

        }

        /// <summary>
        /// Called when a property <c>get</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// If the property has a generated backing field, the value of the field will be loaded into the
        /// <see cref="PropertyInterceptionArgs.Value" /> property. If the value of this property is changed
        /// during the interception, the updated value will be copied into the backing field.
        /// </remarks>
        public void OnGet(PropertyInterceptionArgs e)
        {
            if (e.Value == null || e.Property.PropertyType.IsValueType)
                e.Value = Resolve(e.Property.PropertyType);
        }

        /// <summary>
        /// Resolves the dependency for the provided type, using the optional name qualifier.
        /// </summary>
        /// <param name="type">The dependency type.</param>
        /// <returns></returns>
        private object Resolve(Type type) => FrameworkContext.Current?.ComponentContext?.Resolve(type, Name);
    }
}
