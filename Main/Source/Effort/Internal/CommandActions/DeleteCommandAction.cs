using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Common;
using System.Linq.Expressions;
using NMemory.Tables;
using Effort.Internal.Common;

namespace Effort.Internal.DbCommandActions
{
    internal class DeleteCommandAction : CommandActionBase<DbDeleteCommandTree>
    {
        protected override DbDataReader ExecuteDataReaderAction(DbDeleteCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }

        protected override object ExecuteScalarAction(DbDeleteCommandTree commandTree, ActionContext context)
        {
            throw new NotSupportedException();
        }

        protected override int ExecuteNonQueryAction(DbDeleteCommandTree commandTree, ActionContext context)
        {
            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(commandTree.Predicate, commandTree, context.DbContainer, out table);
            IQueryable entitiesToDelete = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            return DatabaseReflectionHelper.DeleteEntities(entitiesToDelete);
        }
    }
}
