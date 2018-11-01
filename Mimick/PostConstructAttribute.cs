using Mimick.Aspect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PostConstructAttribute : Attribute, IInitializer
    {
        /// <summary>
        /// Gets the scope of when the initializer should be called.
        /// </summary>
        public InitializeScope When => InitializeScope.AfterInit;
    }
}
