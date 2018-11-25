using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated class is a component of the framework, and should be registered when the framework initializes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ComponentAttribute : FrameworkAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAttribute" /> class.
        /// </summary>
        public ComponentAttribute() : this(null, Scope.Singleton)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAttribute" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        public ComponentAttribute(string name) : this(name, Scope.Singleton)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAttribute"/> class.
        /// </summary>
        /// <param name="scope">The scope.</param>
        public ComponentAttribute(Scope scope) : this(null, scope)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentAttribute"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="scope">The scope.</param>
        public ComponentAttribute(string name, Scope scope)
        {
            Name = name;
            Scope = scope;
        }

        #region Properties

        /// <summary>
        /// Gets or sets the optional name of the component when registered in the framework. When set, the component can still be
        /// resolved based on the <see cref="Type"/>, but a name provides an additional level of qualification.
        /// </summary>
        public string Name
        {
            get;
        }

        /// <summary>
        /// Gets or sets how the component should be managed within the containing framework.
        /// </summary>
        public Scope Scope
        {
            get;
        }

        #endregion
    }

    /// <summary>
    /// Indicates how a component should be managed within the containing framework.
    /// </summary>
    public enum Scope
    {
        /// <summary>
        /// The component should be created each time it's required.
        /// </summary>
        Adhoc,

        /// <summary>
        /// The component should persist as a singleton instance for the duration of the framework.
        /// </summary>
        Singleton,

        /// <summary>
        /// The component should persist as a singleton instance for the duration of a thread, created only when required.
        /// </summary>
        Thread,
    }
}
