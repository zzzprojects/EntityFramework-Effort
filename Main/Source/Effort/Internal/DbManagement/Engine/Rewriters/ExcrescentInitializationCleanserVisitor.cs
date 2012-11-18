// --------------------------------------------------------------------------------------------
// <copyright file="ExcrescentInitializationCleanserVisitor.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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

namespace Effort.Internal.DbManagement.Engine.Modifiers
{
    using System.Linq.Expressions;
    using NMemory.Execution.Optimization.Rewriters;
    using Effort.Internal.Common;

    internal class ExcrescentInitializationCleanserVisitor : ExpressionRewriterBase
    {
        protected override Expression VisitMember(MemberExpression node)
        {
            // Check if the target expression is just an object initialization
            if (node.Expression.NodeType == ExpressionType.New)
            {
                NewExpression newExpression = node.Expression as NewExpression;

                // TODO: Is Anonymous Type

                if (newExpression.Members.Count == 1 && 
                    newExpression.Members[0] == node.Member)
                {
                    return newExpression.Arguments[0];
                }
            }
            
            return base.VisitMember(node);
        }
    }
}
