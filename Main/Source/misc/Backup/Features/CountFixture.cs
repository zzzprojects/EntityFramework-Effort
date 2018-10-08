// --------------------------------------------------------------------------------------------
// <copyright file="DateFieldFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2015 Effort Team
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
    using System.Data.Entity;
    using Effort.Test.Data.Features;
    using NUnit.Framework;
    using System.Linq;
    
    public class CountFixture
    {
        private FeatureDbContext context;

        [SetUp]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<
                        StringFieldEntity>());
        }

        protected IDbSet<StringFieldEntity> Entities
        {
            get { return this.context.StringFieldEntities; }
        }

        protected void Add(params string[] values)
        {
            foreach (var value in values)
            {
                this.Entities.Add(new StringFieldEntity { Value = value });
            }

            this.context.SaveChanges();
        }

        [Test]
        public void Count()
        {
            this.Add("John", "Doe");

            var res = this.Entities
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void LongCount()
        {
            this.Add("John", "Doe");

            var res = this.Entities
                .LongCount();

            Assert.AreEqual(2, res);
        }
    }
}
