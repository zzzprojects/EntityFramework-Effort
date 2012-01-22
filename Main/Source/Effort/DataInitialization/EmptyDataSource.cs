using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DataInitialization
{
    internal class EmptyDataSource : IDataSource
    {
        public IEnumerable<object> GetInitialRecords()
        {
            yield break;
        }
    }
}
