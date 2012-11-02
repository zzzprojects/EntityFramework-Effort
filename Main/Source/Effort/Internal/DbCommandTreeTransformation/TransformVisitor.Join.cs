// ----------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Join.cs" company="Effort Team">
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

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Data.Common.CommandTrees;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.DbCommandTreeTransformation.Join;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbJoinExpression expression)
        {
            Expression left = this.Visit(expression.Left.Expression);
            Expression right = this.Visit(expression.Right.Expression);

            ParameterExpression leftParam = Expression.Parameter(left.Type.GetGenericArguments().First(), "p0");
            ParameterExpression rightParam = Expression.Parameter(right.Type.GetGenericArguments().First(), "p1");

            LambdaExpression firstKeySelector;
            LambdaExpression secondKeySelector;

            using (this.CreateVariable(leftParam, expression.Left.VariableName))
            using (this.CreateVariable(rightParam, expression.Right.VariableName))
            {
                Expression joinCondition = this.Visit(expression.JoinCondition); //what is this used for here?

                DbJoinConditionVisitor v = new DbJoinConditionVisitor();

                v.Visit(expression.JoinCondition);

                var leftExpressions = v.LeftSide.Select(dbExp => this.Visit(dbExp)).ToList();
                var rightExpressions = v.RightSide.Select(dbExp => this.Visit(dbExp)).ToList();

                ParameterFinderVisitor pfv = new ParameterFinderVisitor();

                foreach (var exp in leftExpressions)
                {
                    pfv.Visit(exp);
                }

                if (pfv.UsedParameters.Contains(rightParam))
                {
                    if (pfv.UsedParameters.Contains(leftParam))
                    {
                        throw new InvalidOperationException();
                    }
                    else
                    {
                        // THe join condition is reversed

                        var swap = leftExpressions;
                        leftExpressions = rightExpressions;
                        rightExpressions = swap;
                    }
                }

                Expression leftArrayInit = Expression.NewArrayInit(typeof(object),
                    leftExpressions.Select(exp => Expression.Convert(exp, typeof(object))).ToArray());

                Expression rightArrayInit = Expression.NewArrayInit(typeof(object),
                    rightExpressions.Select(exp => Expression.Convert(exp, typeof(object))).ToArray());

                ConstructorInfo propListConstructor = typeof(PropertyList).GetConstructors().First();

                Expression leftNewPropertyList = Expression.New(propListConstructor, leftArrayInit);
                Expression rightNewPropertyList = Expression.New(propListConstructor, rightArrayInit);

                firstKeySelector = Expression.Lambda(leftNewPropertyList, leftParam);
                secondKeySelector = Expression.Lambda(rightNewPropertyList, rightParam);

            }

            //using (this.CreateContext(leftParam, expression.Left.VariableName))
            //{
            //    Expression leftSelectorBody = this.Visit((expression.JoinCondition as DbComparisonExpression).Left);
            //    firstKeySelector = Expression.Lambda(leftSelectorBody, leftParam);

            //}
            //using (this.CreateContext(rightParam, expression.Right.VariableName))
            //{
            //    Expression rightSelectorBody = this.Visit((expression.JoinCondition as DbComparisonExpression).Right);
            //    secondKeySelector = Expression.Lambda(rightSelectorBody, rightParam);
            //}


            Expression result = queryMethodExpressionBuilder.Join(
                left, right,
                expression.Left.VariableName, expression.Right.VariableName,
                firstKeySelector, secondKeySelector,
                expression.ExpressionKind);

            return result;
        }
    }
}