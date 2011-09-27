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
    public class JoinFixture
    {
        private QueryTestRuntime<NorthwindEntities> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindEntities>("name=NorthwindEntities");

        }

        [TestMethod]
        public void CrossJoin()
        {
            bool result = this.runtime.Execute(

              context =>
                  from emp1 in context.Employees

                  from emp2 in context.Employees

                  where 
                    emp1.EmployeeID < emp2.EmployeeID 

                  select new
                  {
                      emp1 = emp1,
                      emp2 = emp2
                  }
          );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void OuterJoin()
        {
            bool result = this.runtime.Execute(

              context =>
                  from emp in context.Employees
                  
                  join rep_ in context.Employees
                  on emp.ReportsTo equals rep_.EmployeeID into rep__
                  from rep in rep__

                  select new 
                  {
                      name = emp.LastName,
                      reportsTo = rep.LastName
                  }
          );

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void OuterJoin2()
        {
            bool result = this.runtime.Execute(

              context =>
                  from emp in context.Employees
                  select new
                  {
                      name = emp.LastName,
                      reportsTo = emp.Employees2.LastName
                  }
          );

            Assert.IsTrue(result);
        }


        [TestMethod]
        public void InnerJoin()
        {
            bool result = this.runtime.Execute(

              context =>
                  from emp in context.Employees
                  join rep in context.Employees

                  on emp.ReportsTo equals rep.EmployeeID

                  select new
                  {
                      name = emp.LastName,
                      reportsTo = emp.LastName
                  }
          );

            Assert.IsTrue(result);
        }
    }
}
