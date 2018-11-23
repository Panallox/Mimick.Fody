using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mimick.Tasks
{
    /// <summary>
    /// A class containing methods for parsing cron expressions into cron schedules.
    /// </summary>
    static class CronParser
    {
        private const int MaximumCronParts = 7;

        /// <summary>
        /// Parse a provided cron expression into a cron schedule.
        /// </summary>
        /// <param name="expression">The cron expression.</param>
        /// <returns>A <see cref="CronSchedule"/> value.</returns>
        /// <exception cref="FormatException">If the expression cannot be evaluated.</exception>
        public static CronSchedule Parse(string expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            var parts = expression.Trim().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length < 4)
                throw new FormatException($"Cannot parse '{expression}', expected at least seconds, minutes, hours and days");

            var values = new CronValue[MaximumCronParts];

            for (int i = 0; i < MaximumCronParts; i++)
            {
                var part = parts[i];
                var type = (CronPart)i;

                values[i] = Parse(part, type);
            }

            return new CronSchedule
            {
                Second = values[0],
                Minute = values[1],
                Hour = values[2],
                DayOfMonth = values[3],
                Month = values[4],
                DayOfWeek = values[5],
                Year = values[6]
            };
        }

        /// <summary>
        /// Parses a value from a cron expression part.
        /// </summary>
        /// <param name="part">The part.</param>
        /// <param name="type">The type.</param>
        /// <returns>A <see cref="CronValue"/> value.</returns>
        /// <exception cref="FormatException">If the expression cannot be evaluated.</exception>
        private static CronValue Parse(string part, CronPart type)
        {
            var buf = part.ToCharArray();
            var i = 0;
            var value = new CronValue(part);
            var values = new List<int>();
            var context = new CronContext { buf = buf, index = i, length = buf.Length };

            value.Frequency = CronFrequency.IntervalAt;
            value.Parameter = CronFrequencyParameter.None;

            var c = buf[i];

            if (c == '*')
                values.Add(-1);
            else if (c == '?')
            {
                if (type != CronPart.DayOfMonth && type != CronPart.DayOfWeek)
                    throw new FormatException($"Cannot parse '{part}', unexpected '?' symbol");

                return CronValue.Any;
            }
            else if (char.IsNumber(c))
                values.Add(ParseNumber(context, type));
            else if (char.IsLetter(c))
            {
                if (type != CronPart.Months)
                    throw new FormatException($"Cannot parse '{part}', expected a number, * or ? value");

                values.Add(ParseMonth(context, type));
            }

            if (!context.HasNext)
            {
                var n = values.First();

                if (n == -1)
                    return CronValue.All;

                value.Values = values.ToArray();
                return value;
            }
        }

        /// <summary>
        /// Parses a month value from a cron expression part.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="type">The processing type.</param>
        /// <returns>The month value expressed as a number.</returns>
        /// <exception cref="FormatException">If the month cannot be evaluated.</exception>
        private static int ParseMonth(CronContext context, CronPart type)
        {
            if (context.length - context.index < 3)
                throw new FormatException($"Cannot parse a month, not enough content available");

            var a = char.ToLower(context.Next());
            var b = char.ToLower(context.Next());
            var c = char.ToLower(context.Next());

            if (!char.IsLetter(a) || !char.IsLetter(b) || !char.IsLetter(c))
                throw new FormatException($"Cannot parse a month, expected a text value");

            if (a == 'j')
            {
                if (b == 'a' && c == 'n') return 1;
                if (b == 'u' && c == 'n') return 6;
                if (b == 'u' && c == 'l') return 7;
            }
            else if (a == 'a')
            {
                if (b == 'p' && c == 'r') return 4;
                if (b == 'u' && c == 'g') return 8;
            }
            else if (a == 'm' && b == 'a')
            {
                if (c == 'r') return 3;
                if (c == 'y') return 5;
            }
            else if (a == 'o' && b == 'c' && c == 't')
                return 10;
            else if (a == 'd' && b == 'e' && c == 'c')
                return 12;
            else if (a == 'n' && b == 'o' && c == 'v')
                return 11;
            else if (a == 'f' && b == 'e' && c == 'b')
                return 2;
            else if (a == 's' && b == 'e' && c == 'p')
                return 9;

            throw new FormatException($"Cannot parse a month, expected a valid value, got '{a}{b}{c}'");
        }

        /// <summary>
        /// Parses a number value from a cron expression part.
        /// </summary>
        /// <param name="context">The processing context.</param>
        /// <param name="type">The processing type.</param>
        /// <returns>The number value.</returns>
        /// <exception cref="FormatException">If the number cannot be evaluated.</exception>
        private static int ParseNumber(CronContext context, CronPart type)
        {
            var start = context.index;

            while (context.index < context.length && char.IsNumber(context.buf[context.index]))
                context.Next();

            if (start == context.index)
                throw new FormatException($"Cannot parse a number at position {i} of {type}");

            var text = new string(context.buf, start, context.index - start);

            if (!int.TryParse(text, out var number))
                throw new FormatException($"Cannot parse a number at position {i} of {type}");

            switch (type)
            {
                case CronPart.Hours:
                case CronPart.Minutes:
                case CronPart.Seconds:
                    if (number >= 0 && number <= 59)
                        return number;
                    break;
                case CronPart.Months:
                    if (number >= 1 && number <= 12)
                        return number;
                    break;
                case CronPart.DayOfMonth:
                    if (number >= 1 && number <= 31)
                        return number;
                    break;
                case CronPart.Years:
                    if (number >= 1900 && number <= 9999)
                        return number;
                    break;
            }

            throw new FormatException($"Cannot parse an invalid number value {number} of {type}");
        }

        /// <summary>
        /// An enumerator containing the different parts of a cron expression.
        /// </summary>
        enum CronPart : int
        {
            Seconds = 0,
            Minutes = 1,
            Hours = 2,
            DayOfMonth = 3,
            Months = 4,
            DayOfWeek = 5,
            Years = 6
        }

        /// <summary>
        /// A class representing a cron-parsing context.
        /// </summary>
        class CronContext
        {
            public char[] buf;
            public int index;
            public int length;

            public bool HasNext => index + 1 < length;
            public char Next() => buf[index++];
        }
    }
}
