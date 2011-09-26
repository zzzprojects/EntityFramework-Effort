using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public interface ITableProvider
    {
        object GetTable(string name);
    }
}
