using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.DatabaseManagement;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public static class DbSchemaStore
    {
        private static ConcurrentCache<DbSchemaKey, DatabaseSchema> store;

        static DbSchemaStore()
        {
            store = new ConcurrentCache<DbSchemaKey, DatabaseSchema>();
        }

        public static DatabaseSchema GetDbSchema(string[] metadataFiles, Func<DatabaseSchema> schemaFactoryMethod)
        {
            return store.Get(new DbSchemaKey(metadataFiles), schemaFactoryMethod);
        }
    }
}
