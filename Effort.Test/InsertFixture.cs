using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Utils;
using Effort.Test.Data;

namespace Effort.Test
{
    [TestClass]
    public class InsertFixture
    {
        private NorthwindEntities context;

        [TestInitialize]
        public void Initialize()
        {
            ////this.context = new NorthwindEntities("name=NorthwindEntities");
            this.context = new NorthwindEntitiesWrapped("name=NorthwindEntities");
        }

        [TestMethod]
        public void InsertIdentity()
        {

            Categories cat1 = new Categories();
            cat1.CategoryName = "Category 1";

            Categories cat2 = new Categories();
            cat2.CategoryName = "Category 2";

            this.context.Categories.AddObject(cat1);
            this.context.SaveChanges();

            this.context.Categories.AddObject(cat2);
            this.context.SaveChanges();

            Assert.AreEqual(cat1.CategoryID + 1, cat2.CategoryID);

            Categories cat1b = this.context.Categories.Single(c => c.CategoryID == cat1.CategoryID);

            Assert.AreEqual(cat1.CategoryName, cat1b.CategoryName);

            this.context.DeleteObject(cat1);
            this.context.DeleteObject(cat2);
            this.context.SaveChanges();


        }
    }
}
