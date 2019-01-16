using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Actions
{
    /// <summary>
    /// A class containing methods and properties which should be trimmed.
    /// </summary>
    public class TrimAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property value which should be trimmed.
        /// </summary>
        [Trim]
        public string Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Trims the provided value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public string Trim([Trim] string value) => value;

        /// <summary>
        /// Trims the provided value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public StringBuilder Trim([Trim] StringBuilder value) => value;

        /// <summary>
        /// Trims the provided value if the value supports trimming.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public object TrimConditional([Trim] object value) => value;

        /// <summary>
        /// Trims the returned value.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: Trim]
        public string TrimReturn(string value) => value;
    }
}
