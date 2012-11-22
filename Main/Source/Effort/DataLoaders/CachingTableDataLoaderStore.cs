// --------------------------------------------------------------------------------------------
// <copyright file="CachingTableDataLoaderStore.cs" company="Effort Team">
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

namespace Effort.DataLoaders
{
    using System;
    using Effort.Internal.Caching;

    /// <summary>
    /// Represents a cache that stores <see cref="CachedDataLoaderData"/> objects.
    /// </summary>
    internal static class CachingTableDataLoaderStore
    {
        /// <summary>
        /// Internal collection.
        /// </summary>
        private static ConcurrentCache<
            CachingTableDataLoaderKey, 
            CachingTableDataLoader> store;

        /// <summary>
        /// Initializes static members of the the <see cref="CachingTableDataLoaderStore" /> 
        /// class.
        /// </summary>
        static CachingTableDataLoaderStore()
        {
            store = new ConcurrentCache<
                CachingTableDataLoaderKey,
                CachingTableDataLoader>();
        }

        /// <summary>
        /// Returns a <see cref="CachedDataLoaderData"/> object that satisfies the specified 
        /// arguments. If no such element exists the provided factory method is used to create 
        /// one.
        /// </summary>
        /// <param name="loader">
        /// The data loader that fetches the data.
        /// </param>
        /// <param name="tableName">
        /// Name of the table.
        /// </param>
        /// <param name="factoryMethod">
        /// The factory method that instatiates the desired CachedDataLoaderTableData object.
        /// </param>
        /// <returns></returns>
        public static CachingTableDataLoader GetCachedData(
            IDataLoader loader,
            string tableName,
            Func<CachingTableDataLoader> factoryMethod)
        {
            CachingTableDataLoaderKey key =
                new CachingTableDataLoaderKey(loader.GetType(), loader.Argument, tableName);

            return store.Get(key, factoryMethod);
        }
    }
}
