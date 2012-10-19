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
        public void StringEndsWith()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.EndsWith("ct"));
            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
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

            var query = context.Products.Where(x => x.ProductName.Length == 15);

            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringIndexOf()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.IndexOf("pe") == 1);
            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringInsert()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Insert(1, "data") == "Sdatapecial product");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringRemove1()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Remove(3) == "Spe");
            var products = query.ToList();
            products.Count.ShouldBeLessThan(20);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringRemove2()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Remove(1, 2) == "Scial product");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringReplace()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product of product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Replace("product", "item") == "Special item of item");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product of product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringSubstring1()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Substring(3) == "cial product");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringSubstring2()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.Substring(2,2) == "ec");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringToLower()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.ToLower() == "special product");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

        [TestMethod]
        public void StringToUpper()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "Special product",
                UnitPrice = -250

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.ToUpper() == "SPECIAL PRODUCT");
            var products = query.ToList();
            products.Count.ShouldEqual(1);
            products.FirstOrDefault(x => x.ProductName == "Special product").ShouldNotBeNull();
        }

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

        [TestMethod]
        public void TrimEnd()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "  One Product ",
                UnitPrice = -250,

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.TrimEnd() == "  One Product");
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        [TestMethod]
        public void TrimStart()
        {
            context.Products.AddObject(new Product
            {
                ProductName = "  One Product ",
                UnitPrice = -250,

            });
            context.SaveChanges();

            var query = context.Products.Where(x => x.ProductName.TrimStart() == "One Product ");
            var products = query.ToList();
            products.FirstOrDefault(x => x.UnitPrice == -250).ShouldNotBeNull();
        }

        #endregion

        #region System.DateTime Method (Static) Mapping


        [TestMethod]
        public void DateTimeNow()
        {
            var query = context.Orders.Where(x => x.OrderDate > DateTime.Now);
            var orders = query.ToList();
            orders.Count.ShouldEqual(0);

            query = context.Orders.Where(x => x.OrderDate < DateTime.Now);
            orders = query.ToList();
            orders.Count.ShouldEqual(830);
        }

        [TestMethod]
        public void DateTimeUtcNow()
        {
            var query = context.Orders.Where(x => x.OrderDate > DateTime.UtcNow);
            var orders = query.ToList();
            orders.Count.ShouldEqual(0);

            query = context.Orders.Where(x => x.OrderDate < DateTime.UtcNow);
            orders = query.ToList();
            orders.Count.ShouldEqual(830);
        }

        #endregion

        #region System.DateTime Method (Instance) Mapping

        [TestMethod]
        public void DateTimeDay()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012,1,2,3,4,5,100),
                ShipName = "SuperShip"                

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Day == 2);
            var orders = query.ToList();
            orders.FirstOrDefault(x=>x.ShipName == "SuperShip").ShouldNotBeNull();

        }

        [TestMethod]
        public void DateTimeDayHour()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5,100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Hour == 3);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMillisecond()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Millisecond == 100);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMinute()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Minute == 4);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayMonth()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Month == 1);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDaySecond()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Second == 5);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }
        [TestMethod]
        public void DateTimeDayYear()
        {
            context.Orders.AddObject(new Order
            {
                OrderDate = new DateTime(2012, 1, 2, 3, 4, 5, 100),
                ShipName = "SuperShip"

            });
            context.SaveChanges();
            var query = context.Orders.Where(x => x.OrderDate.HasValue && x.OrderDate.Value.Year == 2012);
            var orders = query.ToList();
            orders.FirstOrDefault(x => x.ShipName == "SuperShip").ShouldNotBeNull();

        }



        #endregion

        #region Mathematical Function Mapping



        [TestMethod]
        public void MathCeiling()
        {
            context.OrderDetails.AddObject(new OrderDetail
            {
                OrderID = 1,
                Discount = 0.3f,
                Quantity = -123
            });
            context.SaveChanges();

            var query = context.OrderDetails.Where(x => Math.Ceiling(x.Discount) == 1);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x =>x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathFloor()
        {
            context.OrderDetails.AddObject(new OrderDetail
            {
                OrderID = 1,
                Discount = 0.3f,
                Quantity = -123
            });
            context.SaveChanges();

            var query = context.OrderDetails.Where(x => Math.Floor(x.Discount) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        [TestMethod]
        public void MathRound()
        {
            context.OrderDetails.AddObject(new OrderDetail
            {
                OrderID = 1,
                Discount = 0.3f,
                Quantity = -123
            });
            context.SaveChanges();

            var query = context.OrderDetails.Where(x => Math.Round(x.Discount,2) == 0);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

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
        public void MathPower()
        {
            context.OrderDetails.AddObject(new OrderDetail
            {
                OrderID = 1,
                Discount = 10.3f,
                Quantity = -123
            });
            context.SaveChanges();

            var query = context.OrderDetails.Where(x=> Math.Pow(x.Quantity,2) == 123*123);

            var orderdetails = query.ToList();
            orderdetails.FirstOrDefault(x => x.Quantity == -123).ShouldNotBeNull();
        }

        #endregion







    }
}
