// --------------------------------------------------------------------------------------------
// <copyright file="MathFunctionsFixture.cs" company="Effort Team">
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
    using System;
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class MathFunctionsFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            // This context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

        [TestMethod]
        public void DecimalCeiling()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Decimal.Ceiling(0.3m) == 1);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void DecimalFloor()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x =>
                    Decimal.Floor(0.3m) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void DecimalRound()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Decimal.Round(0.3m) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }


        [TestMethod]
        public void MathCeiling()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Math.Ceiling(x.Discount) == 1);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathFloor()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Math.Floor(x.Discount) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathRound()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 0.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Math.Round(x.Discount) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathRoundWithDigits()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 123.396f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Math.Round(x.Discount, 2) == 123.40);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathAbs()
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
                    Math.Abs(x.UnitPrice.Value) == 250);

            var products = query.ToList();
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void MathPower()
        {
            this.context.OrderDetails.AddObject(
                new OrderDetail
                {
                    OrderID = 10248,
                    ProductID = 1,
                    Discount = 10.3f,
                    Quantity = -123
                });

            this.context.SaveChanges();

            var query = this.context
                .OrderDetails
                .Where(x => 
                    Math.Pow(x.Quantity, 2) == 123 * 123);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }
    }
}
