// ----------------------------------------------------------------------------------
// <copyright file="JoinFixture.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    public class JoinFixture
    {
        private IQueryTester<NorthwindObjectContext> tester;

        [TestInitialize]
        public void Initialize()
        {
            this.tester = new NorthwindQueryTester();
        }

        [TestMethod]
        public void CrossJoin()
        {
            string expected = "[{\"LastName\":\"Davolio\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Callahan\"},{\"LastName\":\"King\",\"LastName1\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"King\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Callahan\",\"LastName1\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Davolio\",\"LastName1\":\"King\"},{\"LastName\":\"Fuller\",\"LastName1\":\"King\"},{\"LastName\":\"Leverling\",\"LastName1\":\"King\"},{\"LastName\":\"Peacock\",\"LastName1\":\"King\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"King\"},{\"LastName\":\"Suyama\",\"LastName1\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Suyama\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Suyama\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Suyama\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Suyama\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Suyama\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from 
                        employee1 in context.Employees
                    from 
                        employee2 in context.Employees
                    where
                      employee1.EmployeeID < employee2.EmployeeID
                    select new
                    {
                        E1 = employee1.LastName,
                        E2 = employee2.LastName
                    }, 
                expected);

            Assert.IsTrue(result.Check());
        }

        [TestMethod]
        public void OuterJoin()
        {
            string expected = "[{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"King\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Callahan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Dodsworth\",\"LastName1\":\"Buchanan\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from employee in context.Employees

                    join _principal in context.Employees
                    on employee.ReportsTo equals _principal.EmployeeID into __principal
                    from principal in __principal

                    select new
                    {
                        Name = employee.LastName,
                        PrincipalName = principal.LastName
                    }, 
                expected);

            Assert.IsTrue(result.Check());
        }

        [TestMethod]
        public void OuterJoin2()
        {
            string expected = "[{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Fuller\",\"LastName1\":null},{\"LastName\":\"Leverling\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"King\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Callahan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Dodsworth\",\"LastName1\":\"Buchanan\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from emp in context.Employees
                    select new
                    {
                        Name = emp.LastName,
                        PrincipalName = emp.Principal.LastName
                    }, 
                expected);

            Assert.IsTrue(result.Check());
        }

        [TestMethod]
        public void InnerJoin()
        {
            string expected = "[{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"King\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Callahan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Dodsworth\",\"LastName1\":\"Buchanan\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from employee in context.Employees
                    join principal in context.Employees
                    on employee.ReportsTo equals principal.EmployeeID
                    select new
                    {
                        Name = employee.LastName,
                        PrincipalName = principal.LastName
                    }, 
                expected);

            Assert.IsTrue(result.Check());
        }
    }
}
