// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Join.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;
    using Effort.Internal.Common;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbJoinExpression expression)
        {
            if (expression.ExpressionKind == DbExpressionKind.FullOuterJoin)
            {
                throw new NotSupportedException("Full outer join is not yet supported");
            }

            Expression left = this.Visit(expression.Left.Expression);
            Expression right = this.Visit(expression.Right.Expression);

            Type leftType = TypeHelper.GetElementType(left.Type);
            Type rightType = TypeHelper.GetElementType(right.Type);

            ParameterExpression leftParam =
                Expression.Parameter(leftType, expression.Left.VariableName);

            ParameterExpression rightParam = 
                Expression.Parameter(rightType, expression.Right.VariableName);

            using (this.CreateVariable(leftParam, leftParam.Name))
            using (this.CreateVariable(rightParam, rightParam.Name))
            {
                LambdaExpression joinCondition =
                    Expression.Lambda(
                        this.Visit(expression.JoinCondition),
                        rightParam);

                // TODO: Try to build Join expression

                Expression innerExpression =
                    this.queryMethodExpressionBuilder.Where(right, joinCondition);

                if (expression.ExpressionKind == DbExpressionKind.LeftOuterJoin)
                {
                    innerExpression =
                        queryMethodExpressionBuilder.DefaultIfEmpty(innerExpression);
                }

                // Collection expression for the SelectMany
                LambdaExpression collectionSelector =
                    Expression.Lambda(
                        Expression.Convert(
                            innerExpression,
                            typeof(IEnumerable<>).MakeGenericType(rightType)),
                    leftParam);

                Expression result = this.CreateCrossJoin(
                    left,
                    collectionSelector,
                    leftParam.Name,
                    rightParam.Name);

                return result;
            }
        }
    }
}