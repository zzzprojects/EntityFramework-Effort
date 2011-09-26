using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform.PostProcessing
{
    /// <summary>
    /// Transforms SingleResult<>(x).FirstOrDefault() to x
    /// </summary>
    public class ExcrescentSingleResultCleanserVisitor : ExpressionVisitor
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            //Check if the method is Queryable.FirstOrDefault
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "FirstOrDefault")
            {
                Expression source = node.Arguments[0];

                //Check if the source argument is an initialization
                if (source.NodeType == ExpressionType.New)
                {
                    NewExpression newExpression = source as NewExpression;
                    Type declaringType = newExpression.Constructor.DeclaringType;

                    //Check if the initialized object is a SingleResult<>
                    if (declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(SingleResult<>))
                    {
                        Expression constuctorArgument = newExpression.Arguments[0];

                        return constuctorArgument;
                    }
                }
            }

            return base.VisitMethodCall(node);
        }
    }
}
