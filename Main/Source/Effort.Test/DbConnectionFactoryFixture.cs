using System.Data.Common;
using Effort.DataLoaders;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class DbConnectionFactoryFixture
    {
        [TestMethod]
        public void CsvDataProvider()
        {
            string path = "C:\\";
            DbConnection connection = DbConnectionFactory.CreateTransient(new CsvDataLoader(path));

            EffortConnectionStringBuilder csb = new EffortConnectionStringBuilder(connection.ConnectionString);

            Assert.AreEqual(csb.DataProviderType, typeof(CsvDataLoader));
            Assert.AreEqual(csb.DataProviderArg, path);

        }
    }
}
