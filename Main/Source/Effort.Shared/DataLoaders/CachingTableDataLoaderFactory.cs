// --------------------------------------------------------------------------------------------
// <copyright file="CachingTableDataLoaderFactory.cs" company="Effort Team">
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

namespace Effort.DataLoaders
{
    using System;
    using Effort.Internal.Caching;

    /// <summary>
    ///     Represents a table data loader factory that creates 
    ///     <see cref="CachingTableDataLoader"/> instances for tables.
    /// </summary>
    public class CachingTableDataLoaderFactory : ITableDataLoaderFactory
    {
        /// <summary>
        ///     The wrapped data loader.
        /// </summary>
        private IDataLoader wrappedDataLoader;

        /// <summary>
        ///     The table data loader factory retrieved from the wrapped data loader if neeed.
        /// </summary>
        private ITableDataLoaderFactory wrappedTableDataLoaderFactory;

        /// <summary>
        ///     The latch that locks the entire configuration of the wrapped data loader in
        ///     order to make it be used only once during the caching phase.
        /// </summary>
        private IDataLoaderConfigurationLatch latch;

        /// <summary>
        ///     The store that contains the cached table data.
        /// </summary>
        private ICachingTableDataLoaderStore dataStore;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTableDataLoaderFactory" /> 
        ///     class.
        /// </summary>
        /// <param name="wrappedDataLoader"> 
        ///     The wrapped data loader.
        /// </param>
        public CachingTableDataLoaderFactory(IDataLoader wrappedDataLoader)
            : this(wrappedDataLoader, false)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTableDataLoaderFactory" /> 
        ///     class. 
        ///     Enabling the <paramref name="locking"/> flag makes the caching factory 
        ///     instances to work in a cooperative way. They ensure that only one of wrapped
        ///     factory objects initialized with the same configuration is utilized at the same
        ///     time. 
        /// </summary>
        /// <param name="wrappedDataLoader"> 
        ///     The wrapped data loader. 
        /// </param>
        /// <param name="locking"> 
        ///     Indicates if the wrapped data loader should be used only once at the same time.
        /// </param>
        public CachingTableDataLoaderFactory(IDataLoader wrappedDataLoader, bool locking)
            : this(
                wrappedDataLoader,
                locking ? CreateLatch(wrappedDataLoader) : null,
                new CachingTableDataLoaderStoreProxy())
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTableDataLoaderFactory" /> 
        ///     class.
        /// </summary>
        /// <param name="wrappedDataLoader"> The wrapped data loader. </param>
        /// <param name="latch"> The latch that locks the data loader configuration. </param>
        /// <param name="dataStore"> The store that contains the cached data. </param>
        internal CachingTableDataLoaderFactory(
            IDataLoader wrappedDataLoader,
            IDataLoaderConfigurationLatch latch,
            ICachingTableDataLoaderStore dataStore)
        {
            if (wrappedDataLoader == null)
            {
                throw new ArgumentNullException("wrappedDataLoader");
            }

            if (dataStore == null)
            {
                throw new ArgumentNullException("dataStoreProxy");
            }

            this.wrappedDataLoader = wrappedDataLoader;
            this.latch = latch;
            this.dataStore = dataStore;
        }

        /// <summary>
        ///     Creates a data loader for the specified table.
        /// </summary>
        /// <param name="table"> The metadata of the table. </param>
        /// <returns>
        ///     The data loader for the table.
        /// </returns>
        public ITableDataLoader CreateTableDataLoader(TableDescription table)
        {
            CachingTableDataLoaderKey key =
                new CachingTableDataLoaderKey(
                    new DataLoaderConfigurationKey(this.wrappedDataLoader), 
                    table.Name);

            // If the table data cache does not exists, then the data loader configuration
            // should be locked
            if (latch != null && !this.dataStore.Contains(key))
            {
                // Wait for the lock, this could take some time
                latch.Acquire();

                // Check if the data was created since the waiting
                if (this.dataStore.Contains(key))
                {
                    latch.Release();
                }
            }
            
            // It does not matter if the table data cache was created during the waiting,
            // maybe there is still tables thats data is not fetched
            return this.dataStore.GetCachedData(key, () => CreateCachedData(table));
        }

        /// <summary>
        ///     Disposes the wrapped data loader table factory and releases the latch on the
        ///     wrapped data loader configuration.
        /// </summary>
        public void Dispose()
        {
            // Release the wrapped table loader factory
            if (this.wrappedTableDataLoaderFactory != null)
            {
                this.wrappedTableDataLoaderFactory.Dispose();
            }

            // Release the data loader latch
            if (this.latch != null)
            {
                this.latch.Release();
            }
        }

        /// <summary>
        ///     Creates the default latch for the data loader configuration locking.
        /// </summary>
        /// <param name="dataLoader"> The data loader. </param>
        /// <returns> The latch. </returns>
        private static IDataLoaderConfigurationLatch CreateLatch(IDataLoader dataLoader)
        {
            DataLoaderConfigurationKey key = new DataLoaderConfigurationKey(dataLoader);

            return new DataLoaderConfigurationLatchProxy(key);
        }

        /// <summary>
        ///     Creates a proxy for the global table data cache.
        /// </summary>
        /// <param name="table"> The table metadata. </param>
        /// <returns> The proxy for the cache. </returns>
        private CachingTableDataLoader CreateCachedData(TableDescription table)
        {
            if (this.wrappedTableDataLoaderFactory == null)
            {
                this.wrappedTableDataLoaderFactory =
                    this.wrappedDataLoader.CreateTableDataLoaderFactory();
            }

            ITableDataLoader wrappedTableDataLoader =
                this.wrappedTableDataLoaderFactory.CreateTableDataLoader(table);

            return new CachingTableDataLoader(wrappedTableDataLoader);
        }
    }
}
