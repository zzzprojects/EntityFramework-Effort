// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaStore.cs" company="Effort Team">
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
    using System.Data.Metadata.Edm;
    using Effort.Internal.DbManagement;

    /// <summary>
    /// Represents a cache that stores <see cref="DbSchema"/> objects.
    /// </summary>
    internal static class DbSchemaStore
    {
        /// <summary>
        /// Internal collection.
        /// </summary>
        private static ConcurrentCache<DbSchemaKey, DbSchema> store;

        /// <summary>
        /// Initializes static members of the <see cref="DbSchemaStore" /> class.
        /// </summary>
        static DbSchemaStore()
        {
            store = new ConcurrentCache<DbSchemaKey, DbSchema>();
        }

        /// <summary>
        /// Returns a <see cref="DbSchema"/> object that is associated to the specified
        /// DbSchemaKey.
        /// </summary>
        /// <param name="schemaKey">The DbSchemaKey object.</param>
        /// <returns>The DbSchema object.</returns>
        public static DbSchema GetDbSchema(DbSchemaKey schemaKey)
        {
            return store.Get(schemaKey);
        }

        /// <summary>
        /// Returns a <see cref="DbSchema"/> object that represents the metadata contained by
        /// the specified StoreItemCollection. If no such element exist, the specified factory 
        /// method is used to create one.
        /// </summary>
        /// <param name="metadata">
        /// The StoreItemCollection object that contains the metadata.
        /// </param>
        /// <param name="schemaFactoryMethod">
        /// The factory method that instantiates the desired element.
        /// </param>
        /// <returns>
        /// The DbSchema object.
        /// </returns>
        public static DbSchema GetDbSchema(
            StoreItemCollection metadata, 
            Func<StoreItemCollection, DbSchema> schemaFactoryMethod)
        {
            return store.Get(
                new DbSchemaKey(metadata), 
                () => schemaFactoryMethod(metadata));
        }
    }
}
