using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing an object instance which supports being frozen, which prevents further modifications to the object.
    /// </summary>
    /// <remarks>
    /// A type implementing the <see cref="IFreezable"/> interface should prevent modifications to fields and properties once the object has been frozen.
    /// </remarks>
    public interface IFreezable
    {
        #region Properties

        /// <summary>
        /// Gets whether the object instance has been frozen.
        /// </summary>
        bool IsFrozen
        {
            get;
        }

        #endregion

        /// <summary>
        /// Freezes the object instance and prevents further modifications to the fields and properties of the instance.
        /// </summary>
        void Freeze();
    }
}
