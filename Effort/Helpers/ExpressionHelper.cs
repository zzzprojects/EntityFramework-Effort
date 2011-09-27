using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using MMDB.Linq.Visitors;
using Effort.DbCommandTreeTransform;
using MMDB.Linq;

namespace Effort.Helpers
{
    internal class ExpressionHelper
    {
        public static void TryUnifyValueTypes(ref Expression left, ref Expression right)
        {
            if (!left.Type.IsValueType || !right.Type.IsValueType)
            {
                return;
            }

            bool leftNullable = TypeHelper.IsNullable(left.Type);
            bool rightNullable = TypeHelper.IsNullable(right.Type);

            if (leftNullable || rightNullable)
            {
                if (!rightNullable && Nullable.GetUnderlyingType(left.Type) == right.Type)
                {
                    ConvertExpression(ref left, ref right);
                }

                if (!leftNullable && Nullable.GetUnderlyingType(right.Type) == left.Type)
                {
                    ConvertExpression(ref right, ref left);
                }
            }
        }

        private static void ConvertExpression(ref Expression nullableExpression, ref Expression otherExpression)
        {
            //Check if the nullable expression is constant
            if (nullableExpression.NodeType == ExpressionType.Constant)
            {
                ConstantExpression constant = nullableExpression as ConstantExpression;

                //Change the type of the constant
                nullableExpression = Expression.Constant(constant.Value, otherExpression.Type);
                return;
            }

            //Last chance
            otherExpression = Expression.Convert(otherExpression, nullableExpression.Type);
        }

        public static Expression ConvertToNotNull( Expression exp )
        {
            if(TypeHelper.IsNullable( exp.Type ) )
                return Expression.Convert(exp, TypeHelper.MakeNotNullable( exp.Type ));
            else
                return exp;
        }

    }
}
