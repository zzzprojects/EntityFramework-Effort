using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Common;
using System.Data;
using Effort.Internal.Common;
using System.Linq.Expressions;
using NMemory.Tables;
using Effort.Internal.DbCommandTreeTransformation;
using System.Reflection;
using Effort.Provider;

namespace Effort.Internal.DbCommandActions
{
    internal class UpdateCommandAction : CommandActionBase<DbUpdateCommandTree>
    {
        protected override DbDataReader ExecuteDataReaderAction(DbUpdateCommandTree commandTree, ActionContext context)
        {
            string[] returningFields = DbCommandActionHelper.GetReturningFields(commandTree.Returning);
            IList<IDictionary<string, object>> returningEntities = new List<IDictionary<string, object>>();

            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(commandTree.Predicate, commandTree, context.DbContainer, out table);
            IQueryable entitiesToUpdate = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            Type type = TypeHelper.GetElementType(table.GetType());

            // Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

            // Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor(context.DbContainer.TypeConverter);

            // Setup context for the predicate
            ParameterExpression param = Expression.Parameter(type, "context");
            using (transform.CreateVariable(param, commandTree.Target.VariableName))
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

            IEnumerable<object> updatedEntities = DatabaseReflectionHelper.UpdateEntities(entitiesToUpdate, updater);

            foreach (object entity in updatedEntities)
            {
                Dictionary<string, object> returningEntity = DbCommandActionHelper.CreateReturningEntity(context, returningFields, entity);

                returningEntities.Add(returningEntity);
            }

            return new EffortDataReader(returningEntities, returningFields, context.DbContainer);
        }

        protected override int ExecuteNonQueryAction(DbUpdateCommandTree commandTree, ActionContext context)
        {
            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(commandTree.Predicate, commandTree, context.DbContainer, out table);
            IQueryable entitiesToUpdate = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            Type type = TypeHelper.GetElementType(table.GetType());

            //Collect the SetClause DbExpressions into a dictionary
            IDictionary<string, DbExpression> setClauses = DbCommandActionHelper.GetSetClauseExpressions(commandTree.SetClauses);

            //Collection for collection member bindings
            IList<MemberBinding> memberBindings = new List<MemberBinding>();

            DbExpressionTransformVisitor transform = new DbExpressionTransformVisitor(context.DbContainer.TypeConverter);

            // Setup context for the predicate
            ParameterExpression param = Expression.Parameter(type, "context");
            using (transform.CreateVariable(param, commandTree.Target.VariableName))
            {
                //Initialize member bindings
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

            return DatabaseReflectionHelper.UpdateEntities(entitiesToUpdate, updater).Count();
        }

        protected override object ExecuteScalarAction(DbUpdateCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }


    }
}
