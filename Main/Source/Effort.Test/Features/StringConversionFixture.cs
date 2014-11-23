// --------------------------------------------------------------------------------------------
// <copyright file="StringConversionFixture.cs" company="Effort Team">
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
// -------------------------------------------------------------------------------------------

namespace Effort.Test.Features
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Effort.Test.Data.Features;
    using FluentAssertions;
    using NUnit.Framework;

    public class StringConversionFixture
    {
        private FeatureDbContext context;

        [SetUp]
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

        [TearDown]
        public void Cleanup()
        {
            this.context.Dispose();
        }

#if EF6
        [Test]
        public void ByteToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value8)
                .FirstOrDefault();

            res.Should().Be("_1");
        }

        [Test]
        public void ShortToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value16)
                .FirstOrDefault();

            res.Should().Be("_1");
        }

        [Test]
        public void IntToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value32)
                .FirstOrDefault();

            res.Should().Be("_1");
        }

        [Test]
        public void LongToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.Value64)
                .FirstOrDefault();

            res.Should().Be("_1");
        }

        [Test]
        public void DoubleToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueD)
                .FirstOrDefault();

            res.Should().Be("_1");
        }

        [Test]
        public void DecimalToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueM)
                .FirstOrDefault();

            res.Should().Be("_1.0");
        }

        [Test]
        public void FloatToString()
        {
            var res = this.context
                .NumberFieldEntities
                .Select(x => "_" + x.ValueF)
                .FirstOrDefault();

            res.Should().Be("_1");
        }
#endif
    }
}
