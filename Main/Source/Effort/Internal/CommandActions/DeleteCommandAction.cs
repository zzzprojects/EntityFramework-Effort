using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Linq;
using System.Linq.Expressions;
using Effort.Internal.Common;
using NMemory.Tables;

namespace Effort.Internal.CommandActions
{
    internal class DeleteCommandAction : ICommandAction
    {
        private DbDeleteCommandTree commandTree;

        public DeleteCommandAction(DbDeleteCommandTree commandTree)
        {
            this.commandTree = commandTree;
        }

        public DbDataReader ExecuteDataReader(ActionContext context)
        {
            throw new NotSupportedException();
        }

        public object ExecuteScalar(ActionContext context)
        {
            throw new NotSupportedException();
        }

        public int ExecuteNonQuery(ActionContext context)
        {
            ITable table = null;

            Expression expr = DbCommandActionHelper.GetEnumeratorExpression(this.commandTree.Predicate, commandTree, context.DbContainer, out table);
            IQueryable entitiesToDelete = DatabaseReflectionHelper.CreateTableQuery(expr, context.DbContainer.Internal);

            return DatabaseReflectionHelper.DeleteEntities(entitiesToDelete, context.Transaction);
        }
    }
}
