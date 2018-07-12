// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.Except.cs" company="Effort Team">
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
    using System.Linq.Expressions;
    using System;
    using Effort.Internal.Common;
    using System.Reflection;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbExceptExpression expression)
        {
            Type resultType = this.edmTypeConverter.Convert(expression.ResultType);

            Expression left = this.Visit(expression.Left);
            Expression right = this.Visit(expression.Right);

            // Entity Framework does not ensure that left, right and result items have the same
            // type
            var resultElemType = TypeHelper.GetElementType(resultType);

            this.UnifyCollections(resultElemType, ref left, ref right);

            return queryMethodExpressionBuilder.Except(left, right);
        }

        private void UnifyCollections(
            Type expectedType, 
            ref Expression e1, 
            ref Expression e2)
        {
            ChangeCollectionType(expectedType, ref e1);
            ChangeCollectionType(expectedType, ref e2);
        }

        private void ChangeCollectionType(Type expectedType, ref Expression node)
        {
            Type type = TypeHelper.GetElementType(node.Type);

            if (expectedType == type)
            {
                return;
            }

            var param = Expression.Parameter(type);

            PropertyInfo[] sourceProps = type.GetProperties();
            Expression[] initializers = new Expression[sourceProps.Length];

            for (int j = 0; j < sourceProps.Length; j++)
            {
                initializers[j] = Expression.Property(param, sourceProps[j]);
            }

            Expression body = this.CreateSelector(initializers, expectedType);

            node =
                this.queryMethodExpressionBuilder.Select(
                    node,
                    Expression.Lambda(body, param));
        }
    }
}