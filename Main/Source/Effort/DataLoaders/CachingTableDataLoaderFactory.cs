// --------------------------------------------------------------------------------------------
// <copyright file="CachingTableDataLoaderFactory.cs" company="Effort Team">
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

    public class CachingTableDataLoaderFactory : ITableDataLoaderFactory
    {
        private IDataLoader wrappedDataLoader;
        private ITableDataLoaderFactory wrappedTableDataLoaderFactory;
        private IDataLoaderConfigurationLatch latch;
        private ICachingTableDataLoaderStoreProxy dataStoreProxy;

        public CachingTableDataLoaderFactory(IDataLoader wrappedDataLoader)
            : this(
                wrappedDataLoader, 
                CreateLatch(wrappedDataLoader),
                new CachingTableDataLoaderStoreProxy())
        {
        }

        internal CachingTableDataLoaderFactory(
            IDataLoader wrappedDataLoader,
            IDataLoaderConfigurationLatch latch,
            ICachingTableDataLoaderStoreProxy dataStoreProxy)
        {
            if (wrappedDataLoader == null)
            {
                throw new ArgumentNullException("wrappedDataLoader");
            }

            if (dataStoreProxy == null)
            {
                throw new ArgumentNullException("dataStoreProxy");
            }

            this.wrappedDataLoader = wrappedDataLoader;
            this.latch = latch;
            this.dataStoreProxy = dataStoreProxy;
        }

        public ITableDataLoader CreateTableDataLoader(TableDescription table)
        {
            CachingTableDataLoaderKey key =
                new CachingTableDataLoaderKey(
                    new DataLoaderConfigurationKey(this.wrappedDataLoader), 
                    table.Name);

            if (latch != null)
            {
                if (!this.dataStoreProxy.Contains(key))
                {
                    latch.Acquire();
                }
            }

            return this.dataStoreProxy.GetCachedData(key, () => CreateCachedData(table));
        }

        public void Dispose()
        {
            if (this.wrappedTableDataLoaderFactory != null)
            {
                this.wrappedTableDataLoaderFactory.Dispose();

                // Release the data loader latch
                if (this.latch != null)
                {
                    this.latch.Release();
                }
            }
        }

        private static IDataLoaderConfigurationLatch CreateLatch(IDataLoader dataLoader)
        {
            DataLoaderConfigurationKey key = new DataLoaderConfigurationKey(dataLoader);

            return new DataLoaderConfigurationLatchProxy(key);
        }

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
