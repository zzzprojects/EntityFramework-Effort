using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.DbCommandTreeTransform;
using System.Reflection;
using MMDB.Linq;
using Effort.Helpers;

namespace Effort.DatabaseManagement
{
    internal class MethodProvider : IMethodProvider
    {
        public MethodInfo Like
        {
            get 
            {
                return ReflectionHelper.GetMethodInfo(() => Common.Like(string.Empty, string.Empty));
            }
        }
    }
}
