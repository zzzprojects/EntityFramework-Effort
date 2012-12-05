using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Staff;
using System.Data.Common;

namespace Effort.Test.Features
{
    [TestClass]
    public class GuidKeyFixture
    {
        private StaffDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            DbConnection connection = Effort.DbConnectionFactory.CreateTransient();
            this.context = new StaffDbContext(connection);
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        [TestMethod]
        public void GuidKey_InsertGeneratesNewGuid()
        {
            GuidKeyEntity entity = new GuidKeyEntity();

            this.context.GuidKeyEntities.Add(entity);
            this.context.SaveChanges();

            Assert.AreNotEqual(Guid.Empty, entity.Id);
        }

        [TestMethod]
        public void GuidKey_UpdateKeepsGuid()
        {
            GuidKeyEntity entity = new GuidKeyEntity();

            this.context.GuidKeyEntities.Add(entity);
            this.context.SaveChanges();

            Guid guid = entity.Id;
            entity.Data = "Changed data";
            this.context.SaveChanges();

            Assert.AreEqual(guid, entity.Id);
        }
    }
}
