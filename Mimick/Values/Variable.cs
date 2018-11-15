using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// A class representing a variable which must be resolved prior to an expression value being evaluated.
    /// </summary>
    public sealed class Variable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Variable" /> class.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public Variable(string expression) => Expression = expression;

        #region Properties

        /// <summary>
        /// Gets the variable expression.
        /// </summary>
        public string Expression
        {
            get;
        }

        /// <summary>
        /// Gets or sets the value of the variable.
        /// </summary>
        public object Value
        {
            get; set;
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => Expression;
    }
}
