// --------------------------------------------------------------------------------------------
// <copyright file="JoinFixture.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Features
{
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Effort.Test.Internal.Queries;
    using NUnit.Framework;

    [TestFixture]
    public class JoinFixture
    {
        private IQueryTester<NorthwindObjectContext> tester;

        [SetUp]
        public void Initialize()
        {
            this.tester = new NorthwindQueryTester();
        }

        [Test]
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

        [Test]
        public void TripleCrossJoin()
        {
            string expected = "[{\"LastName\":\"Buchanan\",\"LastName1\":\"Suyama\",\"LastName2\":\"King\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Suyama\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Suyama\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Leverling\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Peacock\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"Peacock\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Leverling\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\",\"LastName2\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Peacock\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Buchanan\",\"LastName2\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Suyama\",\"LastName2\":\"King\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Suyama\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Suyama\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Davolio\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Davolio\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"Peacock\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"King\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Leverling\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\",\"LastName2\":\"King\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Peacock\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Buchanan\",\"LastName2\":\"King\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Suyama\",\"LastName2\":\"King\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Suyama\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Suyama\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Fuller\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Fuller\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"King\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\",\"LastName2\":\"Buchanan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\",\"LastName2\":\"King\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Peacock\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Buchanan\",\"LastName2\":\"King\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Suyama\",\"LastName2\":\"King\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Suyama\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Suyama\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Leverling\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Leverling\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Suyama\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Buchanan\",\"LastName2\":\"King\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Buchanan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Suyama\",\"LastName2\":\"King\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Suyama\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Suyama\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Peacock\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Peacock\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Suyama\",\"LastName1\":\"King\",\"LastName2\":\"Callahan\"},{\"LastName\":\"Suyama\",\"LastName1\":\"King\",\"LastName2\":\"Dodsworth\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Callahan\",\"LastName2\":\"Dodsworth\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from
                        employee1 in context.Employees
                    from
                        employee2 in context.Employees
                    from
                        employee3 in context.Employees
                    where
                      employee1.EmployeeID < employee2.EmployeeID &&
                      employee2.EmployeeID < employee3.EmployeeID 
                    select new
                    {
                        E1 = employee1.LastName,
                        E2 = employee2.LastName,
                        E3 = employee3.LastName
                    },
                expected);

            Assert.IsTrue(result.Check());
        }

        [Test]
        public void OuterJoin()
        {
            string expected = "[{\"LastName\":\"Davolio\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Fuller\",\"LastName1\":null},{\"LastName\":\"Leverling\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Peacock\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Buchanan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Suyama\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"King\",\"LastName1\":\"Buchanan\"},{\"LastName\":\"Callahan\",\"LastName1\":\"Fuller\"},{\"LastName\":\"Dodsworth\",\"LastName1\":\"Buchanan\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from employee in context.Employees

                    join _principal in context.Employees
                    on employee.ReportsTo equals _principal.EmployeeID into __principal
                    from principal in __principal.DefaultIfEmpty()

                    select new
                    {
                        Name = employee.LastName,
                        PrincipalName = principal.LastName
                    }, 
                expected);


            Assert.IsTrue(result.Check());
        }

        [Test]
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

        [Test]
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

        [Test]
        public void OuterApply()
        {
            string expected = "[{\"FirstName\":\"Nancy\",\"FirstName1\":\"Michael\"},{\"FirstName\":\"Andrew\",\"FirstName1\":\"Robert\"},{\"FirstName\":\"Janet\",\"FirstName1\":\"Laura\"},{\"FirstName\":\"Margaret\",\"FirstName1\":\"Anne\"},{\"FirstName\":\"Steven\",\"FirstName1\":null},{\"FirstName\":\"Michael\",\"FirstName1\":null},{\"FirstName\":\"Robert\",\"FirstName1\":null},{\"FirstName\":\"Laura\",\"FirstName1\":null},{\"FirstName\":\"Anne\",\"FirstName1\":null}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from emp1 in context.Employees
                    from emp2 in context.Employees
                        .Where(e => e.EmployeeID == emp1.EmployeeID + 5)
                        .Take(2)
                        .DefaultIfEmpty()
                    select new { e1 = emp1.FirstName, e2 = emp2.FirstName },
                expected);

            Assert.IsTrue(result.Check());
        }

        [Test]
        public void CrossApply()
        {
            string expected = "[{\"FirstName\":\"Nancy\",\"FirstName1\":\"Michael\"},{\"FirstName\":\"Andrew\",\"FirstName1\":\"Robert\"},{\"FirstName\":\"Janet\",\"FirstName1\":\"Laura\"},{\"FirstName\":\"Margaret\",\"FirstName1\":\"Anne\"}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    from emp1 in context.Employees
                    from emp2 in context.Employees
                        .Where(e => e.EmployeeID == emp1.EmployeeID + 5)
                        .Take(2)
                    select new { e1 = emp1.FirstName, e2 = emp2.FirstName },
                expected);

            Assert.IsTrue(result.Check());
        }
    }
}
