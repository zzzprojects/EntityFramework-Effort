using System.Data.Common;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Staff;

namespace Effort.Test
{
    [TestClass]
    public class DbContextFixture
    {
        [TestMethod]
        public void DbContext_Create()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            bool created1 = context.Database.CreateIfNotExists();
            bool created2 = context.Database.CreateIfNotExists();

            Assert.IsTrue(created1);
            Assert.IsFalse(created2);
        }

        [TestMethod]
        public void DbContext_Insert()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void DbContext_Query()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            context.SaveChanges();

            Person person = context.People.Single();

            Assert.AreEqual("John", person.FirstName);
            Assert.AreEqual("Doe", person.LastName);
        }

        [TestMethod]
        public void DbContext_Delete()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            context.SaveChanges();

            Person person = context.People.Single();

            context.People.Remove(person);
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
            Assert.AreEqual(0, context.People.Count());
        }
    }
}
