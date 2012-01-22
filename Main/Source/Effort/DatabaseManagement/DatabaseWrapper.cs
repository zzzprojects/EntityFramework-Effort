using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.DbCommandTreeTransform;
using MMDB;

namespace Effort.DatabaseManagement
{
    internal class DatabaseWrapper : ITableProvider
    {
        private Database database;

        public DatabaseWrapper(Database database)
        {
            this.database = database;
        }

        public object GetTable(string name)
        {
            return database.GetTable(name);
        }
    }
}
