using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Test.Utils
{
    public interface IEqualityComparer
    {
        bool Equals(object x, object y);
    }
}
