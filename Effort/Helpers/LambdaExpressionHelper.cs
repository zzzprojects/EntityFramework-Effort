using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using MMDB.EntityFrameworkProvider.TypeGeneration;

namespace MMDB.EntityFrameworkProvider.Helpers
{
    internal static class LambdaExpressionHelper
    {
        public static LambdaExpression CreateSelectorExpression(Type sourceType, PropertyInfo[] selectorFields)
        {
            object selectorExpression = null;
            Type resultType = null;

            //Single field primary key:
            //Expression:  x => x.Field
            if (selectorFields.Length == 1)
            {
                resultType = selectorFields[0].PropertyType;

                selectorExpression =
                    typeof(LambdaExpressionHelper.WrapperMethods)
                    .GetMethod("CreateSingleFieldSelector")
                    .MakeGenericMethod(sourceType, resultType)
                    .Invoke(null, new object[] { selectorFields[0].Name });
            }
            //Multiple field primary key:
            //Expression: x => new { x.Field1, x.Field2 }
            else
            {

                //Build anonymous type
                resultType =
                    AnonymousTypeFactory.Create(
                        selectorFields.ToDictionary(
                            pi => pi.Name,
                            pi => pi.PropertyType));

                //resultType =
                //    TupleTypeFactory.Create(
                //        selectorFields.Select(pi => pi.PropertyType).ToArray());

                selectorExpression =
                    typeof(LambdaExpressionHelper.WrapperMethods)
                    .GetMethod("CreateMultipleFieldSelector")
                    .MakeGenericMethod(sourceType, resultType)
                    .Invoke(null, new object[] { selectorFields.Select(pi => pi.Name) });
            }

            return selectorExpression as LambdaExpression;
        }

        public static LambdaExpression CreateInitializerExpression(Type type, PropertyInfo[] properties)
        {
            ParameterExpression[] parameters = properties.Select(p => Expression.Parameter(p.PropertyType)).ToArray();

            MemberInitExpression memberInit =
                Expression.MemberInit(
                    Expression.New(type),
                    properties.Select((p, i) => Expression.Bind(p, parameters[i])));

            LambdaExpression lambda = Expression.Lambda(memberInit, parameters);

            return lambda;
        }

        private static class WrapperMethods
        {
            public static Expression<Func<TSource, TResult>> CreateSingleFieldSelector<TSource, TResult>(string fieldName) where TSource : class
            {
                var expressionParameter = Expression.Parameter(typeof(TSource), "x");
                return
                    Expression.Lambda<Func<TSource, TResult>>(
                        Expression.Property(
                            expressionParameter,
                            typeof(TSource),
                            fieldName),
                        expressionParameter);
            }

            public static Expression<Func<TSource, TResult>> CreateMultipleFieldSelector<TSource, TResult>(IEnumerable<string> fields) where TSource : class
            {
                var expressionParameter = Expression.Parameter(typeof(TSource), "x");

                return
                    Expression.Lambda<Func<TSource, TResult>>(
                        Expression.New(
                            typeof(TResult).GetConstructors().Single(),
                            fields.Select(f => Expression.Property(expressionParameter, f)).ToArray()
                        )
                        ,
                        expressionParameter
                    );
            }
        }
    }
}
