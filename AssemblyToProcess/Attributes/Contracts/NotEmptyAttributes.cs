using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="NotEmptyAttribute"/>.
    /// </summary>
    public class NotEmptyAttributes
    {
        /// <summary>
        /// Passes regardless of whether a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfEmpty(string value)
        {

        }

        /// <summary>
        /// Passes regardless of whether a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfEmpty(ICollection<int> value)
        {

        }

        /// <summary>
        /// Passes regardless of whether a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfEmpty(IEnumerable<int> value)
        {

        }

        /// <summary>
        /// Throws an exception when a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfEmpty([NotEmpty] string value)
        {

        }

        /// <summary>
        /// Throws an exception when a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfEmpty([NotEmpty] ICollection<int> value)
        {

        }

        /// <summary>
        /// Throws an exception when a <c>null</c> or empty value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfEmpty([NotEmpty] IEnumerable<int> value)
        {

        }

        /// <summary>
        /// Throws an exception when any argument has a <c>null</c> or empty value entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [NotEmpty]
        public void ThrowIfAnyEmpty(string value1, string value2)
        {

        }

        /// <summary>
        /// Throws an exception when any argument has a <c>null</c> or empty value entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [NotEmpty]
        public void ThrowIfAnyEmpty(ICollection<int> value1, ICollection<int> value2)
        {

        }

        /// <summary>
        /// Throws an exception when any argument has a <c>null</c> or empty value entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [NotEmpty]
        public void ThrowIfAnyEmpty(IEnumerable<int> value1, IEnumerable<int> value2)
        {

        }
    }
}
