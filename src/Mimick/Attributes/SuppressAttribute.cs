using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method should suppress exceptions when raised. The default behaviour is to ignore all exceptions,
    /// however specific exceptions can be ignored by configuring the <see cref="SuppressAttribute.Types"/> property. If a method suppresses
    /// an exception and expects to return a value, the default value of the return type is produced.
    /// </summary>
    [CompilationOptions(Scope = AttributeScope.MultiSingleton)]
    [AttributeUsage(AttributeTargets.Method)]
    [DebuggerStepThrough]
    public sealed class SuppressAttribute : Attribute, IMethodInterceptor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SuppressAttribute" /> class.
        /// </summary>
        public SuppressAttribute()
        {
            Types = Type.EmptyTypes;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the optional collection of exception types.
        /// </summary>
        public Type[] Types
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Called when a method has been invoked, and executes before the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnEnter(MethodInterceptionArgs e) { }

        /// <summary>
        /// Called when a method has been invoked and has produced an unhandled exception.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        /// <param name="ex">The intercepted exception.</param>
        public void OnException(MethodInterceptionArgs e, Exception ex)
        {
            var thrown = ex.GetType();

            if (Types.Length != 0 && !Types.Any(a => a.IsAssignableFrom(thrown)))
                throw ex;

            var type = (e.Method as MethodInfo)?.ReturnType;

            if (type != null && type != typeof(void))
                e.Return = TypeHelper.Default(type);
        }

        /// <summary>
        /// Called when a method has been invoked, and executes after the method body.
        /// </summary>
        /// <param name="e">The interception event arguments.</param>
        public void OnExit(MethodInterceptionArgs e) { }
    }
}
