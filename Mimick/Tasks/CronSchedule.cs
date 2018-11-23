using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A class representing a cron schedule which has been evaluated from an expression.
    /// </summary>
    class CronSchedule
    {
        #region Properties

        /// <summary>
        /// Gets or sets the day of month value.
        /// </summary>
        public CronValue DayOfMonth { get; set; } = CronValue.All;

        /// <summary>
        /// Gets or sets the day of week value.
        /// </summary>
        public CronValue DayOfWeek { get; set; } = CronValue.Any;

        /// <summary>
        /// Gets or sets the hours value.
        /// </summary>
        public CronValue Hour { get; set; } = CronValue.All;

        /// <summary>
        /// Gets or sets the minutes value.
        /// </summary>
        public CronValue Minute { get; set; } = CronValue.All;

        /// <summary>
        /// Gets or sets the months value.
        /// </summary>
        public CronValue Month { get; set; } = CronValue.All;

        /// <summary>
        /// Gets or sets the seconds value.
        /// </summary>
        public CronValue Second { get; set; } = CronValue.All;

        /// <summary>
        /// Gets or sets the yeat value.
        /// </summary>
        public CronValue Year { get; set; } = CronValue.All;

        #endregion
    }
}
