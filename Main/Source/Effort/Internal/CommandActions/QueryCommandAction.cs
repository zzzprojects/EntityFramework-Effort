// --------------------------------------------------------------------------------------------
// <copyright file="QueryCommandAction.cs" company="Effort Team">
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

namespace Effort.Internal.CommandActions
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
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
            visitor.TableProvider = context.DbContainer;

            // Transform command tree
            Expression queryExpression = 
                visitor.Visit(this.commandTree.Query);

            LambdaExpression query = 
                Expression.Lambda(queryExpression, Expression.Parameter(typeof(IDatabase)));

            // Create a stored procedure from the expression
            ISharedStoredProcedure procedure =
                DatabaseReflectionHelper.CreateSharedStoredProcedure(query);

            // Format the parameter values
            IDictionary<string, object> parameters =
                DbCommandActionHelper.FormatParameters(
                    context.Parameters,
                    procedure.Parameters,
                    context.DbContainer.TypeConverter);

            IEnumerable result = null;

            if (context.Transaction != null)
            {
                result = procedure.Execute(
                    context.DbContainer.Internal, 
                    parameters, 
                    context.Transaction);
            }
            else
            {
                result = procedure.Execute(
                    context.DbContainer.Internal, 
                    parameters);
            }

            List<FieldDescription> fields = GetReturningFields(this.commandTree);

            return new EffortDataReader(
                result,
                -1,
                fields.ToArray(), 
                context.DbContainer);
        }

        public object ExecuteScalar(ActionContext context)
        {
            throw new NotSupportedException();
        }

        public int ExecuteNonQuery(ActionContext context)
        {
            throw new NotSupportedException();
        }

        private static List<FieldDescription> GetReturningFields(
            DbQueryCommandTree commandTree)
        {
            List<FieldDescription> fields = new List<FieldDescription>();

            CollectionType collectionType = 
                commandTree.Query.ResultType.EdmType as CollectionType;

            RowType rowType =
                collectionType.TypeUsage.EdmType as RowType;

            foreach (EdmMember member in rowType.Members)
            {
                PrimitiveType memberType = 
                    member.TypeUsage.EdmType as PrimitiveType;

                fields.Add(new FieldDescription(member.Name, memberType.ClrEquivalentType));
            }

            return fields;
        }
    }
}
