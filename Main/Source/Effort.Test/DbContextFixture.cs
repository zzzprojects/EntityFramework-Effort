// --------------------------------------------------------------------------------------------
// <copyright file="DbContextFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Test
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class DbContextFixture
    {
        [TestMethod]
        public void DbContext_Create()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            FeatureDbContext context = new FeatureDbContext(connection);

            bool created1 = context.Database.CreateIfNotExists();
            bool created2 = context.Database.CreateIfNotExists();

            Assert.IsTrue(created1);
            Assert.IsFalse(created2);
        }

        [TestMethod]
        public void DbContext_Insert()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            FeatureDbContext context = new FeatureDbContext(connection);

            context.StringFieldEntities.Add(new StringFieldEntity { Value = "Foo" });
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
        }

        [TestMethod]
        public void DbContext_Query()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            FeatureDbContext context = new FeatureDbContext(connection);

            context.StringFieldEntities.Add(new StringFieldEntity { Value = "Foo" });
            context.SaveChanges();

            StringFieldEntity person = context.StringFieldEntities.Single();

            Assert.AreEqual("Foo", person.Value);
        }

        [TestMethod]
        public void DbContext_Remove()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();
            FeatureDbContext context = new FeatureDbContext(connection);

            context.StringFieldEntities.Add(new StringFieldEntity { Value = "Foo" });
            context.SaveChanges();

            StringFieldEntity person = context.StringFieldEntities.Single();

            context.StringFieldEntities.Remove(person);
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
            Assert.AreEqual(0, context.StringFieldEntities.Count());
        }

        /// <summary>
        ///     StringFieldEntity is directed to the Foo table
        /// </summary>
        [TestMethod]
        public void DbContext_TableName()
        {
            DbConnection connection = 
                DbConnectionFactory.CreateTransient(
                    new LocalFeatureDataLoader());

            FeatureDbContext context = 
                new FeatureDbContext(connection, CompiledModels.TableNameModel);

            var result = context.StringFieldEntities.ToList();

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Foo", result[0].Value);
        }

        [TestMethod]
        public void DbContext_DatabaseExists()
        {
            // Microsoft change the way database existence check works
            //
            // Exist call with an open connection always returns true
            // In an earlier version of Effort, the Exists call opened the connection, 
            // so later initialization did not begin 

            DbConnection connection = DbConnectionFactory.CreateTransient();
            FeatureDbContext context = new FeatureDbContext(connection);
            
            var exists = context.Database.Exists();

            Assert.IsFalse(exists);

            context.StringFieldEntities.Add(new StringFieldEntity { Value = "Foo" });
            int count = context.SaveChanges();

            Assert.AreEqual(1, count);
        }
    }
}
