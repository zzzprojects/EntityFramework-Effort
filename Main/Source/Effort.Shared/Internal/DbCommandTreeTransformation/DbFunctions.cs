// --------------------------------------------------------------------------------------------
// <copyright file="DbFunctions.cs" company="Effort Team">
//     Copyright (C) Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.Common;
    using System.Globalization;

    internal class DbFunctions
    {
        #region Math

        public static decimal? Truncate(decimal? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Truncate(input.Value);
        }

        public static double? Truncate(double? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Truncate(input.Value);
        }

        public static decimal? Ceiling(decimal? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Ceiling(input.Value);
        }

        public static double? Ceiling(double? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Ceiling(input.Value);
        }

        public static decimal? Floor(decimal? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Floor(input.Value);
        }

        public static double? Floor(double? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Floor(input.Value);
        }

        public static decimal? Round(decimal? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Round(input.Value);
        }

        public static double? Round(double? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Round(input.Value);
        }

        public static decimal? Round(decimal? input, int? decimals)
        {
            if (!input.HasValue || !decimals.HasValue)
            {
                return null;
            }

            return Math.Round(input.Value, decimals.Value);
        }

        public static double? Round(double? input, int? digits)
        {
            if (!input.HasValue || !digits.HasValue)
            {
                return null;
            }

            return Math.Round(input.Value, digits.Value);
        }

        public static double? Pow(double? x, double? y)
        {
            if (!x.HasValue || !y.HasValue)
            {
                return null;
            }

            return Math.Pow(x.Value, y.Value);
        }

        public static double? Abs(double? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        public static decimal? Abs(decimal? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        public static long? Abs(long? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        public static int? Abs(int? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        public static short? Abs(short? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        public static sbyte? Abs(sbyte? input)
        {
            if (!input.HasValue)
            {
                return null;
            }

            return Math.Abs(input.Value);
        }

        #endregion

        #region String

        public static string Concat(string a, string b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            return String.Concat(a, b);
        }

        public static bool? Contains(string a, string b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            // TODO: culture
            return a.Contains(b);
        }

        public static string Left(string a, int? count)
        {
            if (a == null || count == null)
            {
                return null;
            }

            // TODO: culture
            return a.Substring(0, count.Value);
        }

        public static string Right(string a, int? count)
        {
            if (a == null || count == null)
            {
                return null;
            }

            // TODO: culture
            return a.Substring(a.Length - count.Value);
        }

        public static string ToUpper(string data)
        {
            if (data == null)
            {
                return null;
            }

            // TODO: culture?
            return data.ToUpper();
        }

        public static string ToLower(string data)
        {
            if (data == null)
            {
                return null;
            }

            // TODO: culture?
            return data.ToLower();
        }

        public static int? IndexOf(string a, string b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            // TODO: culture?
            return b.IndexOf(a) + 1;
        }

        public static string ReverseString(string data)
        {
            if (data == null)
            {
                return null;
            }

            return new string(data.ToCharArray().Reverse().ToArray());
        }

        public static string Substring(string data, int? begin, int? length)
        {
            if (data == null || !begin.HasValue || !length.HasValue)
            {
                return null;
            }

            return data.Substring(begin.Value - 1, length.Value);
        }

        public static string Trim(string data)
        {
            if (data == null)
            {
                return null;
            }

            return data.Trim();
        }

        public static string LTrim(string data)
        {
            if (data == null)
            {
                return null;
            }

            return data.TrimStart();
        }

        public static string RTrim(string data)
        {
            if (data == null)
            {
                return null;
            }

            return data.TrimEnd();
        }

        public static int? Length(string data)
        {
            if (data == null)
            {
                return null;
            }

            return data.Length;
        }

		// need case sensitive ??
        public static string Replace(string data, string oldValue, string newValue)
        {
            if (data == null || oldValue == null || newValue == null)
            {
                return null;
            }

            return data.Replace(oldValue, newValue);
        }

        public static bool? StartsWith(string a, string b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            // TODO: culture
            return b.StartsWith(a);
        }

        public static bool? EndsWith(string a, string b)
        {
            if (a == null || b == null)
            {
                return null;
            }

            // TODO: culture
            return b.EndsWith(a);
        }

		// see "private Expression CreateStringComparison(Expression left, Expression right, DbExpressionKind kind)", for case sensitive.
		internal static int CompareTo(string a, string b)
        {
            if (a == null && b == null)
            {
                return 0;
            }

            if (a == null || b == null)
            {
                return -1;
            }

            return a.CompareTo(b);
        }

	public static bool? ContainsCaseInsensitive(string a, string b)
	{
		if (a == null || b == null)
		{
			return null;
		}

		// TODO: culture
		return a.ToLowerInvariant().Contains(b.ToLowerInvariant());
	}

	public static int? IndexOfCaseInsensitive(string a, string b)
	{
		if (a == null || b == null)
		{
			return null;
		}

		// TODO: culture?
		return b.IndexOf(a, StringComparison.OrdinalIgnoreCase) + 1;
	}

	public static bool? StartsWithCaseInsensitive(string a, string b)
	{
		if (a == null || b == null)
		{
			return null;
		}

		// TODO: culture
		return b.StartsWith(a, StringComparison.OrdinalIgnoreCase);
	}

	public static bool? EndsWithCaseInsensitive(string a, string b)
	{
		if (a == null || b == null)
		{
			return null;
		}

		// TODO: culture
		return b.EndsWith(a, StringComparison.OrdinalIgnoreCase);
	}

	#endregion

	#region Datetime

	public static DateTime? CurrentDateTime()
        {
            return DateTime.Now;
        }

        public static DateTime? CurrentUtcDateTime()
        {
            return DateTime.UtcNow;
        }

        public static DateTime? CreateDateTime(
            int? year, 
            int? month, 
            int? day, 
            int? hour, 
            int? minute, 
            int? second)
        {
            if (!year.HasValue || 
                !month.HasValue || 
                !day.HasValue ||
                !hour.HasValue || 
                !minute.HasValue || 
                !second.HasValue)
            {
                return null;
            }

            return new DateTime(
                year.Value,
                month.Value,
                day.Value,
                hour.Value,
                minute.Value,
                second.Value);
        }

        public static int? GetYear(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Year;
        }

        public static int? GetMonth(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Month;
        }

        public static int? GetDay(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }
            
            return date.Value.Day;
        }

        public static int? GetHour(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Hour;
        }

        public static int? GetMinute(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Minute;
        }

        public static int? GetSecond(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Second;
        }

        public static int? GetMillisecond(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Millisecond;
        }

        public static DateTime? AddYears(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddYears(value.Value);
        }

        public static DateTime? AddMonths(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMonths(value.Value);
        }

        public static DateTime? AddDays(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddDays(value.Value);
        }

        public static DateTime? AddHours(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddHours(value.Value);
        }

        public static DateTime? AddMinutes(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMinutes(value.Value);
        }

        public static DateTime? AddSeconds(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddSeconds(value.Value);
        }

        public static DateTime? AddMilliseconds(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMilliseconds(value.Value);
        }

        public static DateTime? AddMicroseconds(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddTicks(value.Value * 10);
        }

        public static DateTime? AddNanoseconds(DateTime? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddTicks(value.Value / 100);
        }

        public static int? DiffYears(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return val2.Value.Year - val1.Value.Year;
        }

        public static int? DiffMonths(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return 
                (val2.Value.Year - val1.Value.Year) * 12 + 
                (val2.Value.Month - val1.Value.Month);
        }

        public static int? DiffDays(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalDays);
        }

        public static int? DiffHours(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalHours);
        }

        public static int? DiffMinutes(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMinutes);
        }

        public static int? DiffSeconds(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalSeconds);
        }

        public static int? DiffMilliseconds(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMilliseconds);
        }

        public static int? DiffMicroseconds(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks / 10);
        }

        public static int? DiffNanoseconds(DateTime? val1, DateTime? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks * 100);
        }

        public static DateTime? TruncateTime(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Date;
        }

        public static int? DayOfYear(DateTime? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.DayOfYear;
        }

        #endregion

        #region DateTimeOffset

        public static DateTimeOffset? CurrentDateTimeOffset()
        {
            return DateTimeOffset.Now;
        }

        public static DateTimeOffset? CreateDateTimeOffset(
           int? year,
           int? month,
           int? day,
           int? hour,
           int? minute,
           int? second,
           int? offsetMinutes)
        {
            if (!year.HasValue ||
                !month.HasValue ||
                !day.HasValue ||
                !hour.HasValue ||
                !minute.HasValue ||
                !second.HasValue ||
                !offsetMinutes.HasValue)
            {
                return null;
            }

            return new DateTimeOffset(
                year.Value,
                month.Value,
                day.Value,
                hour.Value,
                minute.Value,
                second.Value,
                TimeSpan.FromMinutes(offsetMinutes.Value));
        }

        public static int? GetYear(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Year;
        }

        public static int? GetMonth(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Month;
        }

        public static int? GetDay(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Day;
        }

        public static int? GetHour(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Hour;
        }

        public static int? GetMinute(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Minute;
        }

        public static int? GetSecond(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Second;
        }

        public static int? GetMillisecond(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.Millisecond;
        }

        public static DateTimeOffset? AddYears(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddYears(value.Value);
        }

        public static DateTimeOffset? AddMonths(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMonths(value.Value);
        }

        public static DateTimeOffset? AddDays(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddDays(value.Value);
        }

        public static DateTimeOffset? AddHours(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddHours(value.Value);
        }

        public static DateTimeOffset? AddMinutes(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMinutes(value.Value);
        }

        public static DateTimeOffset? AddSeconds(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddSeconds(value.Value);
        }

        public static DateTimeOffset? AddMilliseconds(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddMilliseconds(value.Value);
        }

        public static DateTimeOffset? AddMicroseconds(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddTicks(value.Value * 10);
        }

        public static DateTimeOffset? AddNanoseconds(DateTimeOffset? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.AddTicks(value.Value / 100);
        }

        public static int? DiffYears(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return val2.Value.Year - val1.Value.Year;
        }

        public static int? DiffMonths(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return
                (val2.Value.Year - val1.Value.Year) * 12 +
                (val2.Value.Month - val1.Value.Month);
        }

        public static int? DiffDays(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalDays);
        }

        public static int? DiffHours(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalHours);
        }

        public static int? DiffMinutes(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMinutes);
        }

        public static int? DiffSeconds(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalSeconds);
        }

        public static int? DiffMilliseconds(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMilliseconds);
        }

        public static int? DiffMicroseconds(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks / 10);
        }

        public static int? DiffNanoseconds(DateTimeOffset? val1, DateTimeOffset? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks * 100);
        }

        public static DateTimeOffset? TruncateTime(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return new DateTimeOffset(date.Value.Date, date.Value.Offset);
        }

        public static int? DayOfYear(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return date.Value.DayOfYear;
        }

        public static int? GetTotalOffsetMinutes(DateTimeOffset? date)
        {
            if (!date.HasValue)
            {
                return null;
            }

            return (int)date.Value.Offset.TotalMinutes;
        }

        #endregion

        #region Guid

        internal static int CompareTo(Guid? a, Guid? b)
        {
            if (a == null && b == null)
            {
                return 0;
            }

            if (a == null || b == null)
            {
                return -1;
            }

            return a.Value.CompareTo(b.Value);
        }

        #endregion

        #region Time

        public static TimeSpan? CreateTime(
            int? hour,
            int? minute,
            int? second)
        {
            if (!hour.HasValue ||
                !minute.HasValue ||
                !second.HasValue)
            {
                return null;
            }

            return new TimeSpan(hour.Value, minute.Value, second.Value);
        }

        public static int? GetHour(TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return null;
            }

            return time.Value.Hours;
        }

        public static int? GetMinute(TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return null;
            }

            return time.Value.Minutes;
        }

        public static int? GetSecond(TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return null;
            }

            return time.Value.Seconds;
        }

        public static int? GetMillisecond(TimeSpan? time)
        {
            if (!time.HasValue)
            {
                return null;
            }

            return time.Value.Milliseconds;
        }

        public static TimeSpan? AddHours(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromHours(value.Value));
        }

        public static TimeSpan? AddMinutes(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromMinutes(value.Value));
        }

        public static TimeSpan? AddSeconds(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromSeconds(value.Value));
        }

        public static TimeSpan? AddMilliseconds(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromMilliseconds(value.Value));
        }

        public static TimeSpan? AddMicroseconds(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromTicks(value.Value * 10));
        }

        public static TimeSpan? AddNanoseconds(TimeSpan? date, int? value)
        {
            if (!date.HasValue || !value.HasValue)
            {
                return null;
            }

            return date.Value.Add(TimeSpan.FromTicks(value.Value / 100));
        }

        public static int? DiffHours(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalHours);
        }

        public static int? DiffMinutes(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMinutes);
        }

        public static int? DiffSeconds(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalSeconds);
        }

        public static int? DiffMilliseconds(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).TotalMilliseconds);
        }

        public static int? DiffMicroseconds(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks / 10);
        }

        public static int? DiffNanoseconds(TimeSpan? val1, TimeSpan? val2)
        {
            if (!val1.HasValue || !val2.HasValue)
            {
                return null;
            }

            return (int)((val2.Value - val1.Value).Ticks * 100);
        }

        #endregion

        public static string ToString(object obj)
        {
            var format = CultureInfo.InvariantCulture;

            return String.Format(format, "{0}", obj);
        }

        public static T? TryParse<T>(string s) where T : struct
        {
            var format = CultureInfo.InvariantCulture;

            try
            {
                return (T)Convert.ChangeType(s, typeof(T), format);
            }
            catch
            {
                return null;
            }
        }
    }
}
