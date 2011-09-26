using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MMDB.EntityFrameworkProvider.UnitTests.Data;
using MMDB.EntityFrameworkProvider.UnitTests.Utils;

namespace MMDB.EntityFrameworkProvider.UnitTests
{
    [TestClass]
    public class BasicFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");
        
        }
        

        [TestMethod]
        public void FullTableScan()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    select emp

            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void FindById()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID == 1
                    select emp
                    
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OrderBy()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    orderby emp.LastName
                    select emp


                ,true //Strict order
                    
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void Projection()
        {
            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    select new { firstName = emp.FirstName, lastName = emp.LastName }

                );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void QueryParameter()
        {
            int id = 1;

            bool result = this.runtime.Execute(

                context =>
                    from emp in context.Employees
                    where emp.EmployeeID == id
                    select emp

            );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void QueryParameterChange()
        {

            for (int id = 1; id < 3; id++)
            {
                bool result = this.runtime.Execute(

                      context =>
                          from emp in context.Employees
                          where emp.EmployeeID == id
                          select emp

                  );

                Assert.IsTrue(result);
            }


        }



        [TestMethod]
        public void UnionAll()
        {

            bool result = this.runtime.Execute(

                context =>
                    context.Employees.Concat(context.Employees)
            );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void UnionAll2()
        {
            bool result = this.runtime.Execute(

                context =>
                    context.Employees.Where(e => e.EmployeeID < 5)
                    
                    .Concat(
                    
                    context.Employees.Where(e => e.EmployeeID > 10))
            );

            Assert.IsTrue(result);
        }



    }
}
