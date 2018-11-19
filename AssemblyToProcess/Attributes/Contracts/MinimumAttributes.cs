using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="MinimumAttribute"/>.
    /// </summary>
    public class MinimumAttributes
    {
        /// <summary>
        /// Passes regardless of the value that is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfBelow(int value)
        {

        }

        /// <summary>
        /// Throws an exception when a value less than 10 is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfBelow([Minimum(10)] int value)
        {

        }

        /// <summary>
        /// Throws an exception when any argument with a value of less than 10 is entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [Minimum(10)]
        public void ThrowIfAnyBelow(int value1, int value2)
        {

        }

        /// <summary>
        /// Throws an exception when the method returns a value of less than 10.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: Minimum(10)]
        public int ThrowIfReturnsBelow(int value) => value;
    }
}
