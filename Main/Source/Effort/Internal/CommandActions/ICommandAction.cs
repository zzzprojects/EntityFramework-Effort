using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Effort.Internal.CommandActions
{
    internal interface ICommandAction
    {
        DbDataReader ExecuteDataReader(DbCommandTree commandTree, ActionContext context);

        object ExecuteScalar(DbCommandTree commandTree, ActionContext context);

        int ExecuteNonQuery(DbCommandTree commandTree, ActionContext context);
    }
}
