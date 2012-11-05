// ----------------------------------------------------------------------------------
// <copyright file="SumTransformerVisitor.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation.PostProcessing
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using Effort.Internal.Common;

    internal class SumTransformerVisitor : ExpressionVisitor, IExpressionModifier
    {
        public Expression ModifyExpression(Expression expression)
        {
            return this.Visit(expression);
        }

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
