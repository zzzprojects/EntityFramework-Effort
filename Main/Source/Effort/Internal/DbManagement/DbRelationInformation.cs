// ----------------------------------------------------------------------------------
// <copyright file="DbRelationInformation.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.DbManagement
{
    using System;
    using System.Reflection;
    using NMemory.Indexes;

    internal class DbRelationInformation
    {
        public DbRelationInformation(
            string fromTable, 
            PropertyInfo[] fromProperties, 
            string toTable,
            PropertyInfo[] toProperties, 
            IKeyInfo primaryKeyInfo,
            IKeyInfo foreignKeyInfo, 
            Delegate primaryToForeignConverter,
            Delegate foreignToPrimaryConverter)
        {
            this.PrimaryTable = fromTable;
            this.ForeignTable = toTable;
            this.PrimaryProperties = fromProperties;
            this.ForeignProperties = toProperties;
            this.PrimaryKeyInfo = primaryKeyInfo;
            this.ForeignKeyInfo = foreignKeyInfo;
            this.PrimaryToForeignConverter = primaryToForeignConverter;
            this.ForeignToPrimaryConverter = foreignToPrimaryConverter;
        }

        public string PrimaryTable { get; private set; }

        public string ForeignTable { get; private set; }

        public PropertyInfo[] PrimaryProperties { get; private set; }

        public PropertyInfo[] ForeignProperties { get; private set; }

        // NMemory.IndexesAnonymousTypeKeyInfo<TEntity, TKey> Create<TEntity, TKey>
        public IKeyInfo PrimaryKeyInfo { get; private set; }

        public IKeyInfo ForeignKeyInfo { get; private set; }

        // Func<TPrimaryKey, TForeignKey>
        public Delegate PrimaryToForeignConverter { get; private set; }

        // Func<TForeignKey, TPrimaryKey>
        public Delegate ForeignToPrimaryConverter { get; private set; }
    }
}
