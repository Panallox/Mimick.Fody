using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Represents an error when a value should not be considered empty.
    /// </summary>
    public class EmptyException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyException" /> class.
        /// </summary>
        public EmptyException() : this(null)
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyException"/> class.
        /// </summary>
        /// <param name="paramName">Name of the parameter.</param>
        public EmptyException(string paramName) => ParamName = paramName;

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
