// ----------------------------------------------------------------------------------
// <copyright file="ExcrescentSingleResultCleanserVisitor.cs" company="Effort Team">
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

    /// <summary>
    /// Transforms SingleResult<>(x).FirstOrDefault() to x
    /// </summary>
    internal class ExcrescentSingleResultCleanserVisitor : ExpressionVisitor, IExpressionModifier
    {
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            // Check if the method is Queryable.FirstOrDefault
            if (node.Method.DeclaringType == typeof(Queryable) && node.Method.Name == "FirstOrDefault")
            {
                Expression source = node.Arguments[0];

                // Check if the source argument is an initialization
                if (source.NodeType == ExpressionType.New)
                {
                    NewExpression newExpression = source as NewExpression;
                    Type declaringType = newExpression.Constructor.DeclaringType;

                    // Check if the initialized object is a SingleResult<>
                    if (declaringType.IsGenericType && declaringType.GetGenericTypeDefinition() == typeof(SingleResult<>))
                    {
                        Expression constuctorArgument = newExpression.Arguments[0];

                        return constuctorArgument;
                    }
                }
            }

            return base.VisitMethodCall(node);
        }

        public Expression ModifyExpression(Expression expression)
        {
            return this.Visit(expression);
        }
    }
}
