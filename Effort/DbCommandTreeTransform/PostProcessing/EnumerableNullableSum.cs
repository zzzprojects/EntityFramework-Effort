using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DbCommandTreeTransform.PostProcessing
{
    internal static class EnumerableNullableSum
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
