using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Mimick;

namespace AssemblyToProcess.Attributes
{
    /// <summary>
    /// A class containing methods introducing the <see cref="ScheduledAttribute"/> decoration.
    /// </summary>
    [Component]
    public class ScheduledAttributes
    {
        private long scheduledCounter;
        private long timedCounter;

        #region Properties

        /// <summary>
        /// Gets the scheduled counter.
        /// </summary>
        public long ScheduledCounter => Interlocked.Read(ref scheduledCounter);

        /// <summary>
        /// Gets the timed counter.
        /// </summary>
        public long TimedCounter => Interlocked.Read(ref timedCounter);

        #endregion

        /*
        /// <summary>
        /// Increments the counter of the attributes on a scheduled interval.
        /// </summary>
        [Scheduled("0/2 * * * * *")]
        public void IncrementScheduledCounter() => Interlocked.Increment(ref scheduledCounter);
        */

        /// <summary>
        /// Increments the counter of the attributes on a timed interval.
        /// </summary>
        [Scheduled(1000)]
        public void IncrementTimedCounter() => Interlocked.Increment(ref timedCounter);
    }
}
