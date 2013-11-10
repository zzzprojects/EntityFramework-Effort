// --------------------------------------------------------------------------------------------
// <copyright file="TimeFunctions.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

    internal class TimeFunctions
    {
        public static readonly MethodInfo CreateTime =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.CreateTime(0, 0, 0));

        public static readonly MethodInfo GetHour =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetHour(default(TimeSpan)));

        public static readonly MethodInfo GetMinute =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetMinute(default(TimeSpan)));

        public static readonly MethodInfo GetSecond =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetSecond(default(TimeSpan)));

        public static readonly MethodInfo GetMillisecond =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.GetMillisecond(default(TimeSpan)));

        public static readonly MethodInfo AddHours =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddHours(default(TimeSpan), 0));

        public static readonly MethodInfo AddMinutes =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMinutes(default(TimeSpan), 0));

        public static readonly MethodInfo AddSeconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddSeconds(default(TimeSpan), 0));

        public static readonly MethodInfo AddMilliseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMilliseconds(default(TimeSpan), 0));

        public static readonly MethodInfo AddMicroseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddMicroseconds(default(TimeSpan), 0));

        public static readonly MethodInfo AddNanoseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.AddNanoseconds(default(TimeSpan), 0));

        public static readonly MethodInfo DiffHours =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffHours(default(TimeSpan), default(TimeSpan)));

        public static readonly MethodInfo DiffMinutes =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMinutes(default(TimeSpan), default(TimeSpan)));

        public static readonly MethodInfo DiffSeconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffSeconds(default(TimeSpan), default(TimeSpan)));

        public static readonly MethodInfo DiffMilliseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMilliseconds(default(TimeSpan), default(TimeSpan)));

        public static readonly MethodInfo DiffMicroseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffMicroseconds(default(TimeSpan), default(TimeSpan)));

        public static readonly MethodInfo DiffNanoseconds =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.DiffNanoseconds(default(TimeSpan), default(TimeSpan)));
    }
}
