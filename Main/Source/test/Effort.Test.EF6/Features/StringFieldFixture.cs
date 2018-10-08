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

using System;
using System.Data.Common;
using Effort.Provider;

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
        public EffortConnection connection;


        [SetUp]
        public void Initialize()
        {
            if (connection == null)
            { 
                connection = Effort.DbConnectionFactory.CreateTransient();
                this.context =
                    new FeatureDbContext(
                        connection,
                        CompiledModels.GetModel<
                            StringFieldEntity>());

                this.context.Database.CreateIfNotExists();
            }
        }

        protected IDbSet<StringFieldEntity> Entities
        {
            get { return this.context.StringFieldEntities; }
        }

        protected void Add(params string[] values)
        {
            this.connection.ClearTables(this.context);
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
            try
            {
                this.connection.IsCaseSensitive = true;
                this.Add("Indie", "Imp", "Huge");

                var res = this.Entities
                    .Where(x => x.Value.CompareTo("I") >= 0)
                    .Count();

                Assert.AreEqual(2, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true; 
            }
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
        public void String_EqualsSensitiveCase()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("john2", "DOE2");

                var res = this.Entities
                    .Where(x => x.Value == "John2").ToList();

                Assert.AreEqual(1, res.Count);
                Assert.AreEqual("john2", res.FirstOrDefault().Value);

                 

                var res2 = this.Entities
                    .Where(x => x.Value == "doe2").ToList();

                Assert.AreEqual(1, res2.Count);
                Assert.AreEqual("DOE2", res2.FirstOrDefault().Value);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_EqualsSensitiveCase2()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("John", null, null);

                var res = this.Entities
                    .Where(x => x.Value == null)
                    .Count();

                Assert.AreEqual(2, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_NotEqualsSensitiveCase()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("John", "Doe", "John");

                var res = this.Entities
                    .Where(x => x.Value != "John")
                    .Count();

                Assert.AreEqual(1, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_NotEqualsSensitiveCaseNull()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("John", "Doe", null);

                var res = this.Entities
                    .Where(x => x.Value != null)
                    .Count();

                Assert.AreEqual(2, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_GreaterThanSensitiveCase()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("Indie", "Imp", "Huge");

                var res = this.Entities
                    .Where(x => x.Value.CompareTo("Imp") > 0)
                    .Count();

                Assert.AreEqual(1, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_GreaterThanOrEqualsSensitiveCase()
        {
            try
            {
                this.connection.IsCaseSensitive = false;
                this.Add("Indie", "Imp", "Huge");

                var res = this.Entities
                    .Where(x => x.Value.CompareTo("I") >= 0)
                    .Count();

                Assert.AreEqual(2, res);
            }
            finally
            {
                this.connection.IsCaseSensitive = true;
            }
        }

        [Test]
        public void String_LessThanSensitiveCase()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("Imp") < 0)
                .Count();

            Assert.AreEqual(1, res);
        }

        [Test]
        public void String_LessThanOrEqualsSensitiveCase()
        {
            this.Add("Indie", "Imp", "Huge");

            var res = this.Entities
                .Where(x => x.Value.CompareTo("Imp") <= 0)
                .Count();

            Assert.AreEqual(2, res);
        } 
    }
}
