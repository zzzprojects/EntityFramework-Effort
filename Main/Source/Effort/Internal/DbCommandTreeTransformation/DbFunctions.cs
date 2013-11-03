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
        #region Math exports

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

        public static readonly MethodInfo AbsDecMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs(0.0M));

        public static readonly MethodInfo Abs64Method =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs(0L));

        public static readonly MethodInfo Abs32Method =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((int?)0));

        public static readonly MethodInfo Abs16Method =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((short?)0));

        public static readonly MethodInfo Abs8Method =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Abs((sbyte?)0));

        #endregion

        #region String exports

        public static readonly MethodInfo ConcatMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Concat("", ""));

        public static readonly MethodInfo ToLowerMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ToLower(""));

        public static readonly MethodInfo ToUpperMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ToUpper(""));

        public static readonly MethodInfo IndexOfMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.IndexOf("", ""));

        public static readonly MethodInfo ReverseStringMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ReverseString(""));

        public static readonly MethodInfo SubstringMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Substring("", 0, 9));

        public static readonly MethodInfo TrimMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Trim(""));

        public static readonly MethodInfo LTrimMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.LTrim(""));

        public static readonly MethodInfo RTrimMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.RTrim(""));

        public static readonly MethodInfo LengthMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Length(""));

        public static readonly MethodInfo ReplaceMethod =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Replace("", "", ""));

        #endregion

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

            return string.Concat(a, b);
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
            return a.IndexOf(b) + 1;
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

        public static string Replace(string data, string oldValue, string newValue)
        {
            if (data == null || oldValue == null || newValue == null)
            {
                return null;
            }

            return data.Replace(oldValue, newValue);
        }

        #endregion
    }
}
