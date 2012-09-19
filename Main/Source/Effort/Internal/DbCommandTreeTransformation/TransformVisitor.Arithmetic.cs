#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.DbCommandTreeTransformation.Join;
using Effort.Internal.DbCommandTreeTransformation.Variables;
using Effort.Internal.TypeConversion;
using Effort.Internal.TypeGeneration;

namespace Effort.Internal.DbCommandTreeTransformation
{
    internal partial class TransformVisitor
    {
        public override Expression Visit(DbArithmeticExpression expression)
        {
            Expression[] args = new Expression[expression.Arguments.Count];

            for (int i = 0; i < expression.Arguments.Count; i++)
            {
                args[i] = this.Visit(expression.Arguments[i]);
            }

            // This check needs because of UnaryMinus, which has a single argument
            if (args.Length == 2)
            {
                ExpressionHelper.TryUnifyValueTypes(ref args[0], ref args[1]);
            }

            switch (expression.ExpressionKind)
            {
                case DbExpressionKind.Plus:
                    return Expression.Add(args[0], args[1]);

                case DbExpressionKind.Minus:
                    return Expression.Subtract(args[0], args[1]);

                case DbExpressionKind.Multiply:
                    return Expression.Multiply(args[0], args[1]);

                case DbExpressionKind.Divide:
                    return Expression.Divide(args[0], args[1]);

                case DbExpressionKind.Modulo:
                    return Expression.Modulo(args[0], args[1]);

                case DbExpressionKind.UnaryMinus:
                    return Expression.Negate(args[0]);
            }

            throw new InvalidOperationException("The ExpressionKind cannot be " + expression.ExpressionKind.ToString());
        }
    }
}