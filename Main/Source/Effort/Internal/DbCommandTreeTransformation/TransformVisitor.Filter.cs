// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Filter.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;
    using Effort.Internal.Common;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbFilterExpression expression)
        {
            Expression source = this.Visit(expression.Input.Expression);
            Type elementType = TypeHelper.GetElementType(source.Type);

            ParameterExpression param = Expression.Parameter(elementType, expression.Input.VariableName);

            using (this.CreateVariable(param, expression.Input.VariableName))
            {
                Expression predicate = this.Visit(expression.Predicate);
                LambdaExpression predicateLambda = Expression.Lambda(predicate, param);

                return queryMethodExpressionBuilder.Where(source, predicateLambda);
            }
        }
    }
}