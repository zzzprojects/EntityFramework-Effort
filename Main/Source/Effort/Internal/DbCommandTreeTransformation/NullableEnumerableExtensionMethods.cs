// --------------------------------------------------------------------------------------------
// <copyright file="NullableEnumerableExtensionMethods.cs" company="Effort Team">
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

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Collections.Generic;

    internal static class NullableEnumerableExtensionMethods
    {
        public static decimal? Sum<TSource>(IEnumerable<TSource> source, Func<TSource, decimal?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            decimal? result = null;

            foreach (TSource item in source)
            {
                decimal? value = selector.Invoke(item);

                if (value.HasValue)
                {
                    if (!result.HasValue)
                    {
                        result = 0m;
                    }

                    result += value.Value;
                }
            }

            return result;
        }

        public static double? Sum<TSource>(IEnumerable<TSource> source, Func<TSource, double?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            double? result = null;

            foreach (TSource item in source)
            {
                double? value = selector.Invoke(item);

                if (value.HasValue)
                {
                    if (!result.HasValue)
                    {
                        result = 0d;
                    }

                    result += value.Value;
                }
            }

            return result;
        }

        public static float? Sum<TSource>(IEnumerable<TSource> source, Func<TSource, float?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            float? result = null;

            foreach (TSource item in source)
            {
                float? value = selector.Invoke(item);

                if (value.HasValue)
                {
                    if (!result.HasValue)
                    {
                        result = 0f;
                    }

                    result += value.Value;
                }
            }

            return result;
        }

        public static int? Sum<TSource>(IEnumerable<TSource> source, Func<TSource, int?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            int? result = null;

            foreach (TSource item in source)
            {
                int? value = selector.Invoke(item);

                if (value.HasValue)
                {
                    if (!result.HasValue)
                    {
                        result = 0;
                    }

                    result += value.Value;
                }
            }

            return result;
        }

        public static long? Sum<TSource>(IEnumerable<TSource> source, Func<TSource, long?> selector)
        {
            if (source == null)
            {
                throw new ArgumentNullException("source");
            }

            if (selector == null)
            {
                throw new ArgumentNullException("selector");
            }

            long? result = null;

            foreach (TSource item in source)
            {
                long? value = selector.Invoke(item);

                if (value.HasValue)
                {
                    if (!result.HasValue)
                    {
                        result = 0;
                    }

                    result += value.Value;
                }
            }

            return result;
        }
    }
}
