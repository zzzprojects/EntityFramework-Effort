// ----------------------------------------------------------------------------------
// <copyright file="DataReaderInspectorFixture.cs" company="Effort Team">
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

namespace Effort.Test
{
    using System.Data.EntityClient;
    using System.Linq;
    using Effort.DataLoaders;
    using Effort.Test.Data.Northwind;
    using Effort.Test.Environment;
    using Effort.Test.Environment.Fakes;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DataReaderInspectorFixture
    {
        [TestMethod]
        public void DataReaderInspector_ResultSetComposerShouldReceiveCalls()
        {
            IDataLoader dataLoader = new NorthwindLocalDataLoader();
            ResultSetComposerMock composerMock = new ResultSetComposerMock();

            EntityConnection inspectedFakeConnection = 
                EntityConnectionHelper.CreateInspectedFakeEntityConnection(
                    NorthwindObjectContext.DefaultConnectionString, 
                    composerMock, 
                    dataLoader);

            using (NorthwindObjectContext context = new NorthwindObjectContext(inspectedFakeConnection))
            {
                // ToList() call enumerates the result set
                context.Categories.ToList();
            }

            // The csv file contains 4 records
            Assert.AreEqual(8, composerMock.CommitCount);

            // Records has 4 fields
            Assert.AreEqual(8 * 4, composerMock.SetValueCount);
        }
    }
}
