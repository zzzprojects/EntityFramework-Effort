using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MMDB.Locking;
using MMDB.Logging;
using MMDB.EntityFrameworkProvider.Helpers;
using MMDB.EntityFrameworkProvider.Caching;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class DbInstanceStore
    {
        private static ConcurrentCache<ConnectionStringKey, Database> store;

        static DbInstanceStore()
        {
            store = new ConcurrentCache<ConnectionStringKey, Database>();
        }

        public static Database GetDbInstance(string connectionString, Func<Database> databaseFactoryMethod)
        {
            return store.Get(new ConnectionStringKey(connectionString), databaseFactoryMethod);
        }
    }
}
