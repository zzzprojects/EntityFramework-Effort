// --------------------------------------------------------------------------------------------
// <copyright file="TraversalVisitor.cs" company="Effort Team">
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
#if !EFOLD
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif

    internal class TraversalVisitor : DbExpressionVisitor<object>
    {
        public override object Visit(DbVariableReferenceExpression expression)
        {
            return null;
        }

        public override object Visit(DbUnionAllExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);

            return null;
        }

        public override object Visit(DbTreatExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbSkipExpression expression)
        {
            this.Visit(expression.Input.Expression);
            return null;
        }

        public override object Visit(DbSortExpression expression)
        {
            this.Visit(expression.Input.Expression);
            return null;
        }

        public override object Visit(DbScanExpression expression)
        {
            this.OnVisited(expression);
            return null;
        }

        public override object Visit(DbRelationshipNavigationExpression expression)
        {
            this.Visit(expression.NavigationSource);
            return null;
        }

        public override object Visit(DbRefExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbQuantifierExpression expression)
        {
            this.Visit(expression.Input.Expression);
            this.Visit(expression.Predicate);
            return null;
        }

        public override object Visit(DbPropertyExpression expression)
        {
            this.Visit(expression.Instance);
            return null;
        }

        public override object Visit(DbProjectExpression expression)
        {
            this.Visit(expression.Input.Expression);
            this.Visit(expression.Projection);
            return null;
        }

        public override object Visit(DbParameterReferenceExpression expression)
        {
            return null;
        }

        public override object Visit(DbOrExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);

            return null;
        }

        public override object Visit(DbOfTypeExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbNullExpression expression)
        {
            return null;
        }

        public override object Visit(DbNotExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbNewInstanceExpression expression)
        {
            foreach (var arg in expression.Arguments)
            {
                this.Visit(arg);
            }

            return null;
        }

        public override object Visit(DbLimitExpression expression)
        {
            this.Visit(expression.Argument);
            this.Visit(expression.Limit);
            return null;
        }

        public override object Visit(DbLikeExpression expression)
        {
            this.Visit(expression.Argument);
            this.Visit(expression.Pattern);
            return null;
        }

        public override object Visit(DbJoinExpression expression)
        {
            this.Visit(expression.Left.Expression);
            this.Visit(expression.Right.Expression);
            return null;
        }

        public override object Visit(DbIsOfExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbIsNullExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbIsEmptyExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbIntersectExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);
            return null;
        }

        public override object Visit(DbGroupByExpression expression)
        {
            this.Visit(expression.Input.Expression);

            foreach (var arg in expression.Keys)
            {
                this.Visit(arg);
            }

            return null;
        }

        public override object Visit(DbRefKeyExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbEntityRefExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbFunctionExpression expression)
        {
            foreach (var arg in expression.Arguments)
            {
                this.Visit(arg);
            }

            return null;
        }

        public override object Visit(DbFilterExpression expression)
        {
            this.Visit(expression.Input.Expression);
            return null;
        }

        public override object Visit(DbExceptExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);
            return null;
        }

        public override object Visit(DbElementExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbDistinctExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbDerefExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbCrossJoinExpression expression)
        {
            foreach (var arg in expression.Inputs)
            {
                this.Visit(arg.Expression);
            }

            return null;
        }

        public override object Visit(DbConstantExpression expression)
        {
            return null;
        }

        public override object Visit(DbComparisonExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);
            return null;
        }

        public override object Visit(DbCastExpression expression)
        {
            this.Visit(expression.Argument);
            return null;
        }

        public override object Visit(DbCaseExpression expression)
        {
            foreach (var arg in expression.When)
            {
                this.Visit(arg);
            }

            this.Visit(expression.Else);

            foreach (var arg in expression.Then)
            {
                this.Visit(arg);
            }

            return null;
        }

        public override object Visit(DbArithmeticExpression expression)
        {
            foreach (var arg in expression.Arguments)
            {
                this.Visit(arg);
            }

            return null;
        }

        public override object Visit(DbApplyExpression expression)
        {
            this.Visit(expression.Input.Expression);
            this.Visit(expression.Apply.Expression);

            return null;
        }

        public override object Visit(DbAndExpression expression)
        {
            this.Visit(expression.Left);
            this.Visit(expression.Right);

            return null;
        }

#if !EFOLD
        public override object Visit(DbInExpression expression)
        {
            throw new System.NotImplementedException();
        }
#endif

        public override object Visit(DbExpression expression)
        {
            return expression.Accept(this);
        }

        protected virtual void OnVisited(DbScanExpression expression)
        {
        }
    }
}
