using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Internal.DbManagement;
using System.Data;

namespace Effort.Internal.DbCommandActions
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
