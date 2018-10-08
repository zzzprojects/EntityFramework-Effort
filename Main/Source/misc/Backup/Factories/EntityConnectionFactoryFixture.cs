// --------------------------------------------------------------------------------------------
// <copyright file="EntityConnectionFactoryFixture.cs" company="Effort Team">
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

namespace Effort.Test.Factories
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using NUnit.Framework;
#if !EFOLD
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.EntityClient;
    using System.Data.Objects;
#endif
    

    [TestFixture]
    public class EntityConnectionFactoryFixture
    {
        [Test]
        public void EntityConnectionFactory_CreateTransientEntityConnection()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);
        }

        [Test]
        public void EntityConnectionFactory_CreateTransientEntityConnection_InitializesDataSchema()
        {
            EntityConnection connection = EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);

            using (ObjectContext context = new ObjectContext(connection))
            {
                Assert.IsTrue(context.DatabaseExists());
                Assert.AreEqual(0, context.CreateObjectSet<Product>().Count(), "Zero rows in the fake table");
            }
        }

        [Test]
        public void EntityConnectionFactory_CreatePersistentEntityConnection()
        {
            var connString = this.GetDefaultConnectionString();

            var csBuilder = new EntityConnectionStringBuilder(connString);
            csBuilder.ProviderConnectionString = Guid.NewGuid().ToString();

            EntityConnection connection = EntityConnectionFactory.CreatePersistent(csBuilder.ConnectionString);
        }

        [Test]
        public void EntityConnectionFactory_ImplicitMetadata()
        {
            var connString = this.GetDefaultConnectionString();

            var csBuilder = new EntityConnectionStringBuilder(connString);
            csBuilder.Metadata = "res://*/";
            csBuilder.ProviderConnectionString = Guid.NewGuid().ToString();

            EntityConnectionFactory.CreateTransient(csBuilder.ConnectionString);
        }

        private string GetDefaultConnectionString()
        {
            var name = NorthwindObjectContext.DefaultConnectionStringName;
            return ConfigurationManager.ConnectionStrings[name].ConnectionString;
        }
    }
}
