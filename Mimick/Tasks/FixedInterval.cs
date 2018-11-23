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
        /// Gets the smallest interval time of the task between the task executing. This method is used in the scheduler
        /// system to calculate an optimum amount of time to wait for elapsed executions.
        /// </summary>
        /// <returns>
        /// The smallest interval of the task; otherwise, <c>null</c> if no smallest interval could be determined.
        /// </returns>
        public long? GetSmallest() => value;
    }
}
