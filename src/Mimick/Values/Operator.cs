using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Values
{
    /// <summary>
    /// Indicates an operator symbol performing a logical operation.
    /// </summary>
    internal enum Operator
    {
        /// <summary>
        /// The symbol is an addition operator.
        /// </summary>
        Add,

        /// <summary>
        /// The symbol is a division operator.
        /// </summary>
        Divide,

        /// <summary>
        /// The symbol is a modulus operator.
        /// </summary>
        Modulus,

        /// <summary>
        /// The symbol is a multiplication operator.
        /// </summary>
        Multiply,

        /// <summary>
        /// The symbol is a subtraction operator.
        /// </summary>
        Subtract,
    }
}
