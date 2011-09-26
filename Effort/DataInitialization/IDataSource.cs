using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DataInitialization
{
    public interface IDataSource
    {
        IEnumerable<object> GetInitialRecords();
    }
}
