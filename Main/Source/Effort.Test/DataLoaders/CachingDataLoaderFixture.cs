// --------------------------------------------------------------------------------------------
// <copyright file="CachingDataLoaderFixture.cs" company="Effort Team">
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

namespace Effort.Test.DataLoaders
{
    using System.Linq;
    using Effort.DataLoaders;
    using Effort.Test.Internal.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class CachingDataLoaderFixture
    {
        [TestMethod]
        public void CachingDataLoader_Recreate()
        {
            CsvDataLoader wrapped = new CsvDataLoader("C:\\path");

            // Create a caching data loader
            CachingDataLoader original = new CachingDataLoader(wrapped);

            // Clone the caching data loader
            CachingDataLoader recreated = new CachingDataLoader();
            ((IDataLoader)recreated).Argument = ((IDataLoader)original).Argument;

            // It should be exactly the same as the original
            Assert.AreEqual(
                original.WrappedDataLoader.GetType(),
                recreated.WrappedDataLoader.GetType());

            Assert.IsInstanceOfType(
                recreated.WrappedDataLoader,
                typeof(CsvDataLoader));

            // The wrapped data loader should be restored completely too
            CsvDataLoader recreatedWrapped = recreated.WrappedDataLoader as CsvDataLoader;

            Assert.IsNotNull(recreatedWrapped);
            Assert.AreEqual(wrapped.ContainerFolderPath, recreatedWrapped.ContainerFolderPath);
        }

        [TestMethod]
        public void CachingDataLoaderFactory_SingleTablesSingleQuery()
        {
            var dataLoaderMock = new FakeDataLoader();
            var storeProxy = CreateFakeStoreProxy();

            ITableDataLoaderFactory factory = 
                new CachingTableDataLoaderFactory(dataLoaderMock, null, storeProxy);

            // First call
            var table = new TableDescription("table", Enumerable.Empty<ColumnDescription>());
            factory.CreateTableDataLoader(table);

            // Second call (should return the ITableDataLoader from cache)
            factory.CreateTableDataLoader(table);

            Assert.AreEqual(1, dataLoaderMock.CreateTableDataLoaderCallCount);
            Assert.AreEqual(1, storeProxy.CachedItemReturnCount);
        }

        [TestMethod]
        public void CachingDataLoaderFactory_MoreTablesMoreQuery()
        {
            var dataLoaderMock = new FakeDataLoader();
            var storeProxy = CreateFakeStoreProxy();

            ITableDataLoaderFactory factory = 
                new CachingTableDataLoaderFactory(dataLoaderMock, null, storeProxy);

            // First call
            var table1 = new TableDescription("table1", Enumerable.Empty<ColumnDescription>());
            factory.CreateTableDataLoader(table1);

            // Second call (another table, the cache should not be used)
            var table2 = new TableDescription("table2", Enumerable.Empty<ColumnDescription>());
            factory.CreateTableDataLoader(table2);

            Assert.AreEqual(2, dataLoaderMock.CreateTableDataLoaderCallCount);
            Assert.AreEqual(0, storeProxy.CachedItemReturnCount);
        }

        [TestMethod]
        public void CachingDataLoaderFactory_InvokesLatch()
        {
            var latchMock = new DataLoaderConfigurationLatchMock();
            var dataLoaderStub = new FakeDataLoader();
            var table = new TableDescription("table", Enumerable.Empty<ColumnDescription>());
            var storeProxy = CreateFakeStoreProxy();

            ITableDataLoaderFactory factory = 
                new CachingTableDataLoaderFactory(dataLoaderStub, latchMock, storeProxy);

            // The latch should not have been invoked yet
            Assert.AreEqual(0, latchMock.AcquireCallCount);
            Assert.AreEqual(0, latchMock.ReleaseCallCount);

            using (factory)
            {
                // This invocation should make the factory using the latch
                factory.CreateTableDataLoader(table);

                // The latch should have been acquired
                Assert.AreEqual(1, latchMock.AcquireCallCount);
                Assert.AreEqual(0, latchMock.ReleaseCallCount);
            }

            // The latch should have been released
            Assert.AreEqual(1, latchMock.AcquireCallCount);
            Assert.AreEqual(1, latchMock.ReleaseCallCount);
        }

        [TestMethod]
        public void CachingDataLoaderFactory_IgnoresLatch()
        {
            var latchMock = new DataLoaderConfigurationLatchMock();
            var stub = new FakeDataLoader();
            var storeProxy = CreateFakeStoreProxy();
            var table = new TableDescription("table", Enumerable.Empty<ColumnDescription>());

            ITableDataLoaderFactory factory =
                new CachingTableDataLoaderFactory(stub, latchMock, storeProxy);

            using (factory)
            {
                // The CreateTableDataLoader is not invoked
            }

            // The latch should not have been invoked
            Assert.AreEqual(0, latchMock.AcquireCallCount);
        }

        [TestMethod]
        public void CachingDataLoaderFactory_ReturnsSameData()
        {
            var latchMock = new DataLoaderConfigurationLatchMock();
            var stub = new FakeDataLoader();
            var storeProxy = CreateFakeStoreProxy();
            var table = new TableDescription("table", Enumerable.Empty<ColumnDescription>());

            CachingTableDataLoaderFactory factory =
                new CachingTableDataLoaderFactory(stub, latchMock, storeProxy);

            var data1 = factory.CreateTableDataLoader(table).GetData();

            var data2 = factory.CreateTableDataLoader(table).GetData();

            Enumerable.SequenceEqual(data1, data2);

            // The returned datasets should be exactly the same (same reference)
            Assert.AreEqual(data1, data2);
        }

        private static FakeCachingTableDataLoaderStoreProxy CreateFakeStoreProxy()
        {
            return new FakeCachingTableDataLoaderStoreProxy();
        }
    }
}
