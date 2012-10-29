﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.DbCommandTreeTransformation;
using Effort.Provider;
using NMemory.Tables;
using NMemory.Transactions;

namespace Effort.Internal.CommandActions
{
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
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

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
            ITable table = DbCommandActionHelper.GetTable(commandTree, context.DbContainer);

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

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