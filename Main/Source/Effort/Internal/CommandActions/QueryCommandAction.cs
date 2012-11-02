// ----------------------------------------------------------------------------------
// <copyright file="QueryCommandAction.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.CommandActions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Linq.Expressions;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
    using Effort.Internal.DbCommandTreeTransformation.PostProcessing;
    using Effort.Provider;
    using NMemory.Modularity;
    using NMemory.StoredProcedures;

    internal class QueryCommandAction : ICommandAction
    {
        private DbQueryCommandTree commandTree;

        public QueryCommandAction(DbQueryCommandTree commandTree)
        {
            this.commandTree = commandTree;
        }

        public DbDataReader ExecuteDataReader(ActionContext context)
        {
            TransformVisitor visitor = new TransformVisitor(context.DbContainer.TypeConverter);
            visitor.SetParameters(commandTree.Parameters.ToArray());
            visitor.TableProvider = context.DbContainer;

            // Transform command tree
            Expression expr = visitor.Visit(commandTree.Query);

            // Execute expression post processing
            foreach (IExpressionModifier modifier in DbCommandTreeTransformation.PostProcessing.Modifiers.GetModifiers())
            {
                expr = modifier.ModifyExpression(expr);
            }

            LambdaExpression query = Expression.Lambda(expr, Expression.Parameter(typeof(IDatabase)));

            // Create a stored procedure from the expression
            ISharedStoredProcedure procedure = DatabaseReflectionHelper.CreateSharedStoredProcedure(query);

            // Determine parameter values
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (CommandActionParameter param in context.Parameters)
            {
                string name = param.Name;
                object value = param.Value;

                // Find the description of the parameter
                ParameterDescription expectedParam = procedure.Parameters.FirstOrDefault(p => p.Name == param.Name);

                // Custom conversion
                value = context.DbContainer.TypeConverter.ConvertClrObject(value, expectedParam.Type);

                parameters.Add(name, value);
            }

            IEnumerable result = null;

            if (context.Transaction != null)
            {
                result = procedure.Execute(context.DbContainer.Internal, parameters, context.Transaction);
            }
            else
            {
                result = procedure.Execute(context.DbContainer.Internal, parameters);
            }

            List<FieldDescription> fields = GetReturningFields(commandTree);

            return new EffortDataReader(result, fields.ToArray(), context.DbContainer);
        }

        public object ExecuteScalar(ActionContext context)
        {
            throw new NotSupportedException();
        }

        public int ExecuteNonQuery(ActionContext context)
        {
            throw new NotSupportedException();
        }

        private static List<FieldDescription> GetReturningFields(DbQueryCommandTree commandTree)
        {
            List<FieldDescription> fields = new List<FieldDescription>();

            CollectionType collectionType = commandTree.Query.ResultType.EdmType as CollectionType;
            RowType rowType = collectionType.TypeUsage.EdmType as RowType;

            foreach (EdmMember member in rowType.Members)
            {
                PrimitiveType memberType = member.TypeUsage.EdmType as PrimitiveType;
                fields.Add(new FieldDescription(member.Name, memberType.ClrEquivalentType));
            }
            return fields;
        }

    }
}
