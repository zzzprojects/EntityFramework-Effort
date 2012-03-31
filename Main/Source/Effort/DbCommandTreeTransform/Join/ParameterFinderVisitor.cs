using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Effort.DbCommandTreeTransform.Join
{
    internal class ParameterFinderVisitor : ExpressionVisitor
    {
        public List<ParameterExpression> UsedParameters { get; set; }
        public List<ParameterExpression> DeclaredParameters { get; set; }

        public ParameterFinderVisitor()
        {
            this.UsedParameters = new List<ParameterExpression>();
            this.DeclaredParameters = new List<ParameterExpression>();
        }

        protected override Expression VisitParameter(ParameterExpression p)
        {
            this.UsedParameters.Add(p);

            return base.VisitParameter(p);
        }

        protected override Expression VisitLambda<T>(Expression<T> lambda)
        {
            this.DeclaredParameters.AddRange(lambda.Parameters);

            return base.VisitLambda<T>(lambda);
        }

        public void RemoveDuplicates()
        {
            this.UsedParameters = this.UsedParameters.Distinct().ToList();
            this.DeclaredParameters = this.DeclaredParameters.Distinct().ToList();
        }
    }
}
