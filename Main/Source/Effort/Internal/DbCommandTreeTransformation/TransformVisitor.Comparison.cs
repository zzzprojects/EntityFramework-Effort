// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.ComparisonExpression.cs" company="Effort Team">
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
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif
    using System.Linq.Expressions;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation.Functions;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbComparisonExpression expression)
        {
            Expression left = this.Visit(expression.Left);
            Expression right = this.Visit(expression.Right);

            ExpressionHelper.TryUnifyValueTypes(ref left, ref right);

            return this.CreateComparison(left, right, expression.ExpressionKind);
        }

        private Expression CreateComparison(Expression left, Expression right, DbExpressionKind kind)
        {
            if (left.Type == typeof(string) && right.Type == typeof(string))
            {
                return CreateStringComparison(left, right, kind);
            }

            switch (kind)
            {
                case DbExpressionKind.Equals:
                    return Expression.Equal(left, right);

                case DbExpressionKind.NotEquals:
                    return Expression.NotEqual(left, right);

                case DbExpressionKind.GreaterThan:
                    return Expression.GreaterThan(left, right);

                case DbExpressionKind.GreaterThanOrEquals:
                    return Expression.GreaterThanOrEqual(left, right);

                case DbExpressionKind.LessThan:
                    return Expression.LessThan(left, right);

                case DbExpressionKind.LessThanOrEquals:
                    return Expression.LessThanOrEqual(left, right);

                default:
                    throw new InvalidOperationException(
                        "The ExpressionKind cannot be " + kind.ToString());
            }
        }

        private Expression CreateStringComparison(Expression left, Expression right, DbExpressionKind kind)
        {
            var method = Expression.Call(null, StringFunctions.CompareTo, left, right);
            var mode = GetCompareMode(kind);

            Expression res = Expression.Equal(method, Expression.Constant(mode.Item1));

            if (!mode.Item2)
            {
                res = Expression.Not(res);
            }

            return res;
        }

        private Tuple<int, bool> GetCompareMode(DbExpressionKind kind)
        {
            switch (kind)
            {
                case DbExpressionKind.Equals:
                    return Tuple.Create(0, true);
                case DbExpressionKind.NotEquals:
                    return Tuple.Create(0, false);
                case DbExpressionKind.GreaterThan:
                    return Tuple.Create(1, true);
                case DbExpressionKind.GreaterThanOrEquals:
                    return Tuple.Create(-1, false);
                case DbExpressionKind.LessThan:
                    return Tuple.Create(-1, true);
                case DbExpressionKind.LessThanOrEquals:
                    return Tuple.Create(1, false);
            }

            throw new InvalidOperationException(
                "The ExpressionKind cannot be " + kind.ToString());
        }
    }
}