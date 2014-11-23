// --------------------------------------------------------------------------------------------
// <copyright file="StringFunctionsFixture.cs" company="Effort Team">
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
#if EFOLD
    using System.Data.Objects;
#endif
    using System.Data.Entity;
    using System.Linq;
    using Effort.Test.Data.Features;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class StringFunctionsFixture
    {
        private FeatureDbContext context;

        [SetUp]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<StringFieldEntity>());
        }

        public IDbSet<StringFieldEntity> Entities
        {
            get
            {
                return this.context.StringFieldEntities;
            }
        }

         [Test]
        public void StringConcat()
        {
            this.Entities.Add(new StringFieldEntity { Value = "data" });
            this.context.SaveChanges();

            var q = this.Entities
                .Select(x => 
                    string.Concat(x.Value, "1"));

            var res = q.ToList();
            res.Any(x => x == "data1").Should().BeTrue();
        }

         [Test]
         public void StringConcatNull()
         {
             this.Entities.Add(new StringFieldEntity { Value = null });
             this.context.SaveChanges();

             var q = this.Entities
                  .Select(x => 
                      string.Concat(x.Value, "1"));

             var res = q.ToList();

#if EF61
             // EF 6.1 fixes this object-relational impedance
             res.Any(x => x == "1").Should().BeTrue();
#else
             res.Any(x => x == null).Should().BeTrue();
#endif
         }

        [Test]
        public void StringIsNullOrEmpty()
        {
            this.Entities.Add(new StringFieldEntity { Value = "" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    string.IsNullOrEmpty(x.Value));

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringIsNullOrEmptyNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    string.IsNullOrEmpty(x.Value));

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringContains()
        {
            this.Entities.Add(new StringFieldEntity { Value = "apple orange banana" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                   x.Value.Contains("banana"));

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringEndsWith()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.EndsWith("ge"));

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringStartsWith()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.StartsWith("or"));

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringLength()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Length == 6);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringLengthNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Length == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringIndexOf()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.IndexOf("ra") == 1);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringIndexOf2()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.IndexOf("app") == -1);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringIndexOfNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.IndexOf(null) == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringIndexOfNull2()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.IndexOf("a") == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringInsert()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Insert(1, "123") == "o123range");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringInsertNull1()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Insert(1, "123") == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringInsertNull2()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Insert(1, null) == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringRemove1()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Remove(3) == "ora");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringRemove2()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Remove(3, 2) == "orae");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReplace()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Replace("nge", "cle") == "oracle");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReplaceNull1()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Replace("nge", "cle") == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReplaceNull2()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Replace(null, "cle") == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReplaceNull3()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Replace("nge", null) == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringSubstring1()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Substring(3) == "nge");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringSubstring2()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Substring(3, 2) == "ng");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringSubstringNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Substring(3, 2) == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringToLower()
        {
            this.Entities.Add(new StringFieldEntity { Value = "OraNge" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.ToLower() == "orange");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringToLowerNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.ToLower() == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringToUpper()
        {
            this.Entities.Add(new StringFieldEntity { Value = "OraNge" });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.ToUpper() == "ORANGE");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringToUpperNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.ToUpper() == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void Trim()
        {
            this.Entities.Add(new StringFieldEntity { Value = " orange  " });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Trim() == "orange");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void TrimNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.Trim() == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void TrimEnd()
        {
            this.Entities.Add(new StringFieldEntity { Value = " orange  " });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.TrimEnd() == " orange");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void TrimEndNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.TrimEnd() == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void TrimStart()
        {
            this.Entities.Add(new StringFieldEntity { Value = " orange  " });
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.TrimStart() == "orange  ");

            q.Should().NotBeEmpty();
        }

        [Test]
        public void TrimStartNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null});
            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Value.TrimStart() == null);

            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReverse()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Reverse(x.Value) == "egnaro");
#else
            var q = this.context
                .StringFieldEntities
                .Where(x =>
                    EntityFunctions.Reverse(x.Value) == "egnaro");
#endif
            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringReverseNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Reverse(x.Value) == null);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.Reverse(x.Value) == null);
#endif
            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringLeft()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Left(x.Value, 2) == "or");
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.Left(x.Value, 2) == "or");
#endif
            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringLeftNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Left(x.Value, 2) == null);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.Left(x.Value, 2) == null);
#endif
            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringRight()
        {
            this.Entities.Add(new StringFieldEntity { Value = "orange" });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Right(x.Value, 2) == "ge");
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.Right(x.Value, 2) == "ge");
#endif
            q.Should().NotBeEmpty();
        }

        [Test]
        public void StringRightNull()
        {
            this.Entities.Add(new StringFieldEntity { Value = null });
            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.Right(x.Value, 2) == null);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.Right(x.Value, 2) == null);
#endif
            q.Should().NotBeEmpty();
        }
    }
}
