// --------------------------------------------------------------------------------------------
// <copyright file="DbFunctions.cs" company="Effort Team">
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

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.Common;

    internal class DbFunctions
    {
        public static readonly MethodInfo TruncateMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Truncate(0.0));

        public static readonly MethodInfo TruncateDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Truncate(0.0M));

        public static readonly MethodInfo CeilingMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Ceiling(0.0));

        public static readonly MethodInfo CeilingDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Ceiling(0.0M));

        public static readonly MethodInfo FloorMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Floor(0.0));

        public static readonly MethodInfo FloorDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Floor(0.0M));

        public static readonly MethodInfo RoundMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Round(0.0));

        public static readonly MethodInfo RoundDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Round(0.0M));

        public static readonly MethodInfo RoundDigitsMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Round(0.0, 0));

        public static readonly MethodInfo RoundDigitsDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Round(0.0M, 0));

        public static readonly MethodInfo PowMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Pow(0.0, 0.0));

        public static readonly MethodInfo AbsMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs(0.0));

        public static readonly MethodInfo AbsMethodDec =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs(0.0M));

        public static readonly MethodInfo AbsMethod64 =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs(0L));

        public static readonly MethodInfo AbsMethod32 =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((int?)0));

        public static readonly MethodInfo AbsMethod16 =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((short?)0));

        public static readonly MethodInfo AbsMethod8 =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((sbyte?)0));

        public static int IndexOf(string a, string b)
        {
            return a.IndexOf(b) + 1;
        }

        public static string ReverseString(string old)
        {
            return new string(old.ToCharArray().Reverse().ToArray());
        }

        public static string Substring(string data, int? begin, int? length)
        {
            return data.Substring(begin.Value - 1, length.Value);
        }

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
    }
}
