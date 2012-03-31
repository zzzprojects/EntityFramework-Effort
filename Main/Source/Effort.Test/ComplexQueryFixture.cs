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
using Effort.Test.Data;
using Effort.Test.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
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
