using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick
{
    /// <summary>
    /// Indicates that the associated method should be invoked at a provided interval from when the framework initializes. If the attribute is
    /// associated with a non-static method, the declaring type should be decorated with the <see cref="ComponentAttribute"/> decoration. The method
    /// should not contain any non-optional parameters, except for a <see cref="ITimedTask"/> parameter which will be populated with the task relevant
    /// to the method invocation.
    /// </summary>
    /// <remarks>
    /// A timed interval executes only after the interval has expired between invocations, meaning that the next interval will not start until the
    /// previous method invocation has completed. If the method throws an exception during the invocation the task will continue.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ScheduledAttribute : FrameworkAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAttribute"/> class.
        /// </summary>
        /// <param name="milliseconds">The interval between executions in milliseconds.</param>
        public ScheduledAttribute(double milliseconds) => FixedInterval = TimeSpan.FromMilliseconds(milliseconds);

        /// <summary>
        /// Initializes a new instance of the <see cref="ScheduledAttribute"/> class.
        /// </summary>
        /// <param name="cron">The cron expression describing when the invocations should occur.</param>
        public ScheduledAttribute(string cron) => CronInterval = cron;

        #region Properties

        /// <summary>
        /// Gets the interval between method invocations.
        /// </summary>
        public string CronInterval
        {
            get;
        }

        /// <summary>
        /// Gets the interval between method invocations.
        /// </summary>
        public TimeSpan FixedInterval
        {
            get;
        }

        #endregion
    }
}
