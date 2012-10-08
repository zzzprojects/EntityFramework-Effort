using System.Data.EntityClient;
using System.Linq;
using Effort.DataLoaders;
using Effort.Test.Data;
using Effort.Test.Environment.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;
using Effort.Test.Environment;

namespace Effort.Test
{
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
