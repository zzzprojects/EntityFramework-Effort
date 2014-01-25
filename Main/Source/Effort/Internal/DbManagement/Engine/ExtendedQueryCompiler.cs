// --------------------------------------------------------------------------------------------
// <copyright file="ExtendedQueryCompiler.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Engine
{
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using Effort.Internal.DbManagement.Engine.Modifiers;
    using NMemory.Execution;
    using NMemory.Execution.Optimization;

    internal class ExtendedQueryCompiler : QueryCompiler
    {
        public ExtendedQueryCompiler()
        {
            this.EnableOptimization = true;
        }

        protected override Expression PostprocessExpression(
            Expression expression, 
            ITransformationContext context)
        {
            expression = base.PostprocessExpression(expression, context);

            IEnumerable<IExpressionRewriter> rewriters =
                this.GetPostprocessingRewriters(expression, context);

            foreach (IExpressionRewriter rewriter in rewriters)
            {
                expression = rewriter.Rewrite(expression);
            }

            return expression;
        }

        protected override IEnumerable<IExpressionRewriter> GetRewriters(
            Expression expression, 
            ITransformationContext context)
        {
            foreach (IExpressionRewriter rewriter in base.GetRewriters(expression, context))
            {
                yield return rewriter;
            }

            // Additional rewriters
        }

        protected virtual IEnumerable<IExpressionRewriter> GetPostprocessingRewriters(
             Expression expression, 
             ITransformationContext context)
        {
            yield return new SumTransformerVisitor();

            yield return new ExcrescentInitializationCleanserVisitor();

            yield return new ExcrescentSingleResultCleanserVisitor();
        }
    }
}
