using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform.PostProcessing
{
    public class ExcrescentInitializationCleanserVisitor : ExpressionVisitor
    {
        

        protected override Expression VisitMember(MemberExpression node)
        {
            string value = "";
            var a = new { Property = value }.Property;

            //Check if the target expression is just an object initialization
            if (node.Expression.NodeType == ExpressionType.New)
            {
                NewExpression newExpression = node.Expression as NewExpression;

                if (newExpression.Members.Count == 1 && newExpression.Members[0] == node.Member)
                {
                    return newExpression.Arguments[0];
                }

            }
            
            return base.VisitMember(node);
        }
    }
}
