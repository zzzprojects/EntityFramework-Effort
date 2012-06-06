using System.Collections.Generic;
using System.Data;
using Effort.Internal.DbManagement;

namespace Effort.Internal.CommandActions
{
    internal sealed class ActionContext
    {
        private DbContainer dbContainer;

        public ActionContext(DbContainer dbContainer)
        {
            this.dbContainer = dbContainer;
            this.Parameters = new List<Parameter>();
        }

        public DbContainer DbContainer
        {
            get { return this.dbContainer; }
        }

        public CommandBehavior CommandBehavior { get; set; }

        public IList<Parameter> Parameters { get; private set; }

    }
}
