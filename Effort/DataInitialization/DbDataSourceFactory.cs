using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.Metadata.Edm;
using MMDB.EntityFrameworkProvider.Helpers;
using System.Data.EntityClient;

namespace MMDB.EntityFrameworkProvider.DataInitialization
{
    public class DbDataSourceFactory : IDataSourceFactory
    {
        private Func<EntityConnection> connectionFactory;

        private EntityConnection connection;

        public DbDataSourceFactory(Func<EntityConnection> connectionFactory)
        {
            this.connectionFactory = connectionFactory;
        }

        public IDataSource Create(string tableName, Type entityType)
        {
            if (connection == null)
            {
                this.connection = this.connectionFactory.Invoke();
                this.connection.Open();
            }

            return new DbDataSource(
                entityType, 
                this.connection,
                tableName);
        }

        public void Dispose()
        {
            if (connection != null)
            {
                this.connection.Dispose();
            }
        }
    }
}
