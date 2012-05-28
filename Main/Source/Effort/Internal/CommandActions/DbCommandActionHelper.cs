using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using NMemory.Tables;
using Effort.Internal.DbManagement;
using Effort.Internal.DbCommandTreeTransformation;
using System.Linq.Expressions;
using Effort.Internal.Common;

namespace Effort.Internal.DbCommandActions
{
    internal static class DbCommandActionHelper
    {
        public static string[] GetReturningFields(DbExpression returning)
        {
            // Find the returning properties
            DbNewInstanceExpression returnExpression = returning as DbNewInstanceExpression;

            if (returnExpression == null)
            {
                throw new NotSupportedException("The type of the Returning properties is not DbNewInstanceExpression");
            }

            List<string> result = new List<string>();

            // Add the returning property names
            foreach (DbPropertyExpression propertyExpression in returnExpression.Arguments)
            {
                result.Add(propertyExpression.Property.Name);
            }

            return result.ToArray();
        }

        public static ITable GetTable(DbModificationCommandTree commandTree, DbContainer dbContainer)
        {
            DbScanExpression source = commandTree.Target.Expression as DbScanExpression;

            if (source == null)
            {
                throw new NotSupportedException("The type of the Target property is not DbScanExpression");
            }

            return dbContainer.Internal.GetTable(source.Target.Name);
        }

        public static IDictionary<string, DbExpression> GetSetClauseExpressions(IList<DbModificationClause> clauses)
        {
            IDictionary<string, DbExpression> result = new Dictionary<string, DbExpression>();

            foreach (DbSetClause setClause in clauses.Cast<DbSetClause>())
            {
                DbPropertyExpression property = setClause.Property as DbPropertyExpression;

                if (property == null)
                {
                    throw new NotSupportedException(setClause.Property.ExpressionKind.ToString() + " is not supported");
                }

                result.Add(property.Property.Name, setClause.Value);
            }

            return result;
        }


        public static Expression GetEnumeratorExpression(DbExpression predicate, DbModificationCommandTree commandTree, DbContainer dbContainer, out ITable table)
        {
            DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor(dbContainer.TypeConverter);
            visitor.SetParameters(commandTree.Parameters.ToArray());
            visitor.TableProvider = dbContainer;

            // Get the source expression
            ConstantExpression source = visitor.Visit(commandTree.Target.Expression) as ConstantExpression;

            // This should be a constant expression
            if (source == null)
            {
                throw new InvalidOperationException();
            }

            table = source.Value as ITable;

            // Get the the type of the elements of the table
            Type elementType = TypeHelper.GetElementType(source.Type);

            // Create context
            ParameterExpression context = Expression.Parameter(elementType, "context");
            using (visitor.CreateVariable(context, commandTree.Target.VariableName))
            {
                // Create the predicate expression
                LambdaExpression predicateExpression =
                    Expression.Lambda(
                        visitor.Visit(predicate),
                        context);

                // Create Where expression
                LinqMethodExpressionBuilder queryMethodBuilder = new LinqMethodExpressionBuilder();

                Expression result = queryMethodBuilder.Where(source, predicateExpression);

                ParameterExpression[] parameterExpressions = visitor.GetParameterExpressions();

                if (parameterExpressions.Length > 0)
                {
                    result = Expression.Lambda(result, parameterExpressions);
                }

                return result;
            }
        }

        public static Dictionary<string, object> CreateReturningEntity(ActionContext context, string[] returningFields, object entity)
        {
            Dictionary<string, object> entityReturningValues = new Dictionary<string, object>();

            for (int i = 0; i < returningFields.Length; i++)
            {
                string property = returningFields[i];

                object value = entity.GetType().GetProperty(property).GetValue(entity, null);

                entityReturningValues[property] = context.DbContainer.TypeConverter.ConvertClrValueFromClrValue(value);
            }
            return entityReturningValues;
        }

        
    }
}
