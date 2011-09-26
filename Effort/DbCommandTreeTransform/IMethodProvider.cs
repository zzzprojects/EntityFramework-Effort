using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public interface IMethodProvider
    {
        MethodInfo Like { get; }
    }
}
