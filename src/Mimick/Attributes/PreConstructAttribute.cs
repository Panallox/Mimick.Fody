using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick.Aspect;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method should be called immediately before the object has been initialized. The method
    /// will be called before the object constructor body, but after the base constructor is called.
    /// </summary>
    /// <remarks>
    /// This attribute cannot be used against generic methods. There is the possibility of a race condition here in that
    /// a method invoked before the object has been initialized could attempt to access fields and properties which have not
    /// yet been initialized by the framework. This method should not attempt to access any members of the same object.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class PreConstructAttribute : Attribute, IInjectBeforeInitializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PreConstructAttribute" /> class.
        /// </summary>
        public PreConstructAttribute()
        {

        }
    }
}
