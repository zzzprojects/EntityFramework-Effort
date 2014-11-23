// --------------------------------------------------------------------------------------------
// <copyright file="MiscellaneousQueryFixture.cs" company="Effort Team">
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

namespace Effort.Test.Features
{
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Effort.Test.Internal.Queries;
    using NUnit.Framework;

    [TestFixture]
    public class MiscellaneousQueryFixture
    {
        private IQueryTester<NorthwindObjectContext> tester;

        [SetUp]
        public void Initialize()
        {
            this.tester = new NorthwindQueryTester();
        }

        /// <summary>
        ///     Query source: http://effort.codeplex.com/discussions/528812
        /// </summary>
        [Test]
        public void Discussion528812()
        {
            var expected = "[{\"C1\":1},{\"C1\":3},{\"C1\":16},{\"C1\":11},{\"C1\":22},{\"C1\":9},{\"C1\":7},{\"C1\":10}]";

            ICorrectness result = this.tester.TestQuery(
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
    }
}
