using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Contracts
{
    /// <summary>
    /// A class containing methods introducing the <see cref="PatternAttribute"/>.
    /// </summary>
    public class PatternAttributes
    {
        /// <summary>
        /// Passes regardless of whether the value does not match a pattern or a <c>null</c> value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void PassIfAnything(string value)
        {

        }

        /// <summary>
        /// Throws an exception when a non-integer value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfNotInteger([Pattern("^\\d+$")] string value)
        {

        }

        /// <summary>
        /// Throws an exception when a non-uppercase string value is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfNotUpperCase([Pattern("^[A-Z\\s]+$")] string value)
        {

        }

        /// <summary>
        /// Throws an exception when a value not encapsulated in brackets is entered.
        /// </summary>
        /// <param name="value">The value.</param>
        public void ThrowIfNotInBrackets([Pattern("^\\(.*\\)$")] string value)
        {

        }
    }
}
