using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes.Behaviours
{
    /// <summary>
    /// A class expected to implement the <see cref="IDisposable"/> interface.
    /// </summary>
    public class DisposableAttributes
    {
        /// <summary>
        /// Gets or sets the count of the number of times the dispose methods were called.
        /// </summary>
        public int DisposeCount { get; set; } = 0;

        /// <summary>
        /// Called when the class is disposed.
        /// </summary>
        [Disposable]
        public void ShutdownA() => DisposeCount++;

        /// <summary>
        /// Called when the class is disposed.
        /// </summary>
        [Disposable]
        public void ShutdownB() => DisposeCount++;
    }
}
  