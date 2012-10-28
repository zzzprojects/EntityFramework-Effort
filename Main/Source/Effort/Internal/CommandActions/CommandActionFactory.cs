namespace Effort.Internal.CommandActions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Data.Common.CommandTrees;

    internal static class CommandActionFactory
    {
        public static ICommandAction Create(DbCommandTree commandTree)
        {
            ICommandAction action = null;

            if (commandTree is DbQueryCommandTree)
            {
                action = new QueryCommandAction(commandTree as DbQueryCommandTree);
            }
            else if (commandTree is DbInsertCommandTree)
            {
                action = new InsertCommandAction(commandTree as DbInsertCommandTree);
            }
            else if (commandTree is DbUpdateCommandTree)
            {
                action = new UpdateCommandAction(commandTree as DbUpdateCommandTree);
            }
            else if (commandTree is DbDeleteCommandTree)
            {
                action = new DeleteCommandAction(commandTree as DbDeleteCommandTree);
            }

            if (action == null)
            {
                throw new NotSupportedException("Not supported DbCommandTree type");
            }

            return action;
        }
    }
}
