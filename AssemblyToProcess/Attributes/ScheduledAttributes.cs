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
        private long counter;

        #region Properties

        /// <summary>
        /// Gets the counter.
        /// </summary>
        public long Counter => Interlocked.Read(ref counter);

        #endregion

        /// <summary>
        /// Increments the counter of the attributes on a timed interval.
        /// </summary>
        [Scheduled(1000)]
        public void IncrementCounter() => Interlocked.Increment(ref counter);
    }
}
