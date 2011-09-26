using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.DatabaseManagement;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public static class TableInitialDataStore
    {
        private static ConcurrentCache<TableInitialDataKey, TableInitialData> store;

        static TableInitialDataStore()
        {
            store = new ConcurrentCache<TableInitialDataKey, TableInitialData>();
        }

        public static TableInitialData GetDbInitialData(string connectionString, string tableName, Func<TableInitialData> tableInitialDataFactoryMethod)
        {
            return store.Get(new TableInitialDataKey(connectionString, tableName), tableInitialDataFactoryMethod);
        }
    }
}
