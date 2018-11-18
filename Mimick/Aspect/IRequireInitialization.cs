using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a requirement that an aspect attribute must be initialized after having been constructed. If implemented, an attribute
    /// will be initialized after having been fully configured.
    /// </summary>
    public interface IRequireInitialization
    {
        /// <summary>
        /// Initialize the attribute.
        /// </summary>
        void Initialize();
    }
}
