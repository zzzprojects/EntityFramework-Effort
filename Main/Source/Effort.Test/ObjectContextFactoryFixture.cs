using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data;

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
