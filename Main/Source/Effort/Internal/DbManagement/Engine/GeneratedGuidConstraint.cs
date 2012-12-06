// --------------------------------------------------------------------------------------------
// <copyright file="GeneratedGuidConstraint.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Engine
{
    using System;
    using System.Linq.Expressions;
    using NMemory.Constraints;

    internal class GeneratedGuidConstraint<T> : ConstraintBase<T, Guid>
    {
        public GeneratedGuidConstraint(Expression<Func<T, Guid>> field)
            : base(field)
        {
        }

        protected override Guid Apply(Guid value)
        {
            // Workaround: Check if the current value is empty
            // Solution: extend NMemory, IConstaintContext parameter
            if (value == Guid.Empty)
            {
                return Guid.NewGuid();
            }
            else
            {
                return value;
            }
        }
    }
}
