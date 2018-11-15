using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface containing options to configure the registration of one or more components within the framework.
    /// </summary>
    public interface IComponentRegistration
    {
        /// <summary>
        /// Sets the scope of the components, which determines how the components should persist.
        /// </summary>
        /// <param name="scope">The scope.</param>
        /// <returns></returns>
        IComponentRegistration ToScope(Scope scope);
    }
}
