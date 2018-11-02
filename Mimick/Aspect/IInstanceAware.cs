using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a requirement that an aspect attribute should be instance aware. If implemented, the attribute
    /// will be provided with the current instance during invocation. This interface will not be honoured when used with attributes
    /// under the <see cref="AttributeScope.Singleton"/> scope.
    /// </summary>
    public interface IInstanceAware
    {
        #region Properties

        /// <summary>
        /// Gets or sets the object instance which the attribute is associated with.
        /// </summary>
        object Instance
        {
            get; set;
        }

        #endregion
    }
}
