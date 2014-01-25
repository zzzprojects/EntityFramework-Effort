// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Apply.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif
    using System.Linq.Expressions;
    using Effort.Internal.Common;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbApplyExpression expression)
        {
            bool outer = false;

            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.OuterApply:
                    outer = true;
                    break;
                case DbExpressionKind.CrossApply:
                    break;
                default:
                    throw new NotSupportedException();
            }

            Expression input = this.Visit(expression.Input.Expression);
            Type inputType = TypeHelper.GetElementType(input.Type);

            ParameterExpression inputParam =
                Expression.Parameter(inputType, expression.Input.VariableName);

            using (this.CreateVariable(inputParam, expression.Input.VariableName))
            {
                // Apply expression might contain reference to the Input element,
                // so do the transformation in the variable scope
                Expression apply = this.Visit(expression.Apply.Expression);
                Type applyType = TypeHelper.GetElementType(apply.Type);

                if (outer)
                {
                    apply = this.queryMethodExpressionBuilder.DefaultIfEmpty(apply);
                }

                // Collection expression for the SelectMany
                LambdaExpression collectionSelector =
                    Expression.Lambda(
                        Expression.Convert(
                            apply,
                            typeof(IEnumerable<>).MakeGenericType(applyType)),
                    inputParam);

                // Create the SelectMany expression
                Expression result = this.CreateCrossJoin(
                    input,
                    collectionSelector,
                    expression.Input.VariableName,
                    expression.Apply.VariableName);

                return result;
            }
        }
    }
}