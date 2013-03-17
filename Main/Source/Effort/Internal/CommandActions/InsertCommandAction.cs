// --------------------------------------------------------------------------------------------
// <copyright file="InsertCommandAction.cs" company="Effort Team">
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
    using System.Collections.Generic;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
    using Effort.Provider;
    using NMemory.Tables;
    using NMemory.Transactions;

    internal class InsertCommandAction : ICommandAction
    {
        private DbInsertCommandTree commandTree;

        public InsertCommandAction(DbInsertCommandTree commandTree)
        {
            this.commandTree = commandTree;
        }

        public DbDataReader ExecuteDataReader(ActionContext context)
        {
            // Find returning fields
            FieldDescription[] returningFields = DbCommandActionHelper.GetReturningFields(this.commandTree.Returning);
            List<IDictionary<string, object>> returningValues = new List<IDictionary<string, object>>();

            // Find NMemory table
            ITable table = DbCommandActionHelper.GetTable(this.commandTree, context.DbContainer);

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(this.commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            TransformVisitor transform = new TransformVisitor(context.DbContainer.TypeConverter);

            // Initialize member bindings
            foreach (PropertyInfo property in table.EntityType.GetProperties())
            {
                Expression setter = null;

                // Check if member has set clause
                if (setClauses.ContainsKey(property.Name))
                {
                    setter = transform.Visit(setClauses[property.Name]);
                }

                // If setter was found, insert it
                if (setter != null)
                {
                    // Type correction
                    setter = ExpressionHelper.CorrectType(setter, property.PropertyType);

                    // Register binding
                    memberBindings.Add(Expression.Bind(property, setter));
                }
            }

            object entity = CreateAndInsertEntity(table, memberBindings, context.Transaction);

            Dictionary<string, object> entityReturningValues = DbCommandActionHelper.CreateReturningEntity(context, returningFields, entity);
            returningValues.Add(entityReturningValues);

            return new EffortDataReader(returningValues.ToArray(), returningFields, context.DbContainer);
        }

        public int ExecuteNonQuery(ActionContext context)
        {
            // Get the source table
            ITable table = DbCommandActionHelper.GetTable(this.commandTree, context.DbContainer);

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = 
                DbCommandActionHelper.GetSetClauseExpressions(this.commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            TransformVisitor transform = new TransformVisitor(context.DbContainer.TypeConverter);

            // Initialize member bindings
            foreach (PropertyInfo property in table.EntityType.GetProperties())
            {
                Expression setter = null;

                // Check if member has set clause
                if (setClauses.ContainsKey(property.Name))
                {
                    setter = transform.Visit(setClauses[property.Name]);
                }

                // If setter was found, insert it
                if (setter != null)
                {
                    // Type correction
                    setter = ExpressionHelper.CorrectType(setter, property.PropertyType);

                    // Register binding
                    memberBindings.Add(Expression.Bind(property, setter));
                }
            }

            CreateAndInsertEntity(table, memberBindings, context.Transaction);

            return 1;
        }

        public object ExecuteScalar(ActionContext context)
        {
            throw new NotSupportedException();
        }

        private static object CreateAndInsertEntity(ITable table, IList<MemberBinding> memberBindings, Transaction transaction)
        {
            LambdaExpression expression =
               Expression.Lambda(
                   Expression.MemberInit(
                       Expression.New(table.EntityType),
                       memberBindings));

            Delegate factory = expression.Compile();

            object newEntity = factory.DynamicInvoke();

            DatabaseReflectionHelper.InsertEntity(table, newEntity, transaction);

            return newEntity;
        }
    }
}
