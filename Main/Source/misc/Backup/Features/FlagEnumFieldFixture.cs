// --------------------------------------------------------------------------------------------
// <copyright file="FlagEnumFieldFixture.cs" company="Effort Team">
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
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;
    using Effort.Test.Data.Features;
    using NUnit.Framework;
    using FluentAssertions;
    
    [TestFixture]
    public class FlagEnumFieldFixture
    {
        private FeatureDbContext context;

        [SetUp]
        public void Initialize()
        {
            DbConnection connection =
                Effort.DbConnectionFactory.CreateTransient();

            this.context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<FlagEnumFieldEntity>());
        }

        [TearDown]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        private IDbSet<FlagEnumFieldEntity> Entities
        {
            get { return this.context.Set<FlagEnumFieldEntity>(); }
        }

#if !EFOLD || NET45
        [Test]
        public void FlagEnumFieldFixture_And()
        {
            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    Value = FlagEnumFieldType.Value1 | FlagEnumFieldType.Value2
                });

            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    Value = FlagEnumFieldType.Value2
                });

            this.context.SaveChanges();

            var res = this.Entities
                .Where(x => (FlagEnumFieldType.Value1 & x.Value) == FlagEnumFieldType.Value1)
                .Count();

            res.Should().Be(1);
        }

        [Test]
        public void FlagEnumFieldFixture_Or()
        {
            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    Value = FlagEnumFieldType.Value1
                });

            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    Value = FlagEnumFieldType.Value2
                });

            this.context.SaveChanges();

            var res = this.Entities
                .Where(x => (FlagEnumFieldType.Value2 | x.Value) == (FlagEnumFieldType)3)
                .Count();

            res.Should().Be(1);
        }

        [Test]
        public void FlagEnumFieldFixture_NullableAnd()
        {
            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    NullableValue = FlagEnumFieldType.Value1 | FlagEnumFieldType.Value2
                });

            this.Entities.Add(
                new FlagEnumFieldEntity
                {
                    NullableValue = FlagEnumFieldType.Value2
                });

            this.context.SaveChanges();

            var res = this.Entities
                .Where(x => (FlagEnumFieldType.Value1 & x.NullableValue) == FlagEnumFieldType.Value1)
                .Count();

            res.Should().Be(1);
        }
#endif
    }
}
