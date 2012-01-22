using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DataInitialization
{
    internal interface IDataSource
    {
        IEnumerable<object> GetInitialRecords();
    }
}
