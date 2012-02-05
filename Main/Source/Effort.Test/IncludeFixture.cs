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

using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Utils;
using Effort.Test.Data;

namespace Effort.Test
{
    [TestClass]
    public class IncludeFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");
        }

        [TestMethod]
        public void OneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.Orders
                        .Include("Customers")
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
                        .Include("Customers")
                        .Include("Employees")
                        .Include("Shippers")
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
                    from ord in context.Customers.Include("Orders")
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
                        .Include("Territories")
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
                    from ord in context.Customers
                        .Include("CustomerDemographics")
                    select ord
                    ).Take(5) //perf

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void CascadedOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.Order_Details
                        .Include("Orders")
                        .Include("Orders.Customers")
                    select ord
                    ).Take(5) // perf

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void CascadedOneToOneInclude()
        {
            bool result = this.runtime.Execute(

                context => (
                    from ord in context.Order_Details
                        .Include("Orders")
                        .Include("Orders.Customers")
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
                        .Include("Orders")
                        .Include("Orders.Order_Details")
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
                        .Include("Orders")
                        .Include("CustomerDemographics")
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
