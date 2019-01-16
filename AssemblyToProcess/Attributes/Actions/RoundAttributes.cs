using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Actions
{
    /// <summary>
    /// A class containing methods and properties which should be rounded.
    /// </summary>
    public class RoundAttributes
    {
        #region Properties

        /// <summary>
        /// Gets or sets the property value which should be rounded.
        /// </summary>
        [Round]
        public double Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Rounds the provided value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public double Round([Round] double value) => value;

        /// <summary>
        /// Rounds the provided value to two decimal places.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public double RoundTo2([Round(2)] double value) => value;

        /// <summary>
        /// Rounds the provided value to the nearest even value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public double RoundToNearestEven([Round(MidpointRounding.ToEven)] double value) => value;

        /// <summary>
        /// Rounds the provided value if the value supports rounding.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The resulting value.</returns>
        public object RoundConditional([Round] object value) => value;

        /// <summary>
        /// Rounds the returned value.
        /// </summary>
        /// <param name="value">The value to return.</param>
        /// <returns>The value.</returns>
        [return: Round]
        public double RoundReturn(double value) => value;
    }
}
