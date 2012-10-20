using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.DataLoaders;

namespace Effort.Internal.DbManagement
{
    internal class DbContainerParameters
    {
        public bool IsDataLoaderCached { get; set; }

        public IDataLoader DataLoader { get; set; }

        public bool IsTransient { get; set; }
    }
}
