// --------------------------------------------------------------------------------------------
// <copyright file="ExtendedTableService.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
    using NMemory.Indexes;
    using NMemory.Modularity;
    using NMemory.Services.Contracts;
    using NMemory.Tables;

    internal class ExtendedTableService : ITableService
    {
        public Table<TEntity, TPrimaryKey> CreateTable<TEntity, TPrimaryKey>(
            IKeyInfo<TEntity, TPrimaryKey> primaryKey, 
            IdentitySpecification<TEntity> identitySpecification, 
            IDatabase database) 
            where TEntity : class
        {
            return new ExtendedTable<TEntity, TPrimaryKey>(
                database,
                primaryKey,
                identitySpecification);
        }
    }
}
