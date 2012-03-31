#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Linq.Expressions;
using Effort.DbCommandTreeTransform;

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
            if (TypeHelper.IsNullable(exp.Type))
            {
                return Expression.Convert(exp, TypeHelper.MakeNotNullable(exp.Type));
            }
            else
            {
                return exp;
            }
        }

        public static Expression CorrectType(Expression exp, Type desiredType)
        {
            Type type = exp.Type;

            if (type.Equals(desiredType))
            {
                return exp;
            }

            if (TypeHelper.IsNullable(type) && TypeHelper.MakeNotNullable(type) == desiredType)
            {
                return Expression.Convert(exp, desiredType);
            }

            if (TypeHelper.IsNullable(desiredType) && TypeHelper.MakeNotNullable(desiredType) == type)
            {
                return Expression.Convert(exp, desiredType);
            }

            throw new NotSupportedException();
        }

    }
}
