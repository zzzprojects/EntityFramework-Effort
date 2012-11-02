// ----------------------------------------------------------------------------------
// <copyright file="DbJoinConditionVisitor.cs" company="Effort Team">
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

namespace Effort.Internal.DbCommandTreeTransformation.Join
{
    using System.Collections.Generic;
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;

    internal class DbJoinConditionVisitor : DbExpressionVisitor<Expression>
    {
        public List<DbExpression> LeftSide = new List<DbExpression>();
        public List<DbExpression> RightSide = new List<DbExpression>();

        public override Expression Visit( DbVariableReferenceExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbUnionAllExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbTreatExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbSkipExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbSortExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbScanExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbRelationshipNavigationExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbRefExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbQuantifierExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbPropertyExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbProjectExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbParameterReferenceExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbOrExpression expression )
        {
            this.Visit( expression.Left );
            this.Visit( expression.Right );
            
            return null;
        }

        public override Expression Visit( DbOfTypeExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbNullExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbNotExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbNewInstanceExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbLimitExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbLikeExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbJoinExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbIsOfExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbIsNullExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbIsEmptyExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbIntersectExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbGroupByExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbRefKeyExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbEntityRefExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbFunctionExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbFilterExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbExceptExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbElementExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbDistinctExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbDerefExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbCrossJoinExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbConstantExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbComparisonExpression expression )
        {
            this.LeftSide.Add( expression.Left );
            this.RightSide.Add( expression.Right );

            return null;
        }

        public override Expression Visit( DbCastExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbCaseExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbArithmeticExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbApplyExpression expression )
        {
            return null;
        }

        public override Expression Visit( DbAndExpression expression )
        {
            this.Visit( expression.Left );
            this.Visit( expression.Right );

            return null;
        }

        public override Expression Visit( DbExpression expression )
        {
            return expression.Accept( this );
        }
    }
}
