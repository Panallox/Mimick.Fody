using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method should be called immediately after the object has been initialized.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PostConstructAttribute : Attribute, IInjectAfterInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PostConstructAttribute" /> class.
        /// </summary>
        public PostConstructAttribute()
        {

        }
    }
}
