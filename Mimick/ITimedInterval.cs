using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// An interface representing a time interval which describes when a task should execute.
    /// </summary>
    public interface ITimedInterval
    {
        /// <summary>
        /// Gets the smallest interval time of the task between the task executing. This method is used in the scheduler
        /// system to calculate an optimum amount of time to wait for elapsed executions.
        /// </summary>
        /// <returns>The smallest interval, in milliseconds, of the task; otherwise, <c>null</c> if no interval could be determined.</returns>
        long? GetSmallest();
    }
}
