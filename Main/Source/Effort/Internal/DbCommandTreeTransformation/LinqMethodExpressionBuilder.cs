// --------------------------------------------------------------------------------------------
// <copyright file="LinqMethodExpressionBuilder.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using System.Data.Common.CommandTrees;
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

            MethodInfo genericMethod = this.queryMethods.Select;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression SelectMany(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.SelectMany;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
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

            MethodInfo genericMethod = this.queryMethods.OrderBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression OrderByDescending(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.OrderByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression ThenBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.ThenBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression ThenByDescending(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.ThenByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression GroupBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.GroupBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

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

        #region Aggregation

        public Expression Count(Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Count;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Max(Expression source, LambdaExpression selector)
        {
            return this.Aggregate(source, selector, "Max");
        }

        public Expression Min(Expression source, LambdaExpression selector)
        {
            return this.Aggregate(source, selector, "Min");
        }

        public Expression Average(Expression source, LambdaExpression selector)
        {
            return this.Aggregate(source, selector, "Average");
        }

        public Expression Sum(Expression source, LambdaExpression selector)
        {
            return this.Aggregate(source, selector, "Sum");
        }

        private Expression Aggregate(Expression source, LambdaExpression selector, string name)
        {
            Type[] aggregateNativeTypes = new Type[] { typeof(int), typeof(long), typeof(double), typeof(float), typeof(decimal) };

            Type sourceType = TypeHelper.GetElementType(source.Type);
            Type selectorType = selector.Body.Type;

            // Native means, that Enumerable contains aggregate method for a specific type
            bool isNative = false;

            // Nullable types need special consideration
            if (TypeHelper.IsNullable(selectorType))
            {
                isNative = aggregateNativeTypes.Contains(selectorType.GetGenericArguments()[0]);
            }
            else
            {
                isNative = aggregateNativeTypes.Contains(selectorType);
            }

            MethodInfo method = null;

            if (isNative)
            {
                // Aggregator methods does not have generic definitions, so we have to search for each type

                // Search for "selectorType Enumerable.'name'<TSource>(IEnumerable<TSource> source, Func<TSource, selectorType> selector)"
                MethodInfo genericMethod = typeof(Enumerable)
                    .GetMethods()
                    .Where(mi =>

                        // The method name
                        mi.Name == name &&

                        // The method has single generic argument
                        // <TSource>
                        mi.GetGenericArguments().Length == 1 &&

                        // Two parameters
                        // (source, selectorType)
                        mi.GetParameters().Length == 2 &&

                        // The type of the second parameter has two generic arguments
                        // Func<TSource, selectorType>
                        mi.GetParameters()[1].ParameterType.GetGenericArguments().Length == 2 &&

                        // selectorType match
                        mi.GetParameters()[1].ParameterType.GetGenericArguments()[1] == selectorType)
                    .Single();

                method = genericMethod.MakeGenericMethod(sourceType);
            }
            else
            {
                // Search for "TResult Enumerable.'name'<TSource, TResult>(IEnumerable<TSource>, Func<TSource, TResult>)"
                MethodInfo genericMethod = typeof(Enumerable).GetMethods()
                    .Where(mi =>
                        mi.Name == name &&
                        mi.GetGenericArguments().Length == 2)
                    .Single();
                method = genericMethod.MakeGenericMethod(sourceType, selectorType);
            }

            return Expression.Call(method, source, selector);
        }

        #endregion

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
    }
}
