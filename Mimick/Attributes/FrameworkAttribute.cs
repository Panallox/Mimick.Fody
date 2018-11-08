using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated class or member has some relevance to the framework. This attribute alone is irrelevant, but
    /// implementing attributes will be resolved by the framework.
    /// </summary>
    public abstract class FrameworkAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrameworkAttribute" /> class.
        /// </summary>
        protected FrameworkAttribute()
        {

        }
    }
}
