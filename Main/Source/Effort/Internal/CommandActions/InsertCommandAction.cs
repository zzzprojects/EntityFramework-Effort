using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Common;
using System.Data;
using NMemory.Tables;
using System.Reflection;
using System.Linq.Expressions;
using Effort.Internal.Common;
using Effort.Provider;
using Effort.Internal.DbCommandTreeTransformation;

namespace Effort.Internal.DbCommandActions
{
    internal class InsertCommandAction : CommandActionBase<DbInsertCommandTree>
    {
        protected override DbDataReader ExecuteDataReaderAction(DbInsertCommandTree commandTree, ActionContext context)
        {
            // Find returning fields
            string[] returningFields = DbCommandActionHelper.GetReturningFields(commandTree.Returning);
            List<IDictionary<string, object>> returningValues = new List<IDictionary<string, object>>();

            // Find NMemory table
            ITable table = DbCommandActionHelper.GetTable(commandTree, context.DbContainer);

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor(context.DbContainer.TypeConverter);

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

            object entity = CreateAndInsertEntity(table, memberBindings);

            Dictionary<string, object> entityReturningValues = DbCommandActionHelper.CreateReturningEntity(context, returningFields, entity);
            returningValues.Add(entityReturningValues);

            return new EffortDataReader(returningValues.ToArray(), returningFields, context.DbContainer);
        }

        

        protected override int ExecuteNonQueryAction(DbInsertCommandTree commandTree, ActionContext context)
        {
            // Get the source table
            ITable table = DbCommandActionHelper.GetTable(commandTree, context.DbContainer);

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();
            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor(context.DbContainer.TypeConverter);

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

            CreateAndInsertEntity(table, memberBindings);

            return 1;
        }

        protected override object ExecuteScalarAction(DbInsertCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }

        private static object CreateAndInsertEntity(ITable table, IList<MemberBinding> memberBindings)
        {
            LambdaExpression expression =
               Expression.Lambda(
                   Expression.MemberInit(
                       Expression.New(table.EntityType),
                       memberBindings));

            Delegate factory = expression.Compile();

            object newEntity = factory.DynamicInvoke();

            ((IReflectionTable)table).Insert(newEntity);

            return newEntity;
        }

    }
}
