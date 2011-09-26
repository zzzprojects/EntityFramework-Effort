using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.DbCommandTreeTransform;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class DatabaseWrapper : ITableProvider
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
