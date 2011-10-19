using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Test.Utils
{
    public class BasicComparer : IEqualityComparer
    {
        public new bool Equals(object x, object y)
        {
            return object.Equals(x, y);
        }

    }
}
