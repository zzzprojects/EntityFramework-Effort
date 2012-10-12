using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;
using SoftwareApproach.TestingExtensions;

namespace Effort.Test
{
    [TestClass]
    public class JoinFixture2
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            //this context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

        [TestMethod]
        public void OuterJoin()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
            });
            context.SaveChanges();
            var query = from product in context.Products
                        join category in context.Categories on product.CategoryID
                        equals category.CategoryID into firstjoin
                        from cat in firstjoin.DefaultIfEmpty()
                        select new { product,cat };


            var products = query.ToList();
            products.Count.ShouldEqual(78);
            var anonymobject = products.FirstOrDefault(x => x.product.ProductName == "Special product");
            anonymobject.ShouldNotBeNull(); //The result contains the element
            anonymobject.cat.ShouldBeNull(); //The category is null
        }
    }
}
