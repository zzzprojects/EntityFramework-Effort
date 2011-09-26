using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common.CommandTrees;
using System.Linq.Expressions;

namespace MMDB.EntityFrameworkProvider.DbCommandTreeTransform
{
    public class DbJoinConditionVisitor : DbExpressionVisitor<Expression>
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
