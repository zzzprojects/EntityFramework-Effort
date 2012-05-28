using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data;

namespace Effort.Internal.DbCommandActions
{
    internal abstract class CommandActionBase<T> : ICommandAction
        where T : DbCommandTree
    {
        public DbDataReader ExecuteDataReader(DbCommandTree commandTree, ActionContext context)
        {
            T tree = commandTree as T;

            if (tree == null)
	        {
		        throw new ArgumentException("", "commandTree");
	        }

            return this.ExecuteDataReaderAction(tree, context);
        }

        public object ExecuteScalar(DbCommandTree commandTree, ActionContext context)
        {
            T tree = commandTree as T;

            if (tree == null)
            {
                throw new ArgumentException("", "commandTree");
            }

            return this.ExecuteScalarAction(tree, context);
        }

        public int ExecuteNonQuery(DbCommandTree commandTree, ActionContext context)
        {
            T tree = commandTree as T;

            if (tree == null)
            {
                throw new ArgumentException("", "commandTree");
            }

            return this.ExecuteNonQueryAction(tree, context);
        }

        protected abstract DbDataReader ExecuteDataReaderAction(T commandTree, ActionContext context);

        protected abstract object ExecuteScalarAction(T commandTree, ActionContext context);

        protected abstract int ExecuteNonQueryAction(T commandTree, ActionContext context);
        
    }
}
