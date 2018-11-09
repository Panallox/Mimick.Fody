using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess
{
    /// <summary>
    /// A class containing methods which must be cached.
    /// </summary>
    public class CachedMethods
    {
        private int id = 1;

        /// <summary>
        /// A non-cached method which will always generate a new value.
        /// </summary>
        /// <returns>The value.</returns>
        public string NonCached() => $"Value {Interlocked.Increment(ref id)}";

        /// <summary>
        /// A cached method which will always return the value generated during the first invocation.
        /// </summary>
        /// <returns>The value.</returns>
        [Cached]
        public string Cached() => $"Value {Interlocked.Increment(ref id)}";

        /// <summary>
        /// A cached method which will return a cached value when the provided argument is the same.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The value.</returns>
        [Cached]
        public string CachedArgument(int value) => $"Value {value}";

        /// <summary>
        /// A cached method which will return a cached value when the provided argument is the same,
        /// until the maximum count of 10 is reached, when the oldest values will be ejected.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns>The value.</returns>
        [Cached(10)]
        public string CachedLimit(int value) => $"Value {value}";

        /// <summary>
        /// A cached method which will return a cached value when the provided argument is the same,
        /// until the maximum time limit is reached.
        /// </summary>
        /// <param name="value">The argument value.</param>
        /// <returns></returns>
        [Cached(int.MaxValue, 10000)]
        public string CachedTimeLimit(int value) => $"Value {value}";
    }
}
