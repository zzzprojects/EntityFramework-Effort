using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.UnitTests.Utils
{
    public class BasicComparer : IEqualityComparer
    {
        public bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

    }
}
