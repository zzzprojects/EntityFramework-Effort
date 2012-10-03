using System;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;

namespace Effort.Test
{
    [TestClass]
    public class ObjectContextFactoryFixture
    {
        [TestMethod]
        public void ObjectContextFactory_CreatePersistentType()
        {
            ObjectContextFactory.CreatePersistentType<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransientType()
        {
            ObjectContextFactory.CreateTransientType<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void ObjectContextFactory_CreatePersistent()
        {
            ObjectContextFactory.CreatePersistent<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransient()
        {
            ObjectContextFactory.CreateTransient<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);
        }


        [TestMethod]
        public void ObjectContextFactory_CreatePersistentType_Default()
        {
            ObjectContextFactory.CreatePersistentType<NorthwindObjectContext>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransientType_Default()
        {
            ObjectContextFactory.CreateTransientType<NorthwindObjectContext>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreatePersistent_Default()
        {
            ObjectContextFactory.CreatePersistent<NorthwindObjectContext>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransient_Default()
        {
            ObjectContextFactory.CreateTransient<NorthwindObjectContext>();
        }

    }
}
