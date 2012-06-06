using System.Linq.Expressions;

namespace Effort.Internal.DbCommandTreeTransformation.PostProcessing
{
    internal interface IExpressionModifier
    {
        Expression ModifyExpression(Expression expression);
    }
}
