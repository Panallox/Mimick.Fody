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
        /// Gets an estimate of the time required until the next invocation based on the provided previous invocation time and the current time.
        /// </summary>
        /// <param name="previous">The optional date and time that the execution previously elapsed.</param>
        /// <param name="now">The date and time.</param>
        /// <returns>An estimate of the time until the next execution, in milliseconds; otherwise, <c>-1</c>.</returns>
        long GetElapseTime(DateTime? previous, DateTime now);

        /// <summary>
        /// Determines whether the interval has elapsed according to an optional previous invocation time and the current time.
        /// </summary>
        /// <param name="previous">The optional date and time that the execution previously elapsed.</param>
        /// <param name="now">The date and time.</param>
        /// <returns><c>true</c> if the interval has elapsed; otherwise, <c>false</c>.</returns>
        bool HasElapsed(DateTime? previous, DateTime now);
    }
}
