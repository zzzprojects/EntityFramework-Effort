// --------------------------------------------------------------------------------------------
// <copyright file="StringFieldFixture.cs" company="Effort Team">
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
    using System.Data.Entity;
    using System.Linq;
    using Effort.Test.Data.Features;
    using NUnit.Framework;

    [TestFixture]
    public class StringFieldFixture
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
        public void String_Equals()
        {
            this.Add("John", "Doe");

            var res = this.Entities
                .Where(x => x.Value == "John")
                .Count();

            Assert.AreEqual(1, res);
        }

        [Test]
        public void String_Equals2()
        {
            this.Add("John", null, null);

            var res = this.Entities
                .Where(x => x.Value == null)
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void String_NotEquals()
        {
            this.Add("John", "Doe", "John");

            var res = this.Entities
                .Where(x => x.Value != "John")
                .Count();

            Assert.AreEqual(1, res);
        }

        [Test]
        public void String_NotEqualsNull()
        {
            this.Add("John", "Doe", null);

            var res = this.Entities
                .Where(x => x.Value != null)
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void String_GreaterThan()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("Imp") > 0)
                .Count();

            Assert.AreEqual(1, res);
        }

        [Test]
        public void String_GreaterThanOrEquals()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("I") >= 0)
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void String_LessThan()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("Imp") < 0)
                .Count();

            Assert.AreEqual(1, res);
        }

        [Test]
        public void String_LessThanOrEquals()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("Imp") <= 0)
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void String_CompareNull()
        {
            this.Add("Indie", "Imp", null);

            var res = this.Entities
                .Where(x => x.Value.CompareTo(null) == -1)
                .Count();

            Assert.AreEqual(2, res);
        }

        [Test]
        public void String_CompareNull2()
        {
            this.Add("Indie", "Imp", null);

            var res = this.Entities
                .Where(x => ((string)null).CompareTo(x.Value) == -1)
                .Count();

            Assert.AreEqual(2, res);
        }
    }
}
