// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.CrossJoin.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbCrossJoinExpression expression)
        {
            List<Expression> inputExpressions = new List<Expression>();

            foreach (var i in expression.Inputs)
            {
                inputExpressions.Add(this.Visit(i.Expression));
            }

            Expression last = inputExpressions[0];

            // Always: at most 2
            for (int i = 1; i < inputExpressions.Count; i++)
            {
                last = queryMethodExpressionBuilder.SelectMany(last, inputExpressions[i],
                    expression.Inputs[i - 1].VariableName,
                    expression.Inputs[i].VariableName);
            }
            return last;
        }
    }
}