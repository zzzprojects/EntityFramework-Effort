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
using NMemory.StoredProcedures;
using NMemory;
using NMemory.Modularity;

namespace Effort.Internal.CommandActions
{
    internal class QueryCommandAction : CommandActionBase<DbQueryCommandTree>
    {
        protected override DbDataReader ExecuteDataReaderAction(DbQueryCommandTree commandTree, ActionContext context)
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

            foreach (Parameter param in context.Parameters)
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

        protected override object ExecuteScalarAction(DbQueryCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }

        protected override int ExecuteNonQueryAction(DbQueryCommandTree commandTree, ActionContext context)
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
