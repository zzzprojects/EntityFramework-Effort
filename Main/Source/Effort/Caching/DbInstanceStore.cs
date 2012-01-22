using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using MMDB.Locking;
using MMDB.Logging;
using Effort.Helpers;
using Effort.Caching;
using MMDB;

namespace Effort.DatabaseManagement
{
    internal class DbInstanceStore
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
