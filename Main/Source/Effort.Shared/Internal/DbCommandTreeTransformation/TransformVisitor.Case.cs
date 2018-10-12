// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Case.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

using System;

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif
    using System.Linq;
    using System.Linq.Expressions;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbCaseExpression expression)
        {
            List<Expression> cases = new List<Expression>() { this.Visit(expression.Else) };

            for (int i = expression.When.Count - 1; i >= 0; i--)
            {
                var ifTrue = this.Visit(expression.Then[i]);
                var ifFalse = cases.Last();

                if (ifTrue.Type != ifFalse.Type)
                {
                    if (ifTrue.Type.IsGenericType
                        && ifTrue.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && ifTrue.Type.GetGenericArguments()[0] == ifFalse.Type)
                    {
                        ifFalse = Expression.Convert(ifFalse, ifTrue.Type);
                    }
                    else if (ifFalse.Type.IsGenericType
                        && ifFalse.Type.GetGenericTypeDefinition() == typeof(Nullable<>)
                        && ifFalse.Type.GetGenericArguments()[0] == ifTrue.Type)
                    {
                        ifTrue = Expression.Convert(ifTrue, ifFalse.Type);
                    }
                }
                cases.Add(
                    Expression.Condition(
                        this.Visit(expression.When[i]),
                        ifTrue,
                        ifFalse));
            }

            return cases.Last();
        }
    }
}