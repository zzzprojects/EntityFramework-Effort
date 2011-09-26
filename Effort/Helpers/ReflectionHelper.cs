using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace MMDB.EntityFrameworkProvider.Helpers
{
    internal static class ReflectionHelper
    {
        public static MethodInfo GetMethodInfo<T>(Expression<Func<T, object>> expr)
        {
            MethodCallExpression methodCall = expr.Body as MethodCallExpression;

            return methodCall.Method;
        }
    }
}
