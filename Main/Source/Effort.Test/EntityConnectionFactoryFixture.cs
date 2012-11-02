// ----------------------------------------------------------------------------------
// <copyright file="EntityConnectionFactoryFixture.cs" company="Effort Team">
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
    using System.Data.EntityClient;
    using System.Data.Objects;
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EntityConnectionFactoryFixture
    {
        [TestMethod]
        public void EntityConnectionFactory_CreateTransientEntityConnection()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);
        }

        [TestMethod]
        public void EntityConnectionFactory_CreateTransientEntityConnection_InitializesDataSchema()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);

            using (ObjectContext context = new ObjectContext(connection))
            {
                Assert.IsTrue(context.DatabaseExists());
                Assert.AreEqual(0, context.CreateObjectSet<Product>().Count(), "Zero rows in the fake table");
            }
        }

        [TestMethod]
        public void EntityConnectionFactory_CreatePersistentEntityConnection()
        {
            // TODO: Use unique connection string

            EntityConnection connection = EntityConnectionFactory.CreatePersistent(NorthwindObjectContext.DefaultConnectionString);
        }
    }
}
