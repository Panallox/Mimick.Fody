using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="MinLengthAttribute"/>.
    /// </summary>
    public class MinLengthAttributes
    {
        /// <summary>
        /// Passes regardless of the collection count or whether a <c>null</c> value is entered.
        /// </summary>
        /// <param name="list">The collection.</param>
        public void PassIfBelow(IList<string> list)
        {

        }

        /// <summary>
        /// Throws an exception when the length of the string is less than 2.
        /// </summary>
        /// <param name="value">The string value.</param>
        public void ThrowIfBelow([MinLength(2)] string value)
        {

        }

        /// <summary>
        /// Throws an exception when the count of the collection is less than 2.
        /// </summary>
        /// <param name="list">The collection.</param>
        public void ThrowIfBelow([MinLength(2)] IList<string> list)
        {

        }

        /// <summary>
        /// Throws an exception when the count of the enumerable is less than 2.
        /// </summary>
        /// <param name="enumerable">The enumerable.</param>
        public void ThrowIfBelow([MinLength(2)] IEnumerable<string> enumerable)
        {

        }

        /// <summary>
        /// Throws an exception when the length of the array is less than 2.
        /// </summary>
        /// <param name="array">The array.</param>
        public void ThrowIfBelow([MinLength(2)] string[] array)
        {

        }

        /// <summary>
        /// Throws an exception when the length of any of the strings is less than 2.
        /// </summary>
        /// <param name="value1">The string value.</param>
        /// <param name="value2">The string value.</param>
        [MinLength(2)]
        public void ThrowIfAnyBelow(string value1, string value2)
        {

        }
    }
}
