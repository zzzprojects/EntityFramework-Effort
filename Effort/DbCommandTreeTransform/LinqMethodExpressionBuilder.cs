using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Helpers;
using Effort.TypeGeneration;

namespace Effort.DbCommandTreeTransform
{
    internal class LinqMethodExpressionBuilder
    {
        private LinqMethodProvider queryMethods;

        public LinqMethodExpressionBuilder()
        {
            this.queryMethods = LinqMethodProvider.Instance;
        }

        public Expression Select(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Select;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression SelectMany(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.SelectMany;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression Where(Expression source, LambdaExpression predicate)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Where;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, Expression.Quote(predicate));
        }

        public Expression Take(Expression source, Expression count)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Take;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, count);
        }

        public Expression Skip(Expression source, Expression count)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Skip;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source, count);
        }

        public Expression OrderBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.OrderBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression OrderByDescending( Expression source, LambdaExpression selector )
        {
            Type sourceType = TypeHelper.GetEncapsulatedType( source.Type );

            MethodInfo genericMethod = this.queryMethods.OrderByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod( sourceType, selector.Body.Type );

            return Expression.Call( method, source, Expression.Quote( selector ) );
        }

        public Expression ThenBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.ThenBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression ThenByDescending(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.ThenByDescending;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression GroupBy(Expression source, LambdaExpression selector)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.GroupBy;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType, selector.Body.Type);

            return Expression.Call(method, source, Expression.Quote(selector));
        }

        public Expression Distinct(Expression source)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Distinct;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression FirstOrDefault(Expression source)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.FirstOrDefault;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression First(Expression source)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.First;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Any(Expression source)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

            MethodInfo genericMethod = this.queryMethods.Any;
            MethodInfo method = genericMethod.MakeGenericMethod(sourceType);

            return Expression.Call(method, source);
        }

        public Expression Except(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetEncapsulatedType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Except;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Intersect(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetEncapsulatedType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Intersect;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Union(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetEncapsulatedType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Union;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);

            return Expression.Call(method, first, second);
        }

        public Expression Concat(Expression first, Expression second)
        {
            Type firstType = TypeHelper.GetEncapsulatedType(first.Type);

            MethodInfo genericMethod = this.queryMethods.Concat;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType);
            
            return Expression.Call(method, first, second);

        }



        #region Aggregation

        public Expression Count(Expression source)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);

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

        private static Type[] aggregateNativeTypes = new Type[] { typeof(int), typeof(long), typeof(double), typeof(float), typeof(decimal) };

        private Expression Aggregate(Expression source, LambdaExpression selector, string name)
        {
            Type sourceType = TypeHelper.GetEncapsulatedType(source.Type);
            Type selectorType = selector.Body.Type;

            //Check if the type is "native"
            bool isNative = false;

            //Nullable types need special consideration
            if(TypeHelper.IsNullable(selectorType))
            {
                isNative = aggregateNativeTypes.Contains(selectorType.GetGenericArguments()[0]);
            }
            else
            {
                isNative = aggregateNativeTypes.Contains(selectorType);
            }

            MethodInfo method = null;


            if(isNative)
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
                        mi.GetParameters()[1]
                        .ParameterType.GetGenericArguments().Length == 2 &&
                            //selectorType match
                        mi
                        .GetParameters()[1]
                        .ParameterType.GetGenericArguments()[1] == selectorType)
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

        public Expression SelectMany(Expression first, Expression second, string name1, string name2)
        {
            Type firstType = TypeHelper.GetEncapsulatedType(first.Type);
            Type secondType = TypeHelper.GetEncapsulatedType(second.Type);

            Dictionary<string, Type> resultTypeProps = 
                new Dictionary<string, Type>
                {
                    { name1, firstType },
                    { name2, secondType }
                };

            Type anonymType = AnonymousTypeFactory.Create(resultTypeProps);

            MethodInfo genericMethod = this.queryMethods.SelectManyWithResultSelector;
            MethodInfo method = genericMethod.MakeGenericMethod(firstType, secondType, anonymType);

            Expression collectionSelector = 
                Expression.Lambda(
                    Expression.Convert(
                        second, 
                        typeof(IEnumerable<>).MakeGenericType(secondType)),
                Expression.Parameter(firstType));

            ParameterExpression p1 = Expression.Parameter(firstType);
            ParameterExpression p2 = Expression.Parameter(secondType);

            Expression constructor = Expression.New(anonymType.GetConstructors().First(), p1, p2);
            Expression resultSelector = Expression.Lambda(constructor, p1, p2);

            Expression result = Expression.Call(null, 
                method, 
                first,
                Expression.Quote(collectionSelector),
                Expression.Quote(resultSelector));

            return result;

        }
        public Expression Join(Expression left, Expression right, string name1, string name2, LambdaExpression leftKeySelector, LambdaExpression rightKeySelector, DbExpressionKind joinType)
        {
            Type rightType = TypeHelper.GetEncapsulatedType(right.Type);
            Type leftType = TypeHelper.GetEncapsulatedType(left.Type);

            Dictionary<string, Type> resultTypeProps =
                new Dictionary<string, Type>() 
                { 
                    { name1, leftType },
                    { name2, rightType }
                }; 
            
            Type anonymType = AnonymousTypeFactory.Create(resultTypeProps);

            MethodInfo genericJoin = null;

            switch(joinType)
            {
                case DbExpressionKind.InnerJoin:
                    genericJoin = this.queryMethods.Join;
                    break;
                case DbExpressionKind.LeftOuterJoin:
                    genericJoin = this.queryMethods.LeftOuterJoin;
                    break;
                case DbExpressionKind.FullOuterJoin:
                    throw new NotImplementedException();
                default:
                    throw new InvalidOperationException();
            }

            MethodInfo joinMethod = genericJoin.MakeGenericMethod(
                leftType,
                rightType,
                leftKeySelector.ReturnType,
                anonymType);


            ParameterExpression p1 = Expression.Parameter(leftType);
            ParameterExpression p2 = Expression.Parameter(rightType);

            Expression constructor = Expression.New(anonymType.GetConstructors().First(), p1, p2);
            Expression resultSelector = Expression.Lambda(constructor, p1, p2);

            Expression result = Expression.Call(null, joinMethod, left, right,
                Expression.Quote(leftKeySelector),
                Expression.Quote(rightKeySelector),
                Expression.Quote(resultSelector));
            return result;
        }


    }
}
