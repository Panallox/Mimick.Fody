using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A timed interval class representing an interval which is based on a schedule.
    /// </summary>
    class CronInterval : ITimedInterval
    {
        private CronSchedule schedule;

        public long? GetSmallest()
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            return null;
        }
    }
}
