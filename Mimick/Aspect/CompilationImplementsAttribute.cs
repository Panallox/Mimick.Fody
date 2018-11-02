using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// Indicates compilation instructions that the associated aspect attribute provides an implementation for the
    /// provided interface type.
    /// </summary>
    /// <remarks>
    /// This attribute should be applied to an aspect attribute, and not directly to a class. The aspect attribute, when
    /// applied to a class, will automatically weave the implementation type into the target.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class CompilationImplementsAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CompilationImplementsAttribute"/> class.
        /// </summary>
        public CompilationImplementsAttribute()
        {

        }

        #region Properties
        
        /// <summary>
        /// Gets or sets the type of the interface which must be implemented.
        /// </summary>
        public Type Interface
        {
            get; set;
        }

        #endregion
    }
}
