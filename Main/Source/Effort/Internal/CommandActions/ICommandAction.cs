using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Data.Common;
using System.Data;

namespace Effort.Internal.DbCommandActions
{
    internal interface ICommandAction
    {
        DbDataReader ExecuteDataReader(DbCommandTree commandTree, ActionContext context);

        object ExecuteScalar(DbCommandTree commandTree, ActionContext context);

        int ExecuteNonQuery(DbCommandTree commandTree, ActionContext context);
    }
}
