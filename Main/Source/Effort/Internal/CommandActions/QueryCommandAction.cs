using System;
using System.Collections;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using Effort.Internal.DbCommandTreeTransformation;
using Effort.Provider;
using Effort.Internal.Common;
using System.Collections.Generic;
using NMemory.StoredProcedures;
using Effort.Internal.DbCommandTreeTransformation.PostProcessing;

namespace Effort.Internal.DbCommandActions
{
    internal class QueryCommandAction : CommandActionBase<DbQueryCommandTree>
    {
        protected override DbDataReader ExecuteDataReaderAction(DbQueryCommandTree commandTree, ActionContext context)
        {
            DbExpressionTransformVisitor visitor = new DbExpressionTransformVisitor(context.DbContainer.TypeConverter);
            visitor.SetParameters(commandTree.Parameters.ToArray());
            visitor.TableProvider = context.DbContainer;

            // Transform command tree
            Expression expr = visitor.Visit(commandTree.Query);

            // Execute expression post processing
            foreach (IExpressionModifier modifier in DbCommandTreeTransformation.PostProcessing.Modifiers.GetModifiers())
            {
                expr = modifier.ModifyExpression(expr);
            }

            // Create a stored procedure from the expression
            IStoredProcedure procedure = DatabaseReflectionHelper.CreateStoredProcedure(expr, context.DbContainer.Internal);

            // Determine parameter values
            Dictionary<string, object> parameters = new Dictionary<string, object>();

            foreach (Parameter param in context.Parameters)
            {
                string name = param.Name;
                object value = param.Value;

                // Find the description of the parameter
                ParameterDescription expectedParam = procedure.Parameters.FirstOrDefault(p => p.Name == param.Name);

                // Custom conversion
                value = context.DbContainer.TypeConverter.ConvertClrValueToClrValue(value, expectedParam.Type);

                parameters.Add(name, value);
            }

            IEnumerable result = procedure.Execute(parameters);

            string[] fieldNames = TypeHelper.GetElementType(result.GetType()).GetProperties().Select(p => p.Name).ToArray();
            return new EffortDataReader(result, fieldNames, context.DbContainer);
        }

        protected override object ExecuteScalarAction(DbQueryCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }

        protected override int ExecuteNonQueryAction(DbQueryCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }
    }
}
