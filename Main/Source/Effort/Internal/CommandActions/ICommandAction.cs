using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Effort.Internal.CommandActions
{
    internal interface ICommandAction
    {
        DbDataReader ExecuteDataReader(ActionContext context);

        object ExecuteScalar(ActionContext context);

        int ExecuteNonQuery(ActionContext context);
    }
}
