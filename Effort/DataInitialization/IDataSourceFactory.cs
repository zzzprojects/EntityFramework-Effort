using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DataInitialization
{
    public interface IDataSourceFactory : IDisposable
    {
        IDataSource Create(string tableName, Type entityType);
    }
}
