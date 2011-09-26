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
