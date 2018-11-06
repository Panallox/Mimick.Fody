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
        /// Gets or sets the persistence scope of the associated attribute.
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
        /// class which is <c>static</c> the scope is changed to <see cref="Singleton"/></para>
        /// </remarks>
        MultiInstanced = 3,

        /// <summary>
        /// An attribute should persist as a singleton within the runtime.
        /// </summary>
        Singleton = 4
    }
}
