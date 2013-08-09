// --------------------------------------------------------------------------------------------
// <copyright file="BitwiseFunctionsFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
                    CompiledModels.GetModel<IntFieldEntity>());
        }

        private static void InitializeData(DbConnection connection)
        {
            var context = 
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<IntFieldEntity>());

            context.IntFieldEntities.Add(
                new IntFieldEntity { Value = 0x0f });

            context.SaveChanges();
            connection.Close();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        [TestMethod]
        public void BitwiseAnd()
        {
            var q = this.context
                .IntFieldEntities
                .Where(x => (x.Value & 1) != 0);

            q.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void BitwiseOr()
        {
            var q = this.context
                .IntFieldEntities
                .Where(x => (x.Value ^ 0xf0) != 0);

            q.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void BitwiseNot()
        {
            var q = this.context
                .IntFieldEntities
                .Where(x => (~ x.Value) != 0);

            q.Any().ShouldBeTrue();
        }

        [TestMethod]
        public void BitwiseXor()
        {
            var q = this.context
                .IntFieldEntities
                .Where(x => (x.Value ^ 0xf0) != 0);

            q.Any().ShouldBeTrue();
        }
    }
}
