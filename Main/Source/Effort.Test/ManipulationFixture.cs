#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System.Linq;
using Effort.Test.Data.Northwind;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class ManipulationFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = new LocalNorthwindObjectContext();
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

            Category cat1 = new Category();
            cat1.CategoryName = "Category 1";

            Category cat2 = new Category();
            cat2.CategoryName = "Category 2";

            this.context.Categories.AddObject(cat1);
            this.context.SaveChanges();

            this.context.Categories.AddObject(cat2);
            this.context.SaveChanges();

            Assert.AreEqual(cat1.CategoryID + 1, cat2.CategoryID);

            Category cat1b = this.context.Categories.Single(c => c.CategoryID == cat1.CategoryID);

            Assert.AreEqual(cat1.CategoryName, cat1b.CategoryName);
        }

        [TestMethod]
        public void Update()
        {
            var q = this.context.Products.Where(p => p.ProductID == 1);

            Product product = q.FirstOrDefault();

            string oldName = product.ProductName;
            string newName = "New name";

            product.ProductName = newName;
            this.context.SaveChanges();

            Assert.AreEqual(q.FirstOrDefault().ProductName, newName);
        }


        [TestMethod]
        public void Delete()
        {
            var q = this.context.OrderDetails.Where(p => p.OrderID == 10248 && p.ProductID == 11);

            OrderDetail detail = q.FirstOrDefault();

            this.context.OrderDetails.DeleteObject(detail);
            this.context.SaveChanges();

            Assert.AreEqual(q.FirstOrDefault(), null);
        }
    }
}
