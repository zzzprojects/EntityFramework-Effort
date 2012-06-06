using System.Collections.Generic;

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
