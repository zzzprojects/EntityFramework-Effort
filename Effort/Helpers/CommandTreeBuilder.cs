using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using System.Reflection;
using System.Data.Common.CommandTrees;

namespace MMDB.EntityFrameworkProvider.Helpers
{
    internal static class CommandTreeBuilder
    {
        public static DbCommandTree CreateSelectAll( MetadataWorkspace workspace, EntitySet entitySet )
        {
            ConstructorInfo treeConstructor = 
                typeof( DbQueryCommandTree )
                .GetConstructor(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(MetadataWorkspace), typeof(DataSpace), typeof(DbExpression) },
                    null );

            Type expressionBuilder = typeof(DbExpression).Assembly.GetType("System.Data.Common.CommandTrees.ExpressionBuilder.DbExpressionBuilder");

            object dbExpression = expressionBuilder.GetMethod("Scan").Invoke(null, new object[] {entitySet});

            object tree = treeConstructor.Invoke(new object[] {workspace, DataSpace.SSpace, dbExpression});
            return tree as DbCommandTree;
        }
    }
}
