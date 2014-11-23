// --------------------------------------------------------------------------------------------
// <copyright file="BitwiseFunctionsFixture.cs" company="Effort Team">
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

namespace Effort.Test.Features.CanonicalFunctions
{
    using System.Data.Common;
    using System.Linq;
    using Effort.Provider;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class BitwiseFunctionsFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();

            InitializeData(connection);

            this.context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<NumberFieldEntity>());
        }

        private static void InitializeData(DbConnection connection)
        {
            var context = 
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<NumberFieldEntity>());

            context.NumberFieldEntities.Add(
                new NumberFieldEntity {
                    Value8 = 0x0f,
                    Value16 = 0x0f,
                    Value32 = 0x0f,
                    Value64 = 0x0f});

            context.SaveChanges();
            connection.Close();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        [TestMethod]
        public void BitwiseAnd8()
        {
            byte val = (0x0f & 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value8 & (byte)0xaa) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseAnd16()
        {
            short val = (0x0f & 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value16 & (short)0xaa) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseAnd32()
        {
            int val = (0x0f & 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value32 & (int)0xaa) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseAnd64()
        {
            long val = (0x0f & 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value64 & 0xaa) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseOr()
        {
            int val = (0x0f | 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value32 | 0xaa) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseNot()
        {
            int val = (~0x0f);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (~x.Value32) == val);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void BitwiseXor()
        {
            int val = (0x0f ^ 0xaa);

            var q = this.context
                .NumberFieldEntities
                .Where(x => (x.Value32 ^ 0xaa) == val);

            q.ShouldNotBeEmpty();
        }
    }
}
