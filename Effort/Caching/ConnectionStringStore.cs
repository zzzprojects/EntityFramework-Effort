using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Concurrent;
using System.Data.EntityClient;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public static class ConnectionStringStore
    {
        private static ConcurrentDictionary<ConnectionStringKey, string> store;

        static ConnectionStringStore()
        {
            store = new ConcurrentDictionary<ConnectionStringKey, string>();
        }

        public static bool TryAdd(string entityConnectionString)
        {
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(entityConnectionString);

            return store.TryAdd(new ConnectionStringKey(builder.ProviderConnectionString), entityConnectionString);
        }

        public static bool TryGet(string providerConnectionString, out string connectionString)
        {
            return store.TryGetValue(new ConnectionStringKey(providerConnectionString), out connectionString);
        }
    }
}
