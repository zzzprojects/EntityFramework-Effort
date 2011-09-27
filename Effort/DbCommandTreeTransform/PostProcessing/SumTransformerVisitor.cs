using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using Effort.Helpers;

namespace Effort.DbCommandTreeTransform.PostProcessing
{
    internal class SumTransformerVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            Type returnType = node.Method.ReturnType;
            
            // There is no scenario when Queryable.Sum is used
            if (node.Method.DeclaringType == typeof(Enumerable) && 
                node.Method.Name == "Sum" && 
                TypeHelper.IsNullable(returnType))
            {
                Type type = TypeHelper.MakeNotNullable(returnType);
                Type sourceType = node.Method.GetGenericArguments()[0];

                return Expression.Call(
                    typeof(EnumerableNullableSum)
                    .GetMethods()
                    .Where(mi =>
                        mi.Name == "Sum" &&
                        mi.ReturnType == returnType)
                    .Single()
                    .MakeGenericMethod(sourceType),
                    node.Arguments);
                
            }
            
            return base.VisitMethodCall(node);
        }


    }
}
