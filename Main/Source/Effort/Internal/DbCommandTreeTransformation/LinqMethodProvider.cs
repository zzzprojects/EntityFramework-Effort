// --------------------------------------------------------------------------------------------
// <copyright file="LinqMethodProvider.cs" company="Effort Team">
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
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;
    using Effort.Internal.Common;

    internal class LinqMethodProvider
    {
        public static readonly LinqMethodProvider Instance =
            new LinqMethodProvider();


        private FastLazy<MethodInfo> select;
        private FastLazy<MethodInfo> selectMany;
        private FastLazy<MethodInfo> selectManyWithResultSelector;

        private FastLazy<MethodInfo> count;
        private FastLazy<MethodInfo> longCount;
        private FastLazy<MethodInfo> where;
        private FastLazy<MethodInfo> take;
        private FastLazy<MethodInfo> skip;
        private FastLazy<MethodInfo> groupBy;

        private FastLazy<MethodInfo> orderBy;
        private FastLazy<MethodInfo> orderByDescending;
        private FastLazy<MethodInfo> thenBy;
        private FastLazy<MethodInfo> thenByDescending;

        private FastLazy<MethodInfo> firstOrDefault;
        private FastLazy<MethodInfo> first;
        private FastLazy<MethodInfo> any;
        private FastLazy<MethodInfo> defaultIfEmpty;
        private FastLazy<MethodInfo> asQueryable;

        private FastLazy<MethodInfo> distinct;
        private FastLazy<MethodInfo> except;
        private FastLazy<MethodInfo> intersect;
        private FastLazy<MethodInfo> union;
        private FastLazy<MethodInfo> concat;

        private MethodInfoGroup sum;
        private MethodInfoGroup min;
        private MethodInfoGroup max;
        private MethodInfoGroup average;

        private FastLazy<MethodInfo> minGeneric;
        private FastLazy<MethodInfo> maxGeneric;
        private FastLazy<MethodInfo> averageGeneric;

        /// <summary>
        /// Prevents a default instance of the <see cref="LinqMethodProvider" /> class from
        /// being created.
        /// </summary>
        private LinqMethodProvider()
        {
            this.select = CreateLazy(CreateSelect);
            this.selectMany = CreateLazy(CreateSelectMany);
            this.selectManyWithResultSelector = CreateLazy(CreateSelectManyWithResultSelector);

            this.count = CreateLazy(CreateCount);
            this.longCount = CreateLazy(CreateLongCount);
            this.where = CreateLazy(CreateWhere);
            this.take = CreateLazy(CreateTake);
            this.skip = CreateLazy(CreateSkip);
            this.groupBy = CreateLazy(CreateGroupBy);

            this.orderBy = CreateLazy(CreateOrderBy);
            this.orderByDescending = CreateLazy(CreateOrderByDescending);
            this.thenBy = CreateLazy(CreateThenBy);
            this.thenByDescending = CreateLazy(CreateThenByDescending);

            this.first = CreateLazy(CreateFirst);
            this.firstOrDefault = CreateLazy(CreateFirstOrDefault);
            this.any = CreateLazy(CreateAny);
            this.defaultIfEmpty = CreateLazy(CreateDefaultIfEmpty);
            this.asQueryable = CreateLazy(CreateAsQueryable);

            this.distinct = CreateLazy(CreateDistinct);
            this.except = CreateLazy(CreateExcept);
            this.intersect = CreateLazy(CreateIntersect);
            this.union = CreateLazy(CreateUnion);
            this.concat = CreateLazy(CreateConcat);

            this.sum = new MethodInfoGroup(
                Tuple.Create(typeof(int), CreateLazy(CreateSumInt)),
                Tuple.Create(typeof(int?), CreateLazy(CreateSumNInt)),
                Tuple.Create(typeof(long), CreateLazy(CreateSumLong)),
                Tuple.Create(typeof(long?), CreateLazy(CreateSumNLong)),
                Tuple.Create(typeof(float), CreateLazy(CreateSumFloat)),
                Tuple.Create(typeof(float?), CreateLazy(CreateSumNFloat)),
                Tuple.Create(typeof(double), CreateLazy(CreateSumDouble)),
                Tuple.Create(typeof(double?), CreateLazy(CreateSumNDouble)),
                Tuple.Create(typeof(decimal), CreateLazy(CreateSumDecimal)),
                Tuple.Create(typeof(decimal?), CreateLazy(CreateSumNDecimal)));

            this.min = new MethodInfoGroup(
                Tuple.Create(typeof(int), CreateLazy(CreateMinInt)),
                Tuple.Create(typeof(int?), CreateLazy(CreateMinNInt)),
                Tuple.Create(typeof(long), CreateLazy(CreateMinLong)),
                Tuple.Create(typeof(long?), CreateLazy(CreateMinNLong)),
                Tuple.Create(typeof(float), CreateLazy(CreateMinFloat)),
                Tuple.Create(typeof(float?), CreateLazy(CreateMinNFloat)),
                Tuple.Create(typeof(double), CreateLazy(CreateMinDouble)),
                Tuple.Create(typeof(double?), CreateLazy(CreateMinNDouble)),
                Tuple.Create(typeof(decimal), CreateLazy(CreateMinDecimal)),
                Tuple.Create(typeof(decimal?), CreateLazy(CreateMinNDecimal)));

            this.max = new MethodInfoGroup(
                Tuple.Create(typeof(int), CreateLazy(CreateMaxInt)),
                Tuple.Create(typeof(int?), CreateLazy(CreateMaxNInt)),
                Tuple.Create(typeof(long), CreateLazy(CreateMaxLong)),
                Tuple.Create(typeof(long?), CreateLazy(CreateMaxNLong)),
                Tuple.Create(typeof(float), CreateLazy(CreateMaxFloat)),
                Tuple.Create(typeof(float?), CreateLazy(CreateMaxNFloat)),
                Tuple.Create(typeof(double), CreateLazy(CreateMaxDouble)),
                Tuple.Create(typeof(double?), CreateLazy(CreateMaxNDouble)),
                Tuple.Create(typeof(decimal), CreateLazy(CreateMaxDecimal)),
                Tuple.Create(typeof(decimal?), CreateLazy(CreateMaxNDecimal)));

            this.average = new MethodInfoGroup(
                Tuple.Create(typeof(int), CreateLazy(CreateAvgInt)),
                Tuple.Create(typeof(int?), CreateLazy(CreateAvgNInt)),
                Tuple.Create(typeof(long), CreateLazy(CreateAvgLong)),
                Tuple.Create(typeof(long?), CreateLazy(CreateAvgNLong)),
                Tuple.Create(typeof(float), CreateLazy(CreateAvgFloat)),
                Tuple.Create(typeof(float?), CreateLazy(CreateAvgNFloat)),
                Tuple.Create(typeof(double), CreateLazy(CreateAvgDouble)),
                Tuple.Create(typeof(double?), CreateLazy(CreateAvgNDouble)),
                Tuple.Create(typeof(decimal), CreateLazy(CreateAvgDecimal)),
                Tuple.Create(typeof(decimal?), CreateLazy(CreateAvgNDecimal)));

            this.minGeneric = CreateLazy(CreateMinGeneric);
            this.maxGeneric = CreateLazy(CreateMaxGeneric);
            this.averageGeneric = CreateLazy(CreateAvgGeneric);
        }

        #region MethodInfo provider properties

        public MethodInfo Select
        {
            get { return this.select.Value; }
        }

        public MethodInfo SelectMany
        {
            get { return this.selectMany.Value; }
        }

        public MethodInfo SelectManyWithResultSelector
        {
            get { return this.selectManyWithResultSelector.Value; }
        }

        public MethodInfo Count
        {
            get { return this.count.Value; }
        }

        public MethodInfo LongCount
        {
            get { return this.longCount.Value; }
        }

        public MethodInfo Where
        {
            get { return this.where.Value; }
        }

        public MethodInfo Take
        {
            get { return this.take.Value; }
        }

        public MethodInfo Skip
        {
            get { return this.skip.Value; }
        }

        public MethodInfo GroupBy
        {
            get { return this.groupBy.Value; }
        }

        public MethodInfo OrderBy
        {
            get { return this.orderBy.Value; }
        }

        public MethodInfo OrderByDescending
        {
            get { return this.orderByDescending.Value; }
        }

        public MethodInfo ThenBy
        {
            get { return this.thenBy.Value; }
        }

        public MethodInfo ThenByDescending
        {
            get { return this.thenByDescending.Value; }
        }

        public MethodInfo First
        {
            get { return this.first.Value; }
        }

        public MethodInfo FirstOrDefault
        {
            get { return this.firstOrDefault.Value; }
        }

        public MethodInfo Any
        {
            get { return this.any.Value; }
        }

        public MethodInfo AsQueryable 
        {
            get { return this.asQueryable.Value; }
        }

        public MethodInfo DefaultIfEmpty
        {
            get { return this.defaultIfEmpty.Value; }
        }

        public MethodInfo Distinct
        {
            get { return this.distinct.Value; }
        }

        public MethodInfo Except
        {
            get { return this.except.Value; }
        }

        public MethodInfo Intersect 
        {
            get { return this.intersect.Value; }
        }

        public MethodInfo Union
        {
            get { return this.union.Value; }
        }

        public MethodInfo Concat
        {
            get { return this.concat.Value; }
        }

        public MethodInfoGroup Sum
        {
            get { return this.sum; }
        }

        public MethodInfoGroup Min
        {
            get { return this.min; }
        }

        public MethodInfoGroup Max
        {
            get { return this.max; }
        }

        public MethodInfoGroup Average
        {
            get { return this.average; }
        }

        public MethodInfo MinGeneric
        {
            get { return this.minGeneric.Value; }
        }

        public MethodInfo MaxGeneric
        {
            get { return this.maxGeneric.Value; }
        }

        public MethodInfo AverageGeneric
        {
            get { return this.averageGeneric.Value; }
        }


        #endregion

        #region MethodInfo factories

        private static MethodInfo CreateSelect()
        {
            return GetMethod(x => x.Select(e => e));
        }

        private static MethodInfo CreateSelectMany()
        {
            return GetMethod(x => x.SelectMany(e => Enumerable.Empty<object>()));
        }

        private static MethodInfo CreateSelectManyWithResultSelector()
        {
            return GetMethod(l => 
                l.SelectMany(
                    r => Enumerable.Empty<object>(), 
                    (xl, xr) => new object()));
        }

        private static MethodInfo CreateJoin()
        {
            return GetMethod(x => 
                x.Join(
                    Enumerable.Empty<object>(), 
                    l => new object(), 
                    r => new object(), 
                    (l, r) => new object()));
        }

        private static MethodInfo CreateCount()
        {
            // This needs Enumerable method, because of IGrouping
            return GetMethod(q => Enumerable.Count(q));
        }

        private static MethodInfo CreateLongCount()
        {
            // This needs Enumerable method, because of IGrouping
            return GetMethod(q => Enumerable.LongCount(q));
        }

        private static MethodInfo CreateWhere()
        {
            return GetMethod(x => x.Where(e => true));
        }

        private static MethodInfo CreateTake()
        {
            return GetMethod(x => x.Take(0));
        }

        private static MethodInfo CreateSkip()
        {
            return GetMethod(x => x.Skip(0));
        }

        private static MethodInfo CreateGroupBy()
        {
            return GetMethod(x => x.GroupBy(e => e));
        }

        private static MethodInfo CreateOrderBy()
        { 
            return GetMethod(x => x.OrderBy(e => e));
        }

        private static MethodInfo CreateOrderByDescending()
        {
            return GetMethod(x => x.OrderByDescending(e => e));
        }

        private static MethodInfo CreateThenBy()
        {
            return GetOrderedMethod(x => x.ThenBy(e => e));
        }

        private static MethodInfo CreateThenByDescending()
        {
            return GetOrderedMethod(x => x.ThenByDescending(e => e));
        }

        private static MethodInfo CreateFirst()
        {
            return GetMethod(x => x.First());
        }

        private static MethodInfo CreateFirstOrDefault()
        {
            return GetMethod(x => x.FirstOrDefault());
        }

        private static MethodInfo CreateAny()
        {
            return GetMethod(x => x.Any());
        }

        private static MethodInfo CreateDefaultIfEmpty()
        {
            return GetMethod(x => x.DefaultIfEmpty());
        }

        private static MethodInfo CreateAsQueryable()
        {
            return GetMethod(x => x.AsQueryable());
        }

        private static MethodInfo CreateDistinct()
        {
            return GetMethod(x => x.Distinct());
        }

        private static MethodInfo CreateExcept()
        {
            return GetMethod(x => x.Except(Enumerable.Empty<object>()));
        }

        private static MethodInfo CreateConcat()
        {
            return GetMethod(x => x.Concat(Enumerable.Empty<object>()));
        }

        private static MethodInfo CreateUnion()
        {
            return GetMethod(x => x.Union(Enumerable.Empty<object>()));
        }

        private static MethodInfo CreateIntersect()
        {
            return GetMethod(x => x.Intersect(Enumerable.Empty<object>()));
        }

        #endregion

        #region Aggregation MethodInfo factories

        private static MethodInfo CreateSumInt()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (int)_));
        }

        private static MethodInfo CreateSumNInt()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (int?)_));
        }

        private static MethodInfo CreateSumLong()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (long)_));
        }

        private static MethodInfo CreateSumNLong()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (long?)_));
        }

        private static MethodInfo CreateSumFloat()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (float)_));
        }

        private static MethodInfo CreateSumNFloat()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (float?)_));
        }

        private static MethodInfo CreateSumDouble()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (double)_));
        }

        private static MethodInfo CreateSumNDouble()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (double?)_));
        }

        private static MethodInfo CreateSumDecimal()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (decimal)_));
        }

        private static MethodInfo CreateSumNDecimal()
        {
            return GetMethod(x => Enumerable.Sum<object>(x, _ => (decimal?)_));
        }

        private static MethodInfo CreateMinGeneric()
        {
            return GetMethod(x => Enumerable.Min<object, object>(x, _ => null));
        }

        private static MethodInfo CreateMinInt()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (int)_));
        }

        private static MethodInfo CreateMinNInt()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (int?)_));
        }

        private static MethodInfo CreateMinLong()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (long)_));
        }

        private static MethodInfo CreateMinNLong()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (long?)_));
        }

        private static MethodInfo CreateMinFloat()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (float)_));
        }

        private static MethodInfo CreateMinNFloat()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (float?)_));
        }

        private static MethodInfo CreateMinDouble()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (double)_));
        }

        private static MethodInfo CreateMinNDouble()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (double?)_));
        }

        private static MethodInfo CreateMinDecimal()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (decimal)_));
        }

        private static MethodInfo CreateMinNDecimal()
        {
            return GetMethod(x => Enumerable.Min<object>(x, _ => (decimal?)_));
        }

        private static MethodInfo CreateMaxGeneric()
        {
            return GetMethod(x => Enumerable.Max<object, object>(x, _ => null));
        }

        private static MethodInfo CreateMaxInt()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (int)_));
        }

        private static MethodInfo CreateMaxNInt()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (int?)_));
        }

        private static MethodInfo CreateMaxLong()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (long)_));
        }

        private static MethodInfo CreateMaxNLong()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (long?)_));
        }

        private static MethodInfo CreateMaxFloat()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (float)_));
        }

        private static MethodInfo CreateMaxNFloat()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (float?)_));
        }

        private static MethodInfo CreateMaxDouble()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (double)_));
        }

        private static MethodInfo CreateMaxNDouble()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (double?)_));
        }

        private static MethodInfo CreateMaxDecimal()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (decimal)_));
        }

        private static MethodInfo CreateMaxNDecimal()
        {
            return GetMethod(x => Enumerable.Max<object>(x, _ => (decimal?)_));
        }

        private static MethodInfo CreateAvgGeneric()
        {
            return GetMethod(x => Enumerable.Max<object, object>(x, _ => null));
        }

        private static MethodInfo CreateAvgInt()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (int)_));
        }

        private static MethodInfo CreateAvgNInt()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (int?)_));
        }

        private static MethodInfo CreateAvgLong()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (long)_));
        }

        private static MethodInfo CreateAvgNLong()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (long?)_));
        }

        private static MethodInfo CreateAvgFloat()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (float)_));
        }

        private static MethodInfo CreateAvgNFloat()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (float?)_));
        }

        private static MethodInfo CreateAvgDouble()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (double)_));
        }

        private static MethodInfo CreateAvgNDouble()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (double?)_));
        }

        private static MethodInfo CreateAvgDecimal()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (decimal)_));
        }

        private static MethodInfo CreateAvgNDecimal()
        {
            return GetMethod(x => Enumerable.Average<object>(x, _ => (decimal?)_));
        }

        #endregion

        #region MethodInfo factory helpers

        private static FastLazy<MethodInfo> CreateLazy(
            Func<MethodInfo> factory)
        {
            return new FastLazy<MethodInfo>(factory);
        }

        private static MethodInfo GetMethod<T>(
            Expression<Func<IQueryable<object>, T>> function)
        {
            MethodInfo result = ReflectionHelper.GetMethodInfo(function);

            return result.GetGenericMethodDefinition();
        }

        private static MethodInfo GetOrderedMethod<T>(
            Expression<Func<IOrderedQueryable<object>, T>> function)
        {
            MethodInfo result = ReflectionHelper.GetMethodInfo(function);

            return result.GetGenericMethodDefinition();
        }

        #endregion
    }
}
