using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.DataProviders;
using System.Data.Common;
using Effort.Provider;

namespace Effort.Test
{
    [TestClass]
    public class DbConnectionFactoryFixture
    {
        [TestMethod]
        public void CsvDataProvider()
        {
            string path = "C:\\";
            DbConnection connection = DbConnectionFactory.CreateTransient(new CsvDataProvider(path));

            EffortConnectionStringBuilder csb = new EffortConnectionStringBuilder(connection.ConnectionString);

            Assert.AreEqual(csb.DataProviderType, typeof(CsvDataProvider));
            Assert.AreEqual(csb.DataProviderArg, path);

        }
    }
}
