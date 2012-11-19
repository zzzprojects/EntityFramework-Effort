// --------------------------------------------------------------------------------------------
// <copyright file="ExpressionHelper.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.Common
{
    using System;
    using System.Linq.Expressions;

    internal class ExpressionHelper
    {
        public static void TryUnifyValueTypes(ref Expression left, ref Expression right)
        {
            if (left.Type == right.Type)
            {
                return;
            }

            if (left.Type.IsValueType && right.Type.IsValueType)
            {
                bool leftNullable = TypeHelper.IsNullable(left.Type);
                bool rightNullable = TypeHelper.IsNullable(right.Type);

                if (leftNullable || rightNullable)
                {
                    if (leftNullable && Nullable.GetUnderlyingType(left.Type) == right.Type)
                    {
                        ConvertExpression(ref left, ref right);
                        return;
                    }

                    if (rightNullable && Nullable.GetUnderlyingType(right.Type) == left.Type)
                    {
                        ConvertExpression(ref right, ref left);
                        return;
                    }
                }
            }

            if (TypeHelper.IsCastableTo(left.Type, right.Type))
            {
                ConvertExpression(ref right, ref left);
                return;
            }
            else if (TypeHelper.IsCastableTo(right.Type, left.Type))
            {
                ConvertExpression(ref left, ref right);
                return;
            }
        }

        public static Expression ConvertToNotNull(Expression exp)
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

        private static void ConvertExpression(ref Expression to, ref Expression expr)
        {
            ////// Check if the nullable expression is constant
            ////if (to.NodeType == ExpressionType.Constant)
            ////{
            ////    ConstantExpression constant = to as ConstantExpression;

            ////    // Change the type of the constant
            ////    to = Expression.Constant(constant.Value, expr.Type);
            ////    return;
            ////}

            // Last chance
            expr = Expression.Convert(expr, to.Type);
        }
    }
}
