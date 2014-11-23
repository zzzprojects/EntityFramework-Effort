// --------------------------------------------------------------------------------------------
// <copyright file="StringFunctions.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

    internal class StringFunctions
    {
        public static readonly MethodInfo Concat =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Concat(string.Empty, string.Empty));

        public static readonly MethodInfo Contains =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Contains(string.Empty, string.Empty));

        public static readonly MethodInfo ToLower =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ToLower(string.Empty));

        public static readonly MethodInfo ToUpper =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ToUpper(string.Empty));

        public static readonly MethodInfo IndexOf =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.IndexOf(string.Empty, string.Empty));

        public static readonly MethodInfo ReverseString =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ReverseString(string.Empty));

        public static readonly MethodInfo Substring =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Substring(string.Empty, 0, 9));

        public static readonly MethodInfo Trim =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Trim(string.Empty));

        public static readonly MethodInfo LTrim =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.LTrim(string.Empty));

        public static readonly MethodInfo RTrim =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.RTrim(string.Empty));

        public static readonly MethodInfo Left =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Left(string.Empty, 0));

        public static readonly MethodInfo Right =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Right(string.Empty, 0));

        public static readonly MethodInfo Length =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Length(string.Empty));

        public static readonly MethodInfo Replace =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.Replace(string.Empty, string.Empty, string.Empty));

        public static readonly MethodInfo StartsWith =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.StartsWith(string.Empty, string.Empty));

        public static readonly MethodInfo EndsWith =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.EndsWith(string.Empty, string.Empty));

        public static readonly MethodInfo CompareTo =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.CompareTo(string.Empty, string.Empty));

        public static readonly MethodInfo ConvertToString =
            ReflectionHelper.GetMethodInfo(() => DbFunctions.ToString(null));
    }
}
