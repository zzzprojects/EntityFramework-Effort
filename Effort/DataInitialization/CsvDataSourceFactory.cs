using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.IO;

namespace Effort.DataInitialization
{
    internal class CsvDataSourceFactory : IDataSourceFactory
    {
        private string connectionString;
        private DirectoryInfo source;

        public CsvDataSourceFactory(string connectionString)
        {
            this.connectionString = connectionString;

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (!builder.ContainsKey("Data Source"))
            {
                throw new InvalidOperationException("Connectionstring does not contain 'Data Source' attribute");
            }

            this.source = new DirectoryInfo(builder["Data Source"] as string);

            if (!source.Exists)
            {
                throw new InvalidOperationException("The directory specified in 'Data Source' attribute does not exist");
            }
        }

        public IDataSource Create(string tableName, Type entityType)
        {
            string csvPath = Path.Combine(source.FullName, string.Format("{0}.csv", tableName));

            return new CachedCsvDataSource(this.connectionString, tableName, csvPath, entityType);
        }

        public void Dispose()
        {
            
        }
    }
}
