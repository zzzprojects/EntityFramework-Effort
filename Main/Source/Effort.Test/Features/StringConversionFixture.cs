using Effort.Test.Data.Features;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using SoftwareApproach.TestingExtensions;

namespace Effort.Test.Features
{
    [TestClass]
    public class StringConversionFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();

            this.context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<NumberFieldEntity>());

            InitializeData(connection);
        }

        private static void InitializeData(DbConnection connection)
        {
            var context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<NumberFieldEntity>());

            context.NumberFieldEntities.Add(
                new NumberFieldEntity
                {
                    Value8 = 1,
                    Value16 = 1,
                    Value32 = 1,
                    Value64 = 1,
                    ValueD = 1.0,
                    ValueF = 1.0f,
                    ValueM = 1.0m
                });

            context.SaveChanges();
            connection.Close();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Dispose();
        }

#if EF6
        [TestMethod]
        public void ByteToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value8)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }

        [TestMethod]
        public void ShortToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value16)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }

        [TestMethod]
        public void IntToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value32)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }

        [TestMethod]
        public void LongToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value64)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }

        [TestMethod]
        public void DoubleToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueD)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }

        [TestMethod]
        public void DecimalToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueM)
                .FirstOrDefault();

            res.ShouldEqual("_1.0");
        }

        [TestMethod]
        public void FloatToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueF)
                .FirstOrDefault();

            res.ShouldEqual("_1");
        }
#endif
    }
}
