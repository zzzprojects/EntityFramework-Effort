using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Effort.DbCommandTreeTransform
{
    internal interface IMethodProvider
    {
        MethodInfo Like { get; }
    }
}
