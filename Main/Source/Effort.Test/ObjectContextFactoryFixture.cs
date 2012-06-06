using System;
using Effort.Test.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class ObjectContextFactoryFixture
    {
        [TestMethod]
        public void CreatePersistentType()
        {
            Type type = ObjectContextFactory.CreatePersistentType<NorthwindEntities>("name=NorthwindEntities");

            object obj = Activator.CreateInstance(type);
        }

        [TestMethod]
        public void CreateTransientType()
        {
            ObjectContextFactory.CreatePersistentType<NorthwindEntities>("name=NorthwindEntities");
        }
    }
}
