using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DataInitialization
{
    internal class EmptyDataSourceFactory : IDataSourceFactory
    {
        public IDataSource Create(string tableName, Type entityType)
        {
            return new EmptyDataSource();
        }

        public void Dispose()
        {
            
        }
    }
}
