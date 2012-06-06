using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;

namespace Effort.Internal.DbCommandTreeTransformation
{
    internal class EntitySetSearchVisitor : DbExpressionTraversalVisitor
    {
        private List<EntitySetBase> tables;

        public EntitySetSearchVisitor()
        {
            this.tables = new List<EntitySetBase>();
        }

        public EntitySetBase[] Search(DbExpression expression)
        {
            this.tables.Clear();

            this.Visit(expression);

            return this.tables.ToArray();
        }


        protected override void OnVisit(DbScanExpression expression)
        {
            EntitySetBase table = expression.Target;

            if (!this.tables.Contains(table))
            {
                this.tables.Add(table);
            }
        }
    }
}
