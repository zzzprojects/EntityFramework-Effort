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

namespace Effort.Test
{
    [TestClass]
    public class JoinFixture
    {
        private QueryTestRuntime<NorthwindObjectContext> runtime;

        [TestInitialize]
        public void Initialize()
        {
            this.runtime = new QueryTestRuntime<NorthwindObjectContext>(NorthwindObjectContext.DefaultConnectionString);

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
                      reportsTo = emp.Principal.LastName
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
