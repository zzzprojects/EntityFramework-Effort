// --------------------------------------------------------------------------------------------
// <copyright file="CanonicalFunctionsFixture.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Test.Features
{
    using System;
    using System.Data.Objects;
    using System.Linq;
    using Effort.Test.Data.Feature;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class CanonicalFunctionsFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            // This context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

        #region System.String Method (Static) Mapping

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

            var query = context.Products.Select(x => String.Concat(x.ProductName,x.QuantityPerUnit));

            var products = query.ToList();
            products.FirstOrDefault(x => x == "Special product"+"kg").ShouldNotBeNull();
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

            var query = this.context.Products.Where(x => String.IsNullOrEmpty(x.ProductName) && String.IsNullOrEmpty(x.QuantityPerUnit));
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        #endregion

        #region System.String Method (Instance) Mapping

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

            var query = this.context.Products.Where(x => x.ProductName.Contains("Sp"));
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

            var query = this.context.Products.Where(x => x.ProductName.EndsWith("ct"));
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

            var query = this.context.Products.Where(x => x.ProductName.StartsWith("Sp"));
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

            var query = this.context.Products.Where(x => x.ProductName.Length == 15);

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

            var query = this.context.Products.Where(x => x.ProductName.IndexOf("pe") == 1);
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

            var query = this.context.Products.Where(x => x.ProductName.Insert(1, "data") == "Sdatapecial product");
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

            var query = this.context.Products.Where(x => x.ProductName.Remove(3) == "Spe");
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

            var query = this.context.Products.Where(x => x.ProductName.Remove(1, 2) == "Scial product");
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

            var query = this.context.Products.Where(x => x.ProductName.Replace("product", "item") == "Special item of item");
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

            var query = this.context.Products.Where(x => x.ProductName.Substring(3) == "cial product");
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

            var query = this.context.Products.Where(x => x.ProductName.Substring(2,2) == "ec");
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

            var query = this.context.Products.Where(x => x.ProductName.ToLower() == "special product");
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

            var query = this.context.Products.Where(x => x.ProductName.ToUpper() == "SPECIAL PRODUCT");
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

            var query = this.context.Products.Where(x => x.ProductName.Trim() == "One Product");
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

            var query = this.context.Products.Where(x => x.ProductName.TrimEnd() == "  One Product");
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

            var query = this.context.Products.Where(x => x.ProductName.TrimStart() == "One Product ");
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        #endregion

        #region System.DateTime Method (Static) Mapping


        [TestMethod]
        public void DateTimeNow()
        {
            var query = this.context.Orders.Where(x => x.OrderDate > DateTime.Now);
            var orders = query.ToList();
            orders.Count.ShouldEqual(0);

            query = this.context.Orders.Where(x => x.OrderDate < DateTime.Now);
            orders = query.ToList();
            orders.Count.ShouldEqual(830);
        }

        [TestMethod]
        public void DateTimeUtcNow()
        {
            var query = this.context.Orders.Where(x => x.OrderDate > DateTime.UtcNow);
            var orders = query.ToList();
            orders.Count.ShouldEqual(0);

            query = this.context.Orders.Where(x => x.OrderDate < DateTime.UtcNow);
            orders = query.ToList();
            orders.Count.ShouldEqual(830);
        }

        #endregion

        #region System.DateTime Method (Instance) Mapping

        [TestMethod]
        public void DateTimeDay()
        {
            this.context.Orders.AddObject(
                new Order
                {
                    OrderDate = new DateTime(2012,1,2,3,4,5,100),
                    ShipName = "SuperShip"                

                });

            this.context.SaveChanges();

            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Day == 2);
            var orders = query.ToList();
            orders.FirstOrDefault(x=>x.ShipName == "SuperShip").ShouldNotBeNull();

        }

        [TestMethod]
        public void DateTimeDayHour()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5,100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Hour == 3);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMillisecond()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Millisecond == 100);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMinute()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Minute == 4);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMonth()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Month == 1);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDaySecond()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = this.context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Second == 5);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }

        [TestMethod]
        public void DateTimeDayYear()
        {
            this.context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            this.context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Year == 2012);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();
        }

        [TestMethod]
        public void DateTimeAdds()
        {
            var date = new DateTime(2012, 1, 2, 3, 4, 5, 100);
            context.Orders.AddObject(new Order
            {
                OrderDate = date,
                ShipName = "SuperShip"

            });
            context.SaveChanges();


            date = date.AddMilliseconds(-1);
            var query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddMilliseconds(date, 1).Value) == 0);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddMilliseconds(1).AddSeconds(-1);
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddSeconds(date, 1).Value) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddSeconds(1).AddMinutes(-1);
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddMinutes(date, 1).Value) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddMinutes(1).AddDays(-1);            
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddDays(date, 1).Value) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddDays(1).AddMonths(-1);            
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddMonths(date, 1).Value) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddMonths(1).AddYears(-1);
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(x.OrderDate.Value, EntityFunctions.AddYears(date, 1).Value) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

            date = date.AddYears(1).Date;
            query = context.Orders.Where(x => x.OrderDate.HasValue && DateTime.Compare(EntityFunctions.TruncateTime(x.OrderDate.Value).Value, date) == 0);
            orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();


        }

        #endregion

        #region System.DateTimeOffset Method (Instance) Mapping

        [TestMethod]
        public void DateTimeOffsetDay()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x=>x.Offset.HasValue && x.Offset.Value.Day == 2);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }

        [TestMethod]
        public void DateTimeOffsetDayHour()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Hour == 3);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }
        [TestMethod]
        public void DateTimeOffsetDayMillisecond()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x =>
                x.Offset.HasValue && 
                x.Offset.Value.Millisecond == 100);

            var orders = query.ToList();

            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }
        [TestMethod]
        public void DateTimeOffsetDayMinute()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Minute == 4);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }
        [TestMethod]
        public void DateTimeOffsetDayMonth()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Month == 1);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }
        [TestMethod]
        public void DateTimeOffsetDaySecond()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Second == 5);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }
        [TestMethod]
        public void DateTimeOffsetDayYear()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Year == 2012);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }



        #endregion

        #region System.DateTimeOffset Method (Static) Mapping

        [TestMethod]
        public void DateTimeOffsetCurrentDateTimeOffset()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "MyData",
                    Offset = DateTimeOffset.Now
                });

            context.SaveChanges();

            var query = context.PrimaryEntities.Where(x => x.Offset.HasValue && x.Offset.Value.Year == DateTimeOffset.Now.Year);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.PrimaryData == "MyData").ShouldNotBeNull();
        }

        #endregion

        #region Mathematical Function Mapping

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

            var query = context.OrderDetails.Where(x => Decimal.Ceiling(0.3m) == 1);

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

            var query = context.OrderDetails.Where(x => Decimal.Floor(0.3m) == 0);

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

            var query = context.OrderDetails.Where(x => Decimal.Round(0.3m) == 0);

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

            var query = context.OrderDetails.Where(x => Math.Ceiling(x.Discount) == 1);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x =>x.Quantity == -123).ShouldNotBeNull();
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

            var query = context.OrderDetails.Where(x => Math.Floor(x.Discount) == 0);

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

            var query = context.OrderDetails.Where(x => Math.Round(x.Discount) == 0);

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

            var query = this.context.OrderDetails.Where(x => Math.Round(x.Discount,2) == 123.40);

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

            var query = this.context.Products.Where(x => Math.Abs(x.UnitPrice.Value) == 250);

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

            var query = context.OrderDetails.Where(x=> Math.Pow(x.Quantity,2) == 123*123);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        #endregion
    }
}
