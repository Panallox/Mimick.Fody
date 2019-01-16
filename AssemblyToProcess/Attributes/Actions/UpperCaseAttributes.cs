using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Actions
{
    /// <summary>
    /// A class containing methods and properties which should be converted to upper-case.
    /// </summary>
    public class UpperCaseAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property value which should be converted to upper-case.
        /// </summary>
        [UpperCase]
        public string Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Converts the provided value to upper-case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public string Upper([UpperCase] string value) => value;

        /// <summary>
        /// Converts the provided value to upper-case if the value supports casing.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public object UpperConditional([UpperCase] object value) => value;

        /// <summary>
        /// Converts the returned value to upper-case.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: UpperCase]
        public string UpperReturn(string value) => value;
    }
}
