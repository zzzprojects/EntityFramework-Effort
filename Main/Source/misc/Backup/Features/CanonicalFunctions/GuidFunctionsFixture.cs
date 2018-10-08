// --------------------------------------------------------------------------------------------
// <copyright file="GuidFunctionsFixture.cs" company="Effort Team">
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

namespace Effort.Test.Features.CanonicalFunctions
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using Effort.Test.Data.Features;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class GuidFunctionsFixture
    {
        private FeatureDbContext context;

        [SetUp]
        public void Initialize()
        {
            DbConnection connection = DbConnectionFactory.CreateTransient();

            this.context =
                new FeatureDbContext(
                    connection,
                    CompiledModels.GetModel<StringFieldEntity>());
        }

        [Test]
        public void NewGuidFunction()
        {
            this.context.StringFieldEntities.Add(
                new StringFieldEntity() { Value = "Value" });
            this.context.SaveChanges();

            var q = this.context
                .StringFieldEntities
                .Select(x =>
                    new
                    {
                        Value = x.Value,
                        Guid = Guid.NewGuid()
                    });

            var entity = q.First();

            entity.Value.Should().Be("Value");
            entity.Guid.Should().NotBe(Guid.Empty);
        }
    }
}
