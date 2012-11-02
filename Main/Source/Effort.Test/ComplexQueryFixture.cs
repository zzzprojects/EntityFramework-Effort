// ----------------------------------------------------------------------------------
// <copyright file="ComplexQueryFixture.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Test
{
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Effort.Test.Environment.Queries;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ComplexQueryFixture
    {
        private IQueryTester<NorthwindObjectContext> tester;

        [TestInitialize]
        public void Initialize()
        {
            this.tester = new NorthwindQueryTester();
        }

        [TestMethod]
        public void Products_That_Has_Higher_Unit_Price_Than_Average()
        {
            string expected = "[{\"ProductID\":7,\"ProductName\":\"Uncle Bob's Organic Dried Pears\",\"SupplierID\":3,\"CategoryID\":7,\"QuantityPerUnit\":\"12 - 1 lb pkgs.\",\"UnitPrice\":30.0,\"UnitsInStock\":15,\"UnitsOnOrder\":0,\"ReorderLevel\":10},{\"ProductID\":8,\"ProductName\":\"Northwoods Cranberry Sauce\",\"SupplierID\":3,\"CategoryID\":2,\"QuantityPerUnit\":\"12 - 12 oz jars\",\"UnitPrice\":40.0,\"UnitsInStock\":6,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":9,\"ProductName\":\"Mishi Kobe Niku\",\"SupplierID\":4,\"CategoryID\":6,\"QuantityPerUnit\":\"18 - 500 g pkgs.\",\"UnitPrice\":97.0,\"UnitsInStock\":29,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":10,\"ProductName\":\"Ikura\",\"SupplierID\":4,\"CategoryID\":8,\"QuantityPerUnit\":\"12 - 200 ml jars\",\"UnitPrice\":31.0,\"UnitsInStock\":31,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":12,\"ProductName\":\"Queso Manchego La Pastora\",\"SupplierID\":5,\"CategoryID\":4,\"QuantityPerUnit\":\"10 - 500 g pkgs.\",\"UnitPrice\":38.0,\"UnitsInStock\":86,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":17,\"ProductName\":\"Alice Mutton\",\"SupplierID\":7,\"CategoryID\":6,\"QuantityPerUnit\":\"20 - 1 kg tins\",\"UnitPrice\":39.0,\"UnitsInStock\":0,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":18,\"ProductName\":\"Carnarvon Tigers\",\"SupplierID\":7,\"CategoryID\":8,\"QuantityPerUnit\":\"16 kg pkg.\",\"UnitPrice\":62.5,\"UnitsInStock\":42,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":20,\"ProductName\":\"Sir Rodney's Marmalade\",\"SupplierID\":8,\"CategoryID\":3,\"QuantityPerUnit\":\"30 gift boxes\",\"UnitPrice\":81.0,\"UnitsInStock\":40,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":26,\"ProductName\":\"Gumbär Gummibärchen\",\"SupplierID\":11,\"CategoryID\":3,\"QuantityPerUnit\":\"100 - 250 g bags\",\"UnitPrice\":31.23,\"UnitsInStock\":15,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":27,\"ProductName\":\"Schoggi Schokolade\",\"SupplierID\":11,\"CategoryID\":3,\"QuantityPerUnit\":\"100 - 100 g pieces\",\"UnitPrice\":43.9,\"UnitsInStock\":49,\"UnitsOnOrder\":0,\"ReorderLevel\":30},{\"ProductID\":28,\"ProductName\":\"Rössle Sauerkraut\",\"SupplierID\":12,\"CategoryID\":7,\"QuantityPerUnit\":\"25 - 825 g cans\",\"UnitPrice\":45.6,\"UnitsInStock\":26,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":29,\"ProductName\":\"Thüringer Rostbratwurst\",\"SupplierID\":12,\"CategoryID\":6,\"QuantityPerUnit\":\"50 bags x 30 sausgs.\",\"UnitPrice\":123.79,\"UnitsInStock\":0,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":32,\"ProductName\":\"Mascarpone Fabioli\",\"SupplierID\":14,\"CategoryID\":4,\"QuantityPerUnit\":\"24 - 200 g pkgs.\",\"UnitPrice\":32.0,\"UnitsInStock\":9,\"UnitsOnOrder\":40,\"ReorderLevel\":25},{\"ProductID\":38,\"ProductName\":\"Côte de Blaye\",\"SupplierID\":18,\"CategoryID\":1,\"QuantityPerUnit\":\"12 - 75 cl bottles\",\"UnitPrice\":263.5,\"UnitsInStock\":17,\"UnitsOnOrder\":0,\"ReorderLevel\":15},{\"ProductID\":43,\"ProductName\":\"Ipoh Coffee\",\"SupplierID\":20,\"CategoryID\":1,\"QuantityPerUnit\":\"16 - 500 g tins\",\"UnitPrice\":46.0,\"UnitsInStock\":17,\"UnitsOnOrder\":10,\"ReorderLevel\":25},{\"ProductID\":51,\"ProductName\":\"Manjimup Dried Apples\",\"SupplierID\":24,\"CategoryID\":7,\"QuantityPerUnit\":\"50 - 300 g pkgs.\",\"UnitPrice\":53.0,\"UnitsInStock\":20,\"UnitsOnOrder\":0,\"ReorderLevel\":10},{\"ProductID\":53,\"ProductName\":\"Perth Pasties\",\"SupplierID\":24,\"CategoryID\":6,\"QuantityPerUnit\":\"48 pieces\",\"UnitPrice\":32.8,\"UnitsInStock\":0,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":56,\"ProductName\":\"Gnocchi di nonna Alice\",\"SupplierID\":26,\"CategoryID\":5,\"QuantityPerUnit\":\"24 - 250 g pkgs.\",\"UnitPrice\":38.0,\"UnitsInStock\":21,\"UnitsOnOrder\":10,\"ReorderLevel\":30},{\"ProductID\":59,\"ProductName\":\"Raclette Courdavault\",\"SupplierID\":28,\"CategoryID\":4,\"QuantityPerUnit\":\"5 kg pkg.\",\"UnitPrice\":55.0,\"UnitsInStock\":79,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":60,\"ProductName\":\"Camembert Pierrot\",\"SupplierID\":28,\"CategoryID\":4,\"QuantityPerUnit\":\"15 - 300 g rounds\",\"UnitPrice\":34.0,\"UnitsInStock\":19,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":62,\"ProductName\":\"Tarte au sucre\",\"SupplierID\":29,\"CategoryID\":3,\"QuantityPerUnit\":\"48 pies\",\"UnitPrice\":49.3,\"UnitsInStock\":17,\"UnitsOnOrder\":0,\"ReorderLevel\":0},{\"ProductID\":63,\"ProductName\":\"Vegie-spread\",\"SupplierID\":7,\"CategoryID\":2,\"QuantityPerUnit\":\"15 - 625 g jars\",\"UnitPrice\":43.9,\"UnitsInStock\":24,\"UnitsOnOrder\":0,\"ReorderLevel\":5},{\"ProductID\":64,\"ProductName\":\"Wimmers gute Semmelknödel\",\"SupplierID\":12,\"CategoryID\":5,\"QuantityPerUnit\":\"20 bags x 4 pieces\",\"UnitPrice\":33.25,\"UnitsInStock\":22,\"UnitsOnOrder\":80,\"ReorderLevel\":30},{\"ProductID\":69,\"ProductName\":\"Gudbrandsdalsost\",\"SupplierID\":15,\"CategoryID\":4,\"QuantityPerUnit\":\"10 kg pkg.\",\"UnitPrice\":36.0,\"UnitsInStock\":26,\"UnitsOnOrder\":0,\"ReorderLevel\":15},{\"ProductID\":72,\"ProductName\":\"Mozzarella di Giovanni\",\"SupplierID\":14,\"CategoryID\":4,\"QuantityPerUnit\":\"24 - 200 g pkgs.\",\"UnitPrice\":34.8,\"UnitsInStock\":14,\"UnitsOnOrder\":0,\"ReorderLevel\":0}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from p in context.Products
                    where p.UnitPrice > context.Products.Average(x => x.UnitPrice)
                    select p

                    , expected
                );

            Assert.IsTrue(result.Check());
        }

        [TestMethod]
        public void Name_Of_The_Customer_Who_Has_The_Most_Orders_Out_Of_The_Top_Ten_Customers()
        {
            string expected = "[{\"ContactName\":\"Jose Pavarotti\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                     (
                     from cus in context.Customers
                     orderby cus
                         .Orders
                         .SelectMany(o => o.OrderDetails)
                         .Sum(od => od.UnitPrice * od.Quantity)
                         descending
                     select
                         cus

                    )
                    .Take(10)
                    .OrderByDescending(c => c.Orders.Count)
                    .Select(c => c.ContactName)
                    .FirstOrDefault()

                    , expected
                );

            Assert.IsTrue(result.Check());
        }

        [TestMethod]
        public void Date_Of_The_Last_Order_Of_The_Top_Customer()
        {
            string expected = "[{\"C1\":\"1998-04-14T00:00:00\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                     (
                     from cus in context.Customers
                     orderby cus
                         .Orders
                         .SelectMany(o => o.OrderDetails)
                         .Sum(od => od.UnitPrice * od.Quantity)
                         descending
                     select
                         cus

                    )
                    .Select(c => c.Orders.Max(o => o.OrderDate))
                    .FirstOrDefault()

                    , expected
                );

            Assert.IsTrue(result.Check());
        }
    }
}
