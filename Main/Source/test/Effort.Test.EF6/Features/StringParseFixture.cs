// --------------------------------------------------------------------------------------------
// <copyright file="StringParseFixture.cs" company="Effort Team">
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
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using Effort.Test.Data.Features;
    using FluentAssertions;
    using NUnit.Framework;
#if !EFOLD
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.Objects;
#endif

    [TestFixture]
    public class StringParseFixture
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
                    CompiledModels.GetModel<StringFieldEntity>());
        }

        private void Add(string data)
        {
            this.context.StringFieldEntities.Add(
                new StringFieldEntity
                {
                    Value = data
                });

            this.context.SaveChanges();
        }

        private ObjectContext ObjectContext
        {
            get 
            {
                return (this.context as IObjectContextAdapter).ObjectContext;
            }
        }

        [TearDown]
        public void Cleanup()
        {
            this.context.Dispose();
        }

        [Test]
        public void CastStringToInt8()
        {
            this.Add("100");

            var q = this.ObjectContext.CreateQuery<DbDataRecord>(
                "SELECT cast(x.Value as System.Byte) FROM StringFieldEntities as x");

            var value = q.FirstOrDefault().GetValue(0);

            value.Should().Be((byte)100);
        }
    }
}
