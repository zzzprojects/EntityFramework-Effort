// --------------------------------------------------------------------------------------------
// <copyright file="ExtendedTable`2.cs" company="Effort Team">
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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using NMemory.Indexes;
    using NMemory.Modularity;
    using NMemory.Tables;
    using NMemory.Transactions;

    internal class ExtendedTable<TEntity, TPrimaryKey> : 
        DefaultTable<TEntity, TPrimaryKey>,
        IExtendedTable<TEntity>
        where TEntity : class
    {
        private bool identityEnabled = true;
        private bool wasRecordAdded = false;

        public ExtendedTable(
            IDatabase database,
            IKeyInfo<TEntity, TPrimaryKey> primaryKey, 
            IdentitySpecification<TEntity> identity) 
            : base(database, primaryKey, identity)
        {
        }

        public bool IsIdentityFieldEnabled 
        {
            get
            {
                return this.identityEnabled;
            }

            set
            {
                if (this.identityEnabled == value)
                {
                    return;
                }

                this.identityEnabled = value;

                if (this.identityEnabled && this.wasRecordAdded)
                {
                    this.CalculateIdentityFeed();
                }

                this.wasRecordAdded = false;
            }
        }

        public void Initialize(IEnumerable<TEntity> entities)
        {
            if (this.Indexes.Count() > 1)
            {
                throw new InvalidOperationException();
            }

            if (this.PrimaryKeyIndex.Count > 0)
            {
                throw new InvalidOperationException();
            }

            foreach (TEntity entity in entities)
            {
                this.PrimaryKeyIndex.Insert(entity);
            }

            this.CalculateIdentityFeed();
        }

        public void Clear()
        {
            NMemory.Linq.QueryableEx.Delete(this);
        }

        protected override void InsertCore(TEntity entity, Transaction transaction)
        {
            base.InsertCore(entity, transaction);

            if (!this.identityEnabled)
            {
                this.wasRecordAdded = true;
            }
        }

        protected override void GenerateIdentityFieldValue(TEntity entity)
        {
            if (this.identityEnabled)
            {
                base.GenerateIdentityFieldValue(entity);
            }
        }



    }
}
