// --------------------------------------------------------------------------------------------
// <copyright file="LinqMethodExpressionBuilder.cs" company="Effort Team">
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
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.TypeGeneration;

    internal class LinqMethodExpressionBuilder
    {
        private LinqMethodProvider queryMethods;

        public LinqMethodExpressionBuilder()
        {
            this.queryMethods = LinqMethodProvider.Instance;
        }

        public Expression Select(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.Select;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression SelectMany(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.SelectMany;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression SelectMany(
            Expression first,
            LambdaExpression collectionSelector,
            LambdaExpression resultSelector)
        {
            Type firstType = TypeHelper.GetElementType(first.Type);
            Type collectionType = TypeHelper.GetElementType(collectionSelector.Body.Type);

            Type resultType = resultSelector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.SelectManyWithResultSelector;

            MethodInfo method =
                genericMethod.MakeGenericMethod(firstType, collectionType, resultType);

            return Expression.Call(
                method,
                first,
                Expression.Quote(collectionSelector),
                Expression.Quote(resultSelector));
        }

        public Expression Where(Expression source, LambdaExpression predicate)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Where;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, Expression.Quote(predicate));
        }

        public Expression Take(Expression source, Expression count)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Take;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, count);
        }

        public Expression Skip(Expression source, Expression count)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Skip;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, count);
        }

        public Expression OrderBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.OrderBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression OrderByDescending(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.OrderByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression ThenBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.ThenBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression ThenByDescending(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.ThenByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression GroupBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = this.queryMethods.GroupBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selectorType);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression Distinct(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Distinct;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression FirstOrDefault(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.FirstOrDefault;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression First(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.First;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Any(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Any;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression DefaultIfEmpty(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            MethodInfo genericMethod = this.queryMethods.DefaultIfEmpty;

            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression AsQueryable(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            MethodInfo genericMethod = this.queryMethods.AsQueryable;

            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Except(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetElementType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Except;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Intersect(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetElementType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Intersect;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Union(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetElementType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Union;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Concat(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetElementType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Concat;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Count(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Count;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression LongCount(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Count;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Max(Expression source, LambdaExpression selector)
        {
            MethodInfoGroup group = this.queryMethods.Max;
            Func<MethodInfo> generic = () => this.queryMethods.MaxGeneric;

            MethodInfo method = GetAggregationMethod(source, selector, group, generic);

            return Expression.Call(method, source, selector);
        }

        public Expression Min(Expression source, LambdaExpression selector)
        {
            MethodInfoGroup group = this.queryMethods.Min;
            Func<MethodInfo> generic = () => this.queryMethods.MinGeneric;

            MethodInfo method = GetAggregationMethod(source, selector, group, generic);

            return Expression.Call(method, source, selector);
        }

        public Expression Average(Expression source, LambdaExpression selector)
        {
            MethodInfoGroup group = this.queryMethods.Average;
            Func<MethodInfo> generic = () => this.queryMethods.AverageGeneric;

            MethodInfo method = GetAggregationMethod(source, selector, group, generic);

            return Expression.Call(method, source, selector);
        }

        public Expression Sum(Expression source, LambdaExpression selector)
        {
            MethodInfoGroup group = this.queryMethods.Sum;

            MethodInfo method = GetAggregationMethod(source, selector, group, null);

            return Expression.Call(method, source, selector);
        }

        private static MethodInfo GetAggregationMethod(
            Expression source,
            LambdaExpression selector,
            MethodInfoGroup group,
            Func<MethodInfo> generic)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            MethodInfo genericMethod = group[selectorType];
            MethodInfo method = null;

            if (genericMethod == null)
            {
                genericMethod = generic.Invoke();
                method = genericMethod.MakeGenericMethod(sourceType, selectorType);
            }
            else
            {
                method = genericMethod.MakeGenericMethod(sourceType);
            }

            return method;
        }
    }
}
