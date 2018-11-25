using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A timed interval class representing a fixed interval between task executions.
    /// </summary>
    class FixedInterval : ITimedInterval
    {
        private readonly long value;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixedInterval"/> class.
        /// </summary>
        /// <param name="interval">The interval.</param>
        public FixedInterval(double interval) => value = (long)interval;

        /// <summary>
        /// Gets an estimate of the time required until the next invocation based on the provided previous invocation time,
        /// and the current time, expressed in milliseconds.
        /// </summary>
        /// <param name="previous">The optional date and time that the execution previously elapsed.</param>
        /// <param name="now">The date and time.</param>
        /// <returns>
        /// An estimate of the time until the next execution, in milliseconds; otherwise, <c>-1</c>.
        /// </returns>
        public long GetElapseTime(DateTime? previous, DateTime now)
        {
            if (previous == null)
                return value;

            var remaining = value - (now - previous.Value).TotalMilliseconds;
            return remaining < 0 ? -1 : (long)remaining;
        }

        /// <summary>
        /// Determines whether the interval has elapsed according to an optional previous invocation time and the current time.
        /// </summary>
        /// <param name="previous">The optional date and time that the execution previously elapsed.</param>
        /// <param name="now">The date and time.</param>
        /// <returns>
        ///   <c>true</c> if the interval has elapsed; otherwise, <c>false</c>.
        /// </returns>
        public bool HasElapsed(DateTime? previous, DateTime now)
        {
            if (previous == null)
                return true;

            return value - (now - previous.Value).TotalMilliseconds <= 0;
        }
    }
}
