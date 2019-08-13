// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.cs" company="Effort Team">
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
    using Effort.Internal.DbManagement;
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.TypeConversion;
    using NMemory.Indexes;
    using Effort.Internal.DbManagement.Engine.Services;
    using Effort.Internal.TypeGeneration;

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
        private DbContainer container;

        public TransformVisitor(DbContainer container)
        {
            this.container = container;
            this.converter = container.TypeConverter;
            this.edmTypeConverter = new EdmTypeConverter(converter);

            this.queryMethodExpressionBuilder = new LinqMethodExpressionBuilder();
            this.currentVariables = new VariableCollection();
            this.parameters = new Dictionary<string, Tuple<TypeUsage, int>>();

            this.functionMapper = new CanonicalFunctionMapper(converter, container);
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

                case "BigCount":
                    result = queryMethodExpressionBuilder.LongCount(sourceGroup);
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

        private Expression[] VisitExpressions(IList<DbExpression> expressions)
        {
            Expression[] result = new Expression[expressions.Count];

            for (int i = 0; i < expressions.Count; i++)
            {
                result[i] = this.Visit(expressions[i]);
            }

            return result;
        }

        private Expression CreateSelector(Expression[] arguments, Type resultType)
        {
            if (resultType.IsArray)
            {
                Expression array =
                    Expression.NewArrayInit(
                        resultType.GetElementType(),
                        arguments);

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
            else if (typeof(DataRow).IsAssignableFrom(resultType))
            {
                IKeyInfoHelper helper = new DataRowKeyInfoHelper(resultType);

                return helper.CreateKeyFactoryExpression(arguments);
            }

            throw new InvalidOperationException();
        }
    }
}
