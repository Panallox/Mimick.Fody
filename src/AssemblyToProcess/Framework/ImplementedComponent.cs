using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Framework
{
    /// <summary>
    /// A class representing an implemented component which should be registered within the framework during configuration.
    /// </summary>
    [Component]
    public class ImplementedComponent : IImplementedComponent
    {
        /// <summary>
        /// Add two values together and return the resulting value.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>
        /// The resulting value.
        /// </returns>
        public int Add(int a, int b) => a + b;
    }

    /// <summary>
    /// An interface representing a component definition which must be implemented.
    /// </summary>
    public interface IImplementedComponent
    {
        /// <summary>
        /// Add two values together and return the resulting value.
        /// </summary>
        /// <param name="a">The first value.</param>
        /// <param name="b">The second value.</param>
        /// <returns>The resulting value.</returns>
        int Add(int a, int b);
    }
}
