// --------------------------------------------------------------------------------------------
// <copyright file="VarCharLimitConstraintFactory`1.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema.Constraints
{
    using NMemory.Common;
    using NMemory.Constraints;

    internal class VarCharLimitConstraintFactory<TEntity> : 
        ConstraintFactoryBase<TEntity, string>
    {
        private readonly int maxLength;

        public VarCharLimitConstraintFactory(
            IEntityMemberInfo<TEntity, string> member, 
            int maxLength)
            : base(member)
        {
            this.maxLength = maxLength;
        }

        protected override IConstraint<TEntity> Create(IEntityMemberInfo<TEntity, string> member)
        {
            return new NVarCharConstraint<TEntity>(member, this.maxLength);
        }
    }
}
