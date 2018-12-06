using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// Indicates compilation options against an aspect attribute which defines how the attribute will behave during runtime.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class CompilationOptionsAttribute : Attribute
    {
        #region Properties

        /// <summary>
        /// Gets or sets whether, when used against a method or parameter interceptor, any updates to the arguments of the invocation should
        /// be copied across to the arguments of the method before the execution of the method body. If enabled, updates to the <see cref="MethodInterceptionArgs.Arguments"/>
        /// values or an update to the <see cref="ParameterInterceptionArgs.Value"/> will be reflected on the method arguments.
        /// </summary>
        public bool CopyArguments
        {
            get; set;
        }
        
        /// <summary>
        /// Gets or sets how an aspect attribute should be inlined during compile-time. If not specified, defaults to <see cref="Inlining.Truncate"/>.
        /// </summary>
        /// <remarks>
        /// Any inlining specified is introduced cautiously, rather than aggressively. The compiler will attempt to calculate whether
        /// a method is eligible for inlining based on the content of the implemented method body.
        /// </remarks>
        public Inlining Inlining
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the persistence scope of the associated attribute. If not specified, defaults to <see cref="AttributeScope.Singleton"/>.
        /// </summary>
        public AttributeScope Scope
        {
            get; set;
        }

        #endregion
    }

    /// <summary>
    /// Indicates how an aspect attribute should persist during the lifetime of an application.
    /// </summary>
    public enum AttributeScope : int
    {
        /// <summary>
        /// An attribute should be created whenever required.
        /// </summary>
        Adhoc = 1,

        /// <summary>
        /// An attribute should persist for each instance of the containing type.
        /// </summary>
        /// <remarks>
        /// <para>If an attribute marked as <see cref="Instanced"/> has constructor arguments,
        /// the scope is changed to <see cref="MultiInstanced"/></para>
        /// <para>If an attribute marked as <see cref="Instanced"/> is associated with a member or
        /// class which is <c>static</c> the scope is changed to <see cref="Singleton"/></para>
        /// </remarks>
        Instanced = 2,

        /// <summary>
        /// An attribute should persist for each instance of the containing type, with one instance per usage.
        /// </summary>
        /// <remarks>
        /// <para>If an attribute marked as <see cref="MultiInstanced"/> is associated with a member or
        /// class which is <c>static</c> the scope is changed to <see cref="MultiSingleton"/></para>
        /// </remarks>
        MultiInstanced = 3,

        /// <summary>
        /// An attribute should persist as a singleton within the runtime.
        /// </summary>
        Singleton = 4,

        /// <summary>
        /// An attribute should persist as a singleton within the runtime, with one instance per usage.
        /// </summary>
        MultiSingleton = 5
    }

    [Flags]
    public enum Inlining : int
    {
        /// <summary>
        /// A method should not be inlined.
        /// </summary>
        None = 0,

        /// <summary>
        /// A method should be inlined where possible.
        /// </summary>
        Inline = 1,

        /// <summary>
        /// A method should not be implemented or called if the method is considered to be empty.
        /// </summary>
        Truncate = 2,

        /// <summary>
        /// A method should either be inlined or not implemented or called.
        /// </summary>
        InlineAndTruncate = Inline | Truncate
    }
}
