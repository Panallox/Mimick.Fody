using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Actions
{
    /// <summary>
    /// A class containing methods and properties which should be converted to lower-case.
    /// </summary>
    public class LowerCaseAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property value which should be converted to lower-case.
        /// </summary>
        [LowerCase]
        public string Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Converts the provided value to lower-case.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public string Lower([LowerCase] string value) => value;

        /// <summary>
        /// Converts the provided value to lower-case if the value supports casing.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public object LowerConditional([LowerCase] object value) => value;

        /// <summary>
        /// Converts the returned value to lower-case.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: LowerCase]
        public string LowerReturn(string value) => value;
    }
}
