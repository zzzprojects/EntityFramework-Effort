// --------------------------------------------------------------------------------------------
// <copyright file="UpdateCommandAction.cs" company="Effort Team">
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
    using System.Data.Common.CommandTrees;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
    using Effort.Provider;
    using NMemory.Tables;

    internal class UpdateCommandAction : ICommandAction
    {
        private DbUpdateCommandTree commandTree;

        public UpdateCommandAction(DbUpdateCommandTree commandTree)
        {
            this.commandTree = commandTree;
        }

        public DbDataReader ExecuteDataReader(ActionContext context)
        {
            FieldDescription[] returningFields = DbCommandActionHelper.GetReturningFields(this.commandTree.Returning);
            IList<IDictionary<string, object>> returningEntities = new List<IDictionary<string, object>>();

            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(this.commandTree.Predicate, this.commandTree, context.DbContainer, out table);
            IQueryable entitiesToUpdate = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            Type type = TypeHelper.GetElementType(table.GetType());

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(this.commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            TransformVisitor transform = new TransformVisitor(context.DbContainer.TypeConverter);

            // Setup context for the predicate
            ParameterExpression param = Expression.Parameter(type, "context");
            using (transform.CreateVariable(param, this.commandTree.Target.VariableName))
            {
                // Initialize member bindings
                foreach (PropertyInfo property in type.GetProperties())
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

                        memberBindings.Add(Expression.Bind(property, setter));
                    }
                }
            }

            Expression updater =
                Expression.Lambda(
                    Expression.MemberInit(Expression.New(type), memberBindings),
                    param);

            IEnumerable<object> updatedEntities = DatabaseReflectionHelper.UpdateEntities(entitiesToUpdate, updater, context.Transaction);

            foreach (object entity in updatedEntities)
            {
                Dictionary<string, object> returningEntity = DbCommandActionHelper.CreateReturningEntity(context, returningFields, entity);

                returningEntities.Add(returningEntity);
            }

            return new EffortDataReader(returningEntities, returningFields, context.DbContainer);
        }

        public int ExecuteNonQuery(ActionContext context)
        {
            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(this.commandTree.Predicate, this.commandTree, context.DbContainer, out table);
            IQueryable entitiesToUpdate = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            Type type = TypeHelper.GetElementType(table.GetType());

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(this.commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            TransformVisitor transform = new TransformVisitor(context.DbContainer.TypeConverter);

            // Setup context for the predicate
            ParameterExpression param = Expression.Parameter(type, "context");
            using (transform.CreateVariable(param, this.commandTree.Target.VariableName))
            {
                // Initialize member bindings
                foreach (PropertyInfo property in type.GetProperties())
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

                        memberBindings.Add(Expression.Bind(property, setter));
                    }
                }
            }

            Expression updater =
                Expression.Lambda(
                    Expression.MemberInit(Expression.New(type), memberBindings),
                    param);

            return DatabaseReflectionHelper.UpdateEntities(entitiesToUpdate, updater, context.Transaction).Count();
        }

        public object ExecuteScalar(ActionContext context)
        {
            throw new NotSupportedException();
        }
    }
}
