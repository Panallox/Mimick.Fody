using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated field, property, or parameter should be populated from a value matching the provided descriptor.
    /// </summary>
    [CompilationOptions(Scope = AttributeScope.Instanced)]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Parameter | AttributeTargets.Property)]
    public sealed class ValueAttribute : Attribute, IParameterInterceptor, IPropertyGetInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValueAttribute" /> class.
        /// </summary>
        /// <param name="pattern">The pattern.</param>
        public ValueAttribute(string pattern) => Pattern = pattern;

        #region Properties

        /// <summary>
        /// Gets the pattern of the value.
        /// </summary>
        public string Pattern
        {
            get;
        }

        #endregion

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body and method interceptors.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnEnter(ParameterInterceptionArgs e)
        {
            var type = e.Parameter.ParameterType;

            if (e.Value != null && !type.IsValueType)
                return;

            e.Value = Resolve(type);
        }

        /// <summary>
        /// Called when a property <c>get</c> method is invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(PropertyInterceptionArgs e, Exception ex)
        {

        }

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
            var type = e.Property.PropertyType;

            if (e.Value != null && !type.IsValueType)
                return;

            e.Value = Resolve(type);
        }

        /// <summary>
        /// Resolves the value of the field, property or parameter.
        /// </summary>
        /// <param name="type">The type of the storage.</param>
        /// <returns></returns>
        private object Resolve(Type type)
        {
            var orDefault = type.IsValueType && type != typeof(string) ? Activator.CreateInstance(type) : null;
            var context = FrameworkContext.Instance;
            return context.Configurations.Get(Pattern, type, orDefault);
        }
    }
}
