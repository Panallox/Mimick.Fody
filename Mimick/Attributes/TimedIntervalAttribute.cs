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
    public sealed class TimedIntervalAttribute : FrameworkAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalAttribute"/> class.
        /// </summary>
        /// <param name="milliseconds">The milliseconds.</param>
        public TimedIntervalAttribute(int milliseconds) : this(0, 0, 0, 0, milliseconds) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalAttribute"/> class.
        /// </summary>
        /// <param name="seconds">The seconds.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public TimedIntervalAttribute(int seconds, int milliseconds) : this(0, 0, 0, seconds, milliseconds) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalAttribute"/> class.
        /// </summary>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public TimedIntervalAttribute(int minutes, int seconds, int milliseconds) : this(0, 0, minutes, seconds, milliseconds) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalAttribute"/> class.
        /// </summary>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public TimedIntervalAttribute(int hours, int minutes, int seconds, int milliseconds) : this(0, hours, minutes, seconds, milliseconds) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TimedIntervalAttribute"/> class.
        /// </summary>
        /// <param name="days">The days.</param>
        /// <param name="hours">The hours.</param>
        /// <param name="minutes">The minutes.</param>
        /// <param name="seconds">The seconds.</param>
        /// <param name="milliseconds">The milliseconds.</param>
        public TimedIntervalAttribute(int days, int hours, int minutes, int seconds, int milliseconds)
        {
            Interval = new TimeSpan(days, hours, minutes, seconds, milliseconds);
        }

        #region Properties

        /// <summary>
        /// Gets the interval between method invocations.
        /// </summary>
        public TimeSpan Interval
        {
            get;
        }

        #endregion
    }
}
