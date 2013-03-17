// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.GroupBy.cs" company="Effort Team">
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

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbGroupByExpression expression)
        {
            Expression source = this.Visit(expression.Input.Expression);
            Type elementType = TypeHelper.GetElementType(source.Type);

            Type resultType = edmTypeConverter.GetElementType(expression.ResultType);
            Expression result = source;

            if (expression.Keys.Count == 0)
            {
                // This is a special case
                // The DbGroupByExpression does not contain any Key element
                // There is no GroupByClause

                List<Expression> constructorArguments = new List<Expression>();

                for (int i = 0; i < expression.Aggregates.Count; i++)
                {
                    DbFunctionAggregate aggregation = expression.Aggregates[i] as DbFunctionAggregate;

                    if (aggregation == null)
                    {
                        throw new InvalidOperationException(expression.Aggregates[i].GetType().ToString() + "is not supported");
                    }

                    Expression arg = this.CreateAggregateFunction(
                        aggregation,
                        //Aggregation is executed on the source
                        expression.Input.GroupVariableName,
                        elementType,
                        source,
                        resultType.GetProperties()[0].PropertyType);

                    constructorArguments.Add(arg);
                }

                Expression aggregationResults =
                    Expression.New(
                        resultType.GetConstructors().Single(),
                        constructorArguments.ToArray(),
                        resultType.GetProperties());

                // Wrap by a SingleResult collection object
                result =
                    Expression.New(
                        typeof(SingleResult<>).MakeGenericType(resultType).GetConstructors().Single(),
                        aggregationResults);

                // Make it queryable
                result = queryMethodExpressionBuilder.AsQueryable(result);
            }
            else
            {

                // The properties of the selector form a subset of the properties of the result type
                // These properties defined first in the edm type
                PropertyInfo[] props = resultType.GetProperties();
                Dictionary<string, Type> selectorProperties = new Dictionary<string, Type>();

                // Collect the properties
                for (int i = 0; i < expression.Keys.Count; i++)
                {
                    selectorProperties.Add(props[i].Name, props[i].PropertyType);
                }

                Type selectorType = AnonymousTypeFactory.Create(selectorProperties);
                LambdaExpression selector = null;

                ParameterExpression groupParam = Expression.Parameter(elementType, expression.Input.VariableName);
                using (this.CreateVariable(groupParam, expression.Input.VariableName))
                {
                    selector =
                        Expression.Lambda(
                            this.CreateSelector(expression.Keys, selectorType),
                            groupParam);
                }

                // Build the GroupBy call expression
                result = queryMethodExpressionBuilder.GroupBy(result, selector);

                // Get IGrouping<> type
                Type groupingType = TypeHelper.GetElementType(result.Type);
                // Collect argument initiators in an array
                Expression[] groupInit = new Expression[expression.Keys.Count + expression.Aggregates.Count];

                ParameterExpression selectParam = Expression.Parameter(groupingType, "group");
                Expression keyParam = Expression.Property(selectParam, "Key");
                // Collect the Key arguments

                for (int i = 0; i < expression.Keys.Count; i++)
                {
                    groupInit[i] = Expression.Property(keyParam, props[i].Name);
                }


                // Collect the aggregate arguments
                for (int i = 0; i < expression.Aggregates.Count; i++)
                {
                    DbFunctionAggregate aggregate = expression.Aggregates[i] as DbFunctionAggregate;

                    if (aggregate == null)
                    {
                        throw new InvalidOperationException(expression.Aggregates[i].GetType().ToString() + "is not supported");
                    }

                    groupInit[expression.Keys.Count + i] =
                        this.CreateAggregateFunction(
                            aggregate,
                        // Aggregation is executed on the group
                            expression.Input.GroupVariableName,
                            elementType,
                            selectParam,
                            props[expression.Keys.Count + i].PropertyType);
                }

                selector =
                    Expression.Lambda(
                        Expression.New(
                            resultType.GetConstructors().Single(),
                            groupInit,
                            resultType.GetProperties()),
                        selectParam);

                result = queryMethodExpressionBuilder.Select(result, selector);
            }

            return result;
        }
    }
}