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
    public class CanonicalFunctionsFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            //this context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

        #region System.String Method (Static) Mapping

        [TestMethod]
        public void StringConcat()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                QuantityPerUnit = "kg",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Select(x => String.Concat(x.ProductName,x.QuantityPerUnit));
            var products = query.ToList();
            products.FirstOrDefault(x => x == "Special product"+"kg").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringContains()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Contains("Sp"));
            var products = query.ToList();
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringIsNullOrEmpty()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "",
                UnitPrice = -250,
                QuantityPerUnit = null

            });
            context.SaveChanges();

            var query = context.Products.Where(x => String.IsNullOrEmpty(x.ProductName) && String.IsNullOrEmpty(x.QuantityPerUnit));
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        #endregion

        #region System.String Method (Instance) Mapping

        [TestMethod]
        public void Trim()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "   One Product ",
                UnitPrice = -250,
                QuantityPerUnit = null

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Trim() == "One Product");
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        #endregion

        [TestMethod]
        public void MathAbs()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();
            
            var query = context.Products.Where(x => Math.Abs(x.UnitPrice.Value) == 250);
                        
            var products = query.ToList();
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull(); 
        }




        [TestMethod]
        public void StringStartsWith()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.StartsWith("Sp"));
            var lol = context.Products.Select(x => x.ProductName).ToList();
            var verylol = lol.Where(x => x.Contains("Sp")).ToList();
            var products = query.ToList();
            products.Count.ShouldEqual(2);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }


        [TestMethod]
        public void StringLength()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Length ==  15);

            var products = query.ToList();
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }
    }
}
