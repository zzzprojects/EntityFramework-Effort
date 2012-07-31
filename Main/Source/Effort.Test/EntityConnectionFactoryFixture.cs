using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.EntityClient;
using System.Data.Objects;

namespace Effort.Test
{
    [TestClass]
    public class EntityConnectionFactoryFixture
    {
        [TestMethod]
        public void CreateTransientEntityConnection()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient("name=NorthwindEntities");
        }

        [TestMethod]
        public void EntityConnectionFactoryInitializesDataScheme()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient("name=NorthwindEntities");

            using (ObjectContext context = new ObjectContext(connection))
            {
                Assert.IsTrue(context.DatabaseExists());
                Assert.AreEqual(0, context.CreateObjectSet<Data.Products>().Count(), "Zero rows in the fake table");
            }
        }
    }
}
