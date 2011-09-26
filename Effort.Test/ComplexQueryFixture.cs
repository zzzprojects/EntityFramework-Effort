using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMDB.EntityFrameworkProvider.UnitTests.Utils;
using MMDB.EntityFrameworkProvider.UnitTests.Data;

namespace MMDB.EntityFrameworkProvider.UnitTests
{
    [TestClass]
    public class ComplexQueryFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

        }

        [TestMethod]
        public void Products_That_Has_Higher_Unit_Price_Than_Average()
        {
            //return;
            bool result = this.runtime.Execute(

                context =>

                    from
                        p in context.Products
                    where
                        p.UnitPrice > context.Products.Average( x => x.UnitPrice )
                    select
                        p

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void Name_Of_The_Customer_Who_Has_The_Most_Orders_Out_Of_The_Top_Ten_Customers()
        {
            //return;
            bool result = this.runtime.Execute(

                context =>

                    (
                     from cus in context.Customers
                     orderby cus
                         .Orders
                         .SelectMany(o => o.Order_Details)
                         .Sum(od => od.UnitPrice * od.Quantity)
                         descending
                     select
                         cus

                    )
                    .Take(10)
                    .OrderByDescending(c => c.Orders.Count)
                    .Select(c => c.ContactName)
                    .FirstOrDefault()
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Date_Of_The_Last_Order_Of_The_Top_Customer()
        {
            //return;
            bool result = this.runtime.Execute(

                context =>

                    (
                     from cus in context.Customers
                     orderby cus
                         .Orders
                         .SelectMany(o => o.Order_Details)
                         .Sum(od => od.UnitPrice * od.Quantity)
                         descending
                     select
                         cus

                    )
                    .Select(c => c.Orders.Max(o => o.OrderDate))
                    .FirstOrDefault()
            );

            Assert.IsTrue(result);
        }


    }
}
