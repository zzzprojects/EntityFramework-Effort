// --------------------------------------------------------------------------------------------
// <copyright file="DateTimeFunctions.cs" company="Effort Team">
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

namespace Effort.Internal.DbCommandTreeTransformation.Functions
{
    using System;
    using System.Reflection;
    using Effort.Internal.Common;

    internal class DateTimeFunctions
    {
        public static readonly MethodInfo Current =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.CurrentDateTime());

        public static readonly MethodInfo CurrentUtc =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.CurrentUtcDateTime());

        public static readonly MethodInfo CreateDateTime =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.CreateDateTime(0, 0, 0, 0, 0, 0));

        public static readonly MethodInfo GetYear =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetYear(default(DateTime)));

        public static readonly MethodInfo GetMonth =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetMonth(default(DateTime)));

        public static readonly MethodInfo GetDay =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetDay(default(DateTime)));

        public static readonly MethodInfo GetHour =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetHour(default(DateTime)));

        public static readonly MethodInfo GetMinute =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetMinute(default(DateTime)));

        public static readonly MethodInfo GetSecond =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetSecond(default(DateTime)));

        public static readonly MethodInfo GetMillisecond =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetMillisecond(default(DateTime)));

        public static readonly MethodInfo AddYears =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddYears(default(DateTime), 0));

        public static readonly MethodInfo AddMonths =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMonths(default(DateTime), 0));

        public static readonly MethodInfo AddDays =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddDays(default(DateTime), 0));

        public static readonly MethodInfo AddHours =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddHours(default(DateTime), 0));

        public static readonly MethodInfo AddMinutes =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMinutes(default(DateTime), 0));

        public static readonly MethodInfo AddSeconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddSeconds(default(DateTime), 0));

        public static readonly MethodInfo AddMilliseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMilliseconds(default(DateTime), 0));

        public static readonly MethodInfo AddMicroseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMicroseconds(default(DateTime), 0));

        public static readonly MethodInfo AddNanoseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddNanoseconds(default(DateTime), 0));

        public static readonly MethodInfo TruncateTime =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.TruncateTime(default(DateTime)));

        public static readonly MethodInfo DiffYears =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffYears(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffMonths =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMonths(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffDays =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffDays(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffHours =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffHours(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffMinutes =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMinutes(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffSeconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffSeconds(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffMilliseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMilliseconds(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffMicroseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMicroseconds(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DiffNanoseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffNanoseconds(default(DateTime), default(DateTime)));

        public static readonly MethodInfo DayOfYear =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DayOfYear(default(DateTime)));
    
    }
}
