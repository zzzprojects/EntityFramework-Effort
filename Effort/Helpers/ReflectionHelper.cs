using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq.Expressions;

namespace Effort.Helpers
{
    internal static class ReflectionHelper
    {

        public static MethodInfo GetMethodInfo<TSource, TResult>(Expression<Func<TSource, TResult>> expr)
        {
            MethodCallExpression methodCall = expr.Body as MethodCallExpression;

            return methodCall.Method;
        }

        public static MethodInfo GetMethodInfo<T>(Expression<Func<T, object>> expr)
        {
            MethodCallExpression methodCall = expr.Body as MethodCallExpression;

            return methodCall.Method;
        }

        public static MethodInfo GetMethodInfo(Expression<Func<object>> expr)
        {
            MethodCallExpression methodCall = expr.Body as MethodCallExpression;

			if( expr.Body is UnaryExpression )
			{
				methodCall = (expr.Body as UnaryExpression).Operand as MethodCallExpression;
			}

            return methodCall.Method;
        }
    }
}
