using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Internal.DbCommandTreeTransformation.PostProcessing
{
    internal static class Modifiers
    {
        public static IEnumerable<IExpressionModifier> GetModifiers()
        {
            yield return new ExcrescentInitializationCleanserVisitor();

            yield return new ExcrescentSingleResultCleanserVisitor();

            yield return new SumTransformerVisitor();
        }
    }
}
