using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DataInitialization
{
    internal interface IDataSourceFactory : IDisposable
    {
        IDataSource Create(string tableName, Type entityType);
    }
}
