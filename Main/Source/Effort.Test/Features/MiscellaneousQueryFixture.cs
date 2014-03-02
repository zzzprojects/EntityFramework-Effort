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
    using System.Collections.Generic;
    using System.Linq;
    using Effort.Test.Data.Northwind;
    using Effort.Test.Internal.Queries;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class MiscellaneousQueryFixture
    {
        private IQueryTester<NorthwindObjectContext> tester;

        [TestInitialize]
        public void Initialize()
        {
            this.tester = new NorthwindQueryTester();
        }

        /// <summary>
        ///     Query source: http://effort.codeplex.com/discussions/528812
        /// </summary>
        [TestMethod]
        public void Discussion528812()
        {
            var expected = "[{\"C2\":null},{\"C2\":null},{\"C2\":null},{\"C2\":null},{\"C2\":null},{\"C2\":null},{\"C2\":null},{\"C2\":null}]";

            ICorrectness result = this.tester.TestQuery(
                context =>
                    context.Categories.Select(x =>
                        new
                        {
                            Description = context.Products.FirstOrDefault(u => false) != null ?
                                context.Products.FirstOrDefault(u => false).ProductName :
                                null
                        }),
                expected);

            Assert.IsTrue(result.Check());
        }
    }
}
