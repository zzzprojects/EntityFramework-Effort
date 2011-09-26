using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.DbCommandTreeTransform;
using System.Reflection;
using MMDB.Linq;

namespace MMDB.EntityFrameworkProvider.DatabaseManagement
{
    public class MethodProvider : IMethodProvider
    {
        public MethodInfo Like
        {
            get 
            {
                return typeof(Common).GetMethod("Like", BindingFlags.Public | BindingFlags.Static);
            }
        }
    }
}
