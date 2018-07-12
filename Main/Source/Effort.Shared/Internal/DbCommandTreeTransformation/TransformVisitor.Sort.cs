// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Sort.cs" company="Effort Team">
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
        public override Expression Visit(DbSortExpression expression)
        {
            Expression source = this.Visit(expression.Input.Expression);

            return this.CreateOrderByExpression(expression.SortOrder, expression.Input.VariableName, source);
        }

        private Expression CreateOrderByExpression(IList<DbSortClause> sortorder, string sourceVariableName, Expression source)
        {
            Type sourceType = TypeHelper.GetElementType(source.Type);

            Expression result = source;
            LambdaExpression selector = null;

            for (int i = 0; i < sortorder.Count; i++)
            {
                DbSortClause sort = sortorder[i];

                ParameterExpression param = Expression.Parameter(sourceType, sourceVariableName);
                using (this.CreateVariable(param, sourceVariableName))
                {
                    selector = Expression.Lambda(this.Visit(sort.Expression), param);
                }

                if (sort.Ascending)
                {
                    if (i == 0)
                    {
                        result = queryMethodExpressionBuilder.OrderBy(result, selector);
                    }
                    else
                    {
                        result = queryMethodExpressionBuilder.ThenBy(result, selector);
                    }
                }
                else
                {
                    if (i == 0)
                    {
                        result = queryMethodExpressionBuilder.OrderByDescending(result, selector);
                    }
                    else
                    {
                        result = queryMethodExpressionBuilder.ThenByDescending(result, selector);
                    }
                }
            }

            return result;
        }
    }
}