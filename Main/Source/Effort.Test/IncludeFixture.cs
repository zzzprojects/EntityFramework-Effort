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
using Effort.Test.Data.Northwind;
using System.Data.Entity;

namespace Effort.Test
{
    [TestClass]
    public class IncludeFixture
    {
        private QueryTestRuntime<NorthwindObjectContext> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void OneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.Orders
                        .Include(o => o.Customer)
                    select ord
                    ).Take(5) // perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MultipleOneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context =>(
                    from ord in context.Orders
                        .Include(o => o.Customer)
                        .Include(o => o.Employee)
                        .Include(o => o.Shipper)
                    select ord
                    ).Take(5) //perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OneToManyInclude()
        {
            bool result = this.runtime.Execute(

                context =>(
                    from ord in context.Customers
                        .Include(o => o.Orders)
                    select ord
                    )
                    .Take(5) //perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ManyToManyInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from emp in context.Employees
                        .Include(e => e.Territories)
                    select emp
                    ).Take(5) //perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ManyToManyInclude2()
        {
            bool result = this.runtime.Execute(

                context => (
                    from cus in context.Customers
                        .Include(c => c.CustomerDemographics)
                    select cus
                    ).Take(5) //perf

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void CascadedOneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.OrderDetails
                        .Include(o => o.Order.Customer)
                    select ord
                    ).Take(5) // perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MixedCascadedOneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.OrderDetails
                        .Include(o => o.Order)
                        .Include(o => o.Order.Customer)
                    select ord
                    ).Take(5) // perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CascadedOneToManyInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from cus in context.Customers
                        .Include(c => c.Orders.Select(o => o.OrderDetails))
                    select cus
                    ).Take(5) // perf

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void CombinedInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from cus in context.Customers
                        .Include(c => c.Orders)
                        .Include(c => c.CustomerDemographics)
                    select cus
                    ).Take(5) //perf
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void NestedInclude()
        {
            bool result = this.runtime.Execute(

                context =>

                    from
                        cus in context.Customers
                    select
                        new
                        {
                            Customer = cus,
                            Orders = cus.Orders,
                            Demographics = cus.CustomerDemographics
                        }
            );

            Assert.IsTrue(result);
        }



    }
}
