// --------------------------------------------------------------------------------------------
// <copyright file="MiscellaneousQueryFixture.cs" company="Effort Team">
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
    using Effort.Test.Data.Northwind;
    using Effort.Test.Internal.Queries;
    using NUnit.Framework;
#if EFOLD
    using System.Data;
#endif


    [TestFixture]
    public class MiscellaneousQueryFixture
    {
        /// <summary>
        ///     Source: http://effort.codeplex.com/discussions/528812
        /// </summary>
        [Test]
        public void Discussion528812()
        {
            var expected = "[{\"C1\":1},{\"C1\":3},{\"C1\":16},{\"C1\":11},{\"C1\":22},{\"C1\":9},{\"C1\":7},{\"C1\":10}]";

            ICorrectness result = new NorthwindQueryTester().TestQuery(
                context => context.Categories
                    .Where(x => x.Description != null)
                    .Select(x =>
                        new
                        {
                            Description = context.Products.Any(u => u.CategoryID == x.CategoryID) != null ?
                                (int?)context.Products.FirstOrDefault(u => u.CategoryID == x.CategoryID).ProductID :
                                null
                        }),
               expected);

            Assert.IsTrue(result.Check());
        }

        /// <summary>
        ///     Source: https://github.com/tamasflamich/effort/issues/56
        /// </summary>
        [Test]
        public void GithubIssue56()
        {
            var connection = Effort.EntityConnectionFactory.CreateTransient(NorthwindObjectContext.DefaultConnectionString);
            var dbContext = new NorthwindDbContext(connection);

            var cat1 = new Category { CategoryName = "Foo" };
            dbContext.Configuration.AutoDetectChangesEnabled = true;
            dbContext.Categories.Add(cat1);
            dbContext.SaveChanges();

            var dbContext2 = new NorthwindDbContext(connection);
            var cat2 = dbContext2.Categories.FirstOrDefault();

            cat2.CategoryName = "Baar";
            dbContext2.SaveChanges();

            // This test would fail without the following line
            dbContext.Entry(cat1).State = EntityState.Detached;

            Assert.AreEqual(
                dbContext.Categories.FirstOrDefault().CategoryName,
                dbContext2.Categories.FirstOrDefault().CategoryName);
        }
    }
}
