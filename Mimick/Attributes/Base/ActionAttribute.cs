using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// Indicates that the associated parameter, property or method return value should have an action applied to it. When applied to
    /// a method, all parameters are actioned using the attribute.
    /// </summary>
    [CompilationOptions(CopyArguments = true, Scope = AttributeScope.Singleton)]
    [DebuggerStepThrough]
    public abstract class ActionAttribute : Attribute, IParameterInterceptor, IPropertySetInterceptor, IMethodReturnInterceptor
    {
        /// <summary>
        /// Applies the action to the parameter, property or return value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="type">The value type.</param>
        /// <returns>The actioned value.</returns>
        protected abstract object Apply(object value, Type type);

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body and method interceptors.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public virtual void OnEnter(ParameterInterceptionArgs e) => e.Value = Apply(e.Value, e.Parameter.ParameterType);

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
        /// Called when a method is invoked and is returning.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public virtual void OnReturn(MethodReturnInterceptionArgs e)
        {
            if (e.Method is MethodInfo method)
                e.Value = Apply(e.Value, method.ReturnType);
        }

        /// <summary>
        /// Called when a property <c>set</c> method is intercepted and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <remarks>
        /// The value of the <see cref="P:Mimick.Aspect.PropertyInterceptionArgs.Value" /> property will be populated with the
        /// updated value which has been assigned during the set operation.
        /// </remarks>
        public virtual void OnSet(PropertyInterceptionArgs e) => e.Value = Apply(e.Value, e.Property.PropertyType);
    }
}
