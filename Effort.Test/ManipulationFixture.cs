using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Utils;
using Effort.Test.Data;
using System.Transactions;

namespace Effort.Test
{
    [TestClass]
    public class ManipulationFixture
    {
        private NorthwindEntities context;

        [AssemblyInitialize]
        public static void RegisterProviders(TestContext context)
        {
            DatabaseEmulatorProviderConfiguration.RegisterProvider();
        }

        [TestInitialize]
        public void Initialize()
        {
            this.context = new NorthwindEntitiesEmulated();
        }

        [TestMethod]
        public void ManipulationInitialData()
        {
            Assert.AreEqual(this.context.Categories.Count(), 8);
            Assert.AreEqual(this.context.Products.Count(), 77);
        }

        [TestMethod]
        public void InsertWithIdentity()
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
        }

        [TestMethod]
        public void Update()
        {
            var q = this.context.Products.Where(p => p.ProductID == 1);

            Products product = q.FirstOrDefault();

            string oldName = product.ProductName;
            string newName = "New name";

            product.ProductName = newName;
            this.context.SaveChanges();

            Assert.AreEqual(q.FirstOrDefault().ProductName, newName);
        }


        [TestMethod]
        public void Delete()
        {
            var q = this.context.Order_Details.Where(p => p.OrderID == 10248 && p.ProductID == 11);

            Order_Details detail = q.FirstOrDefault();

            this.context.Order_Details.DeleteObject(detail);
            this.context.SaveChanges();

            Assert.AreEqual(q.FirstOrDefault(), null);
        }
    }
}
