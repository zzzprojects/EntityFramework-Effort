// --------------------------------------------------------------------------------------------
// <copyright file="StringFunctionsFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Features.CanonicalFunctions
{
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class StringFunctionsFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            // This context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

         [TestMethod]
        public void StringConcat()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    QuantityPerUnit = "kg",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Select(x => 
                    string.Concat(x.ProductName,x.QuantityPerUnit));

            var products = query.ToList();
            products.FirstOrDefault(x => x == "Special product" + "kg").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringIsNullOrEmpty()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "",
                    UnitPrice = -250,
                    QuantityPerUnit = null

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    string.IsNullOrEmpty(x.ProductName) && 
                    string.IsNullOrEmpty(x.QuantityPerUnit));

            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        [TestMethod]
        public void StringContains()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250
                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Contains("Sp"));

            var products = query.ToList();
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringEndsWith()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.EndsWith("ct"));

            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringStartsWith()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.StartsWith("Sp"));

            var products = query.ToList();
            products.Count.ShouldEqual(2);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringLength()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Length == 15);

            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringIndexOf()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.IndexOf("pe") == 1);

            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringInsert()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Insert(1, "data") == "Sdatapecial product");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringRemove1()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Remove(3) == "Spe");

            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringRemove2()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Remove(1, 2) == "Scial product");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringReplace()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product of product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Replace("product", "item") == "Special item of item");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product of product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringSubstring1()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Substring(3) == "cial product");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringSubstring2()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Substring(2,2) == "ec");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringToLower()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.ToLower() == "special product");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringToUpper()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "Special product",
                    UnitPrice = -250

                });

            this.context.SaveChanges();

            var query = 
                this.context.Products.Where(x => 
                    x.ProductName.ToUpper() == "SPECIAL PRODUCT");

            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void Trim()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "   One Product ",
                    UnitPrice = -250,
                    QuantityPerUnit = null

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.Trim() == "One Product");

            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        [TestMethod]
        public void TrimEnd()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "  One Product ",
                    UnitPrice = -250,

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.TrimEnd() == "  One Product");

            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        [TestMethod]
        public void TrimStart()
        {
            this.context.Products.AddObject(
                new Product
                {
                    ProductName = "  One Product ",
                    UnitPrice = -250,

                });

            this.context.SaveChanges();

            var query = this.context
                .Products
                .Where(x => 
                    x.ProductName.TrimStart() == "One Product ");

            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }
    }
}
