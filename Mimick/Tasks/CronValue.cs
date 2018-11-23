using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A class representing a cron value, which is a component piece of a cron expression evaluated into data.
    /// </summary>
    class CronValue
    {
        private readonly string expression;

        /// <summary>
        /// Initializes a new instance of the <see cref="CronValue"/> class.
        /// </summary>
        /// <param name="value">The expression value.</param>
        public CronValue(string value) => expression = value;

        #region Properties

        /// <summary>
        /// Gets a cron value representing all possible values.
        /// </summary>
        public static CronValue All => new CronValue("*") { Frequency = CronFrequency.All, Values = new[] { -1 } };

        /// <summary>
        /// Gets a cron value representing an non-processing any value.
        /// </summary>
        public static CronValue Any => new CronValue("?") { Frequency = CronFrequency.Any, Values = new[] { -1 } };

        /// <summary>
        /// Gets or sets an optional ending range value.
        /// </summary>
        public int? EndAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the frequency which determines how the values are used.
        /// </summary>
        public CronFrequency Frequency
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the parameter associated with the frequency.
        /// </summary>
        public CronFrequencyParameter Parameter
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets an optional starting range value.
        /// </summary>
        public int? StartAt
        {
            get; set;
        }

        /// <summary>
        /// Gets or sets the values of the expression, relative to the frequency.
        /// </summary>
        public int[] Values
        {
            get; set;
        }

        #endregion
        
        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString() => expression;
    }
    
    /// <summary>
    /// An enumerator containing the repeat frequency descriptions.
    /// </summary>
    enum CronFrequency
    {
        /// <summary>
        /// Indicates that the repetition should include all possible values.
        /// </summary>
        All,

        /// <summary>
        /// Indicates that the repetition should not evaluate the field.
        /// </summary>
        Any,

        /// <summary>
        /// Indicates that the repetition should occur at a fixed interval or time.
        /// </summary>
        /// <example>Every 1 second.</example>
        IntervalAt,

        /// <summary>
        /// Indicates that the repetition should occur at an absolute value.
        /// </summary>
        /// <example>At every 1 second of a minute.</example>
        FixedAt,

        /// <summary>
        /// Indicates that the repetition should occur over a range of values.
        /// </summary>
        Range,
    }

    /// <summary>
    /// An enumerator containing the repeat frequency parameters.
    /// </summary>
    enum CronFrequencyParameter
    {
        /// <summary>
        /// Indicates that no parameter has been specified.
        /// </summary>
        None,

        /// <summary>
        /// Indicates that the repetition repeats against the day of the week (Monday-Sunday).
        /// </summary>
        Day,

        /// <summary>
        /// Indicates that the repetition repeats against the day of the month.
        /// </summary>
        DayOfMonth
    }
}
