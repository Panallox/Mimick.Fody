using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Actions
{
    /// <summary>
    /// A class containing methods and properties which should perform replacements.
    /// </summary>
    public class ReplaceAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property value which should be replaced.
        /// </summary>
        [Replace("(\\d)", "*")]
        public string Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Replaces the provided value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public string Replace([Replace("(\\d)", "*")] string value) => value;

        /// <summary>
        /// Replaces the provided value if the value supports replacement.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public object ReplaceConditional([Replace("(\\d)", "*")] object value) => value;

        /// <summary>
        /// Replaces the returned value.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: Replace("(\\d)", "*")]
        public string ReplaceReturn(string value) => value;
    }
}
