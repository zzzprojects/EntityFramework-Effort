// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.TypeConversion;

    internal partial class TransformVisitor : DbExpressionVisitor<Expression>
    {
        private Dictionary<string, Tuple<TypeUsage, int>> parameters;

        private ITableProvider tableProvider;
        private IDbMethodProvider methodProvider;

        private LinqMethodExpressionBuilder queryMethodExpressionBuilder;
        private CanonicalFunctionMapper functionMapper;

        private ITypeConverter converter;
        private EdmTypeConverter edmTypeConverter;

        private VariableCollection currentVariables;

        public TransformVisitor(ITypeConverter converter)
        {
            this.converter = converter;
            this.edmTypeConverter = new EdmTypeConverter(converter);

            this.queryMethodExpressionBuilder = new LinqMethodExpressionBuilder();
            this.currentVariables = new VariableCollection();
            this.parameters = new Dictionary<string, Tuple<TypeUsage, int>>();

            this.functionMapper = new CanonicalFunctionMapper(converter);
            this.methodProvider = new Effort.Internal.DbManagement.DbMethodProvider();
        }

        public ITableProvider TableProvider
        {
            set { this.tableProvider = value; }
            get { return this.tableProvider; }
        }

        public IDbMethodProvider MethodProvider
        {
            set { this.methodProvider = value; }
            get { return this.methodProvider; }
        }

        #region Context management

        public VariableHandler CreateVariable(Expression contextParam, string name)
        {
            Variable context = new Variable();
            context.Expression = contextParam;
            context.Name = name;

            VariableHandler handler = new VariableHandler(context, this.currentVariables);

            return handler;
        }

        #endregion

        public override Expression Visit(DbExpression expression)
        {
            //Expression recalls the specific Visit method
            return expression.Accept(this);
        }

        private Expression Visit(DbExpression expression, Type requiredType)
        {
            Expression result = this.Visit(expression);

            if (result.Type != requiredType)
            {
                result = Expression.Convert(result, requiredType);
            }

            return result;
        }

        


        private Expression CreateAggregateFunction(DbFunctionAggregate functionAggregate, string sourceVariableName, Type sourceType, Expression sourceGroup, Type resultType)
        {
            Expression result = null;

            //More the one aggregate argument is not supported
            if (functionAggregate.Arguments.Count > 1)
            {
                throw new InvalidOperationException("DbFunctionAggreate contains more than one argument");
            }


            LambdaExpression aggregateSelector = null;
            // Count does not have selector
            if (functionAggregate.Arguments.Count == 1)
            {
                // Build the selector of the current aggregate

                ParameterExpression aggregateContext = Expression.Parameter(sourceType, sourceVariableName);
                using (this.CreateVariable(aggregateContext, sourceVariableName))
                {
                    aggregateSelector =
                        Expression.Lambda(
                            this.Visit(functionAggregate.Arguments[0]),
                            aggregateContext);
                }
            }

            //Create Expression Call
            switch (functionAggregate.Function.Name)
            {
                case "Count":
                    result = queryMethodExpressionBuilder.Count(sourceGroup);
                    break;

                case "Max":
                    result = queryMethodExpressionBuilder.Max(sourceGroup, aggregateSelector);
                    break;

                case "Min":
                    result = queryMethodExpressionBuilder.Min(sourceGroup, aggregateSelector);
                    break;

                case "Avg":
                    result = queryMethodExpressionBuilder.Average(sourceGroup, aggregateSelector);
                    break;

                case "Sum":
                    result = queryMethodExpressionBuilder.Sum(sourceGroup, aggregateSelector);
                    break;

                default:
                    throw new NotSupportedException(functionAggregate.Function.Name + " is not a not supported DbFunctionAggregate");
            }

            //Type unify
            if (resultType != null && result.Type != resultType)
            {
                result = Expression.Convert(result, resultType);
            }

            return result;
        }

        //tamas
        

        private Expression CreateSelector(IList<DbExpression> arguments, Type resultType)
        {
            List<Expression> constructorExpressions = new List<Expression>();
            PropertyInfo[] props = resultType.GetProperties();


            if (resultType.IsArray)
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    Expression argumentExpression = this.Visit(arguments[i]);

                    constructorExpressions.Add(argumentExpression);
                }

                Expression array =
                    Expression.NewArrayInit(
                        resultType.GetElementType(),
                        constructorExpressions.ToArray());

                Type listType = typeof(List<>).MakeGenericType(resultType.GetElementType());

                var constr = listType
                    .GetConstructors()
                    .Where(c =>
                        c.GetParameters().Length == 1 &&
                        c.GetParameters()[0].ParameterType.IsGenericType &&
                        c.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                    .First();

                Expression list = Expression.New(constr, array);

                Expression queryable =
                    Expression.Call(
                        typeof(Queryable)
                            .GetMethods().Where(m => m.Name == "AsQueryable" && m.IsGenericMethod).Single()
                            .MakeGenericMethod(listType.GetGenericArguments()[0]),
                        list);

                return queryable;
            }
            else
            {
                for (int i = 0; i < arguments.Count; i++)
                {
                    Expression argumentExpression = this.Visit(arguments[i], props[i].PropertyType);

                    constructorExpressions.Add(argumentExpression);
                }

                return Expression.New(
                    resultType.GetConstructors().Single(),
                    constructorExpressions,
                    resultType.GetProperties());
            }
        }
    }
}
