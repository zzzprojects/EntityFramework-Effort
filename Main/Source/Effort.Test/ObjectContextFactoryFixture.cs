using System;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class ObjectContextFactoryFixture
    {
        [TestMethod]
        public void ObjectContextFactory_CreatePersistentType()
        {
            ObjectContextFactory.CreatePersistentType<NorthwindEntities>("name=NorthwindEntities");
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransientType()
        {
            ObjectContextFactory.CreateTransientType<NorthwindEntities>("name=NorthwindEntities");
        }

        [TestMethod]
        public void ObjectContextFactory_CreatePersistent()
        {
            ObjectContextFactory.CreatePersistent<NorthwindEntities>("name=NorthwindEntities");
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransient()
        {
            ObjectContextFactory.CreateTransient<NorthwindEntities>("name=NorthwindEntities");
        }


        [TestMethod]
        public void ObjectContextFactory_CreatePersistentType_Default()
        {
            ObjectContextFactory.CreatePersistentType<NorthwindEntities>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransientType_Default()
        {
            ObjectContextFactory.CreateTransientType<NorthwindEntities>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreatePersistent_Default()
        {
            ObjectContextFactory.CreatePersistent<NorthwindEntities>();
        }

        [TestMethod]
        public void ObjectContextFactory_CreateTransient_Default()
        {
            ObjectContextFactory.CreateTransient<NorthwindEntities>();
        }

    }
}
