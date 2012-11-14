// --------------------------------------------------------------------------------------------
// <copyright file="TableInitialDataStore.cs" company="Effort Team">
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

namespace Effort.Internal.Caching
{
    using System;
    using Effort.DataLoaders;
    using Effort.Internal.DbManagement;

    /// <summary>
    /// Represents a cache that stores <see cref="TableInitialData"/> objects.
    /// </summary>
    internal static class TableInitialDataStore
    {
        /// <summary>
        /// Internal collection.
        /// </summary>
        private static ConcurrentCache<TableInitialDataKey, DbTableInitialData> store;

        /// <summary>
        /// Initializes static members of the the <see cref="TableInitialDataStore" /> class.
        /// </summary>
        static TableInitialDataStore()
        {
            store = new ConcurrentCache<TableInitialDataKey, DbTableInitialData>();
        }

        /// <summary>
        /// Returns a <see cref="DbTableInitialData"/> object that satisfies the specified 
        /// arguments. If no such element exists the provided factory method is used to create 
        /// one.
        /// </summary>
        /// <param name="loader">
        /// The data loader that fetches the data.
        /// </param>
        /// <param name="entityType">
        /// Type of the entities.
        /// </param>
        /// <param name="tableInitialDataFactoryMethod">
        /// The factory method that instatiates the desired DbTableInitialData object.
        /// </param>
        /// <returns></returns>
        public static DbTableInitialData GetDbInitialData(
            IDataLoader loader, 
            Type entityType, 
            Func<DbTableInitialData> tableInitialDataFactoryMethod)
        {
            TableInitialDataKey key =
                new TableInitialDataKey(loader.GetType(), loader.Argument, entityType);

            return store.Get(key, tableInitialDataFactoryMethod);
        }
    }
}
