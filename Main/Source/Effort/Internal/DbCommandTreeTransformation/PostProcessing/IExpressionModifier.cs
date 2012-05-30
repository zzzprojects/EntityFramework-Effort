using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Effort.Internal.DbCommandTreeTransformation.PostProcessing
{
    internal interface IExpressionModifier
    {
        Expression ModifyExpression(Expression expression);
    }
}
