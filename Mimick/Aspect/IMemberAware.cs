using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Aspect
{
    /// <summary>
    /// An interface representing a requirement that an aspect attribute should be member aware. If implemented, the aspect attribute will
    /// be provided the <see cref="Member"/> property with the member that the attribute was decorating. The property will not
    /// be available during attribute construction, but will be available if the <see cref="IRequireInitialization"/> interface is implemented.
    /// </summary>
    public interface IMemberAware
    {
        #region Properties

        /// <summary>
        /// Gets or sets the member that the attribute was associated with.
        /// </summary>
        MemberInfo Member
        {
            get; set;
        }

        #endregion
    }
}
