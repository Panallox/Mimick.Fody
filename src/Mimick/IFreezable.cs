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
        /// <exception cref="FrozenException">If the object has already been frozen.</exception>
        void Freeze();
    }

    /// <summary>
    /// An exception class thrown when an attempt is made to modify a frozen object.
    /// </summary>
    public class FrozenException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FrozenException" /> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public FrozenException(string message) : base(message)
        {

        }
    }
}
