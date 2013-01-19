// --------------------------------------------------------------------------------------------
// <copyright file="TransformVisitor.IsNull.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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
    using System.Data.Common.CommandTrees;
    using System.Linq.Expressions;
    using Effort.Internal.Common;

    internal partial class TransformVisitor
    {
        public override Expression Visit(DbIsNullExpression expression)
        {
            Expression source = this.Visit(expression.Argument);

            // zsvarnai:
            // mivel nalunk az ideiglenes sorok nem mind nullable tipusuak, ezert elofordulhat olyan, 
            // hogy nem nullable elemen hivodik DbIsNull
            // Ez ilyenkor nem hasonlitja, hanem hamissal ter vissza
            // Azt azert majd le kell tesztelni, hogy logikailag jo nem okozunk-e ezzel gondot

            // tamasflamich:
            // okoztunk :(

            if (source.Type.IsValueType && !TypeHelper.IsNullable(source.Type))
            {
                return Expression.Constant(false);
            }
            else
            {
                return Expression.Equal(source, Expression.Constant(null));
            }
        }
    }
}