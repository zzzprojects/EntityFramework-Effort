using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.EntityClient;
using System.Data.Objects;
using Effort.Test.Data.Northwind;

namespace Effort.Test
{
    [TestClass]
    public class EntityConnectionFactoryFixture
    {
        [TestMethod]
        public void EntityConnectionFactory_CreateTransientEntityConnection()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void EntityConnectionFactory_CreateTransientEntityConnection_InitializesDataSchema()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);

            using (ObjectContext context = new ObjectContext(connection))
            {
                Assert.IsTrue(context.DatabaseExists());
                Assert.AreEqual(0, context.CreateObjectSet<Product>().Count(), "Zero rows in the fake table");
            }
        }

        [TestMethod]
        public void EntityConnectionFactory_CreatePersistentEntityConnection()
        {
            // TODO: Use unique connection string

            EntityConnection connection = EntityConnectionFactory.CreatePersistent(NorthwindObjectContext.DefaultConnectionString);
        }
    }
}
