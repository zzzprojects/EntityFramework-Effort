using System.Data.Common;
using Effort.DataLoaders;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Staff;
using System.Linq;

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

            Assert.AreEqual(csb.DataLoaderType, typeof(CsvDataLoader));
            Assert.AreEqual(csb.DataLoaderArgument, path);
        }


        [TestMethod]
        public void A()
        {
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();

            using (StaffDbContext context = new StaffDbContext(connection))
            {
                context.People.ToList();
            }
        }

    }
}
