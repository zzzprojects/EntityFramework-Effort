// --------------------------------------------------------------------------------------------
// <copyright file="CachingDataLoaderFixture.cs" company="Effort Team">
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

namespace Effort.Test.DataLoaders
{
    using System.Data.Common;
    using System.Linq;
    using Effort.Test.Data.Staff;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Effort.DataLoaders;

    [TestClass]
    public class CachingDataLoaderFixture
    {
        [TestMethod]
        public void CachingDataLoader_Recreate()
        {
            CsvDataLoader wrapped = new CsvDataLoader("C:\\path");

            CachingDataLoader original = new CachingDataLoader(wrapped);

            CachingDataLoader recreated = new CachingDataLoader();
            ((IDataLoader)recreated).Argument = ((IDataLoader)original).Argument;

            Assert.AreEqual(
                original.WrappedDataLoader.GetType(),
                recreated.WrappedDataLoader.GetType());

            Assert.IsInstanceOfType(
                recreated.WrappedDataLoader,
                typeof(CsvDataLoader));

            CsvDataLoader recreatedWrapped = recreated.WrappedDataLoader as CsvDataLoader;

            Assert.AreEqual(
                wrapped.ContainerFolderPath,
                recreatedWrapped.ContainerFolderPath);
        }
    }
}
