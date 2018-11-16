using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="MaxLengthAttribute"/>.
    /// </summary>
    public class MaxLengthAttributes
    {
        /// <summary>
        /// Passes regardless of the collection count or whether a <c>null</c> value is entered.
        /// </summary>
        /// <param name="list">The collection.</param>
        public void PassIfAbove(IList<string> list)
        {

        }

        /// <summary>
        /// Throws an exception when the length of the string is greater than 2.
        /// </summary>
        /// <param name="value">The string value.</param>
        public void ThrowIfAbove([MaxLength(2)] string value)
        {

        }

        /// <summary>
        /// Throws an exception when the count of the collection is greater than 2.
        /// </summary>
        /// <param name="list">The collection.</param>
        public void ThrowIfAbove([MaxLength(2)] IList<string> list)
        {

        }

        /// <summary>
        /// Throws an exception when the count of the enumerable is greater than 2.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public void ThrowIfAbove([MaxLength(2)] IEnumerable<string> enumerable)
        {

        }

        /// <summary>
        /// Throws an exception when the length of the array is greater than 2.
        /// </summary>
        /// <param name="array">The array.</param>
        public void ThrowIfAbove([MaxLength(2)] string[] array)
        {

        }

        /// <summary>
        /// Throws an exception when the length of any of the strings is greater than 2.
        /// </summary>
        /// <param name="value1">The string value.</param>
        /// <param name="value2">The string value.</param>
        [MaxLength(2)]
        public void ThrowIfAnyAbove(string value1, string value2)
        {

        }
    }
}
