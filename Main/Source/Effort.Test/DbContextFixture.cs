// ----------------------------------------------------------------------------------
// <copyright file="DbContextFixture.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
    using System.Data.Common;
    using System.Linq;
    using Effort.Test.Data.Staff;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DbContextFixture
    {
        [TestMethod]
        public void DbContext_Create()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            bool created1 = context.Database.CreateIfNotExists();
            bool created2 = context.Database.CreateIfNotExists();

            Assert.IsTrue(created1);
            Assert.IsFalse(created2);
        }

        [TestMethod]
        public void DbContext_Insert()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void DbContext_Query()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            context.SaveChanges();

            Person person = context.People.Single();

            Assert.AreEqual("John", person.FirstName);
            Assert.AreEqual("Doe", person.LastName);
        }

        [TestMethod]
        public void DbContext_Delete()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            StaffDbContext context = new StaffDbContext(connection);

            context.People.Add(new Person { FirstName = "John", LastName = "Doe" });
            context.SaveChanges();

            Person person = context.People.Single();

            context.People.Remove(person);
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
            Assert.AreEqual(0, context.People.Count());
        }
    }
}
