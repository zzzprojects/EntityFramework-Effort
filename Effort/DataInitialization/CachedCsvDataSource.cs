using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.Caching;
using MMDB.EntityFrameworkProvider.DatabaseManagement;

namespace MMDB.EntityFrameworkProvider.DataInitialization
{
    public class CachedCsvDataSource : CsvDataSource
    {
        private string connectionString;
        private string tableName;

        public CachedCsvDataSource(string connectionString, string tableName, string path, Type entityType)
            : base(path, entityType)
        {
            this.connectionString = connectionString;
            this.tableName = tableName;
        }

        public override IEnumerable<object> GetInitialRecords()
        {
            var cache = TableInitialDataStore.GetDbInitialData(
                this.connectionString, 
                this.tableName, 
                () => new TableInitialData(base.GetInitialRecords()));

            return cache.GetClonedInitialData();
        }


    }
}
