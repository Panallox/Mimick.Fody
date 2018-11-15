using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing methods which provide exception suppression.
    /// </summary>
    public class SuppressAttributes
    {
        /// <summary>
        /// Throws an exception without any suppression.
        /// </summary>
        public void ThrowException() => throw new Exception();

        /// <summary>
        /// Throws an exception and expects the exception to be suppressed.
        /// </summary>
        [Suppress]
        public void ThrowAndSuppressException() => throw new Exception();

        /// <summary>
        /// Throws and filters an exception, causing the exception to be thrown as expected.
        /// </summary>
        [Suppress(Types = new[] { typeof(InvalidCastException) })]
        public void ThrowAndFilterException() => throw new InvalidOperationException();

        /// <summary>
        /// Throws and filters an exception, causing the exception to be suppressed.
        /// </summary>
        [Suppress(Types = new[] { typeof(InvalidOperationException )})]
        public void ThrowAndFilterAndSuppressException() => throw new InvalidOperationException();

        /// <summary>
        /// Throws an exception and expects the exception to be suppressed, and returns the default return value.
        /// </summary>
        /// <returns>The default value.</returns>
        [Suppress]
        public int ThrowAndSuppressExceptionAndReturnDefault() => throw new Exception();
    }
}
