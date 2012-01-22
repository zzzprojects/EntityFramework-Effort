using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.DbCommandTreeTransform
{
    internal interface ITableProvider
    {
        object GetTable(string name);
    }
}
