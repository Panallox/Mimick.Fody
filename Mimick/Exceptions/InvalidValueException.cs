using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Represents an error when a value is not within a required range or satisfies a specific condition.
    /// </summary>
    public class InvalidValueException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException" /> class.
        /// </summary>
        public InvalidValueException() : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InvalidValueException" /> class.
        /// </summary>
        /// <param name="paramName">The name of the parameter.</param>
        public InvalidValueException(string paramName) => ParamName = paramName;

        #region Properties

        /// <summary>
        /// Gets the parameter name.
        /// </summary>
        public string ParamName
        {
            get;
        }

        #endregion
    }
}
