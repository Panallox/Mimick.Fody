using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="MaximumAttribute"/>.
    /// </summary>
    public class MaximumAttributes
    {
        /// <summary>
        /// Passes regardless of the value that is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfAbove(int value)
        {

        }

        /// <summary>
        /// Throws an exception when a value greater than 10 is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfAbove([Maximum(10)] int value)
        {

        }

        /// <summary>
        /// Throws an exception when any argument with a value of greater than 10 is entered.
        /// </summary>
        /// <param name="value1">The 1st value.</param>
        /// <param name="value2">The 2nd value.</param>
        [Maximum(10)]
        public void ThrowIfAnyAbove(int value1, int value2)
        {

        }

        /// <summary>
        /// Throws an exception when the method returns a value of greater than 10.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: Maximum(10)]
        public int ThrowIfReturnAbove(int value) => value;
    }
}
