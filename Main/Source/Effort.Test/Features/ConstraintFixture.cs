// --------------------------------------------------------------------------------------------
// <copyright file="ConstraintFixture.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using System;
    using System.Linq;
    using Effort.Test.Data.Feature;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;
    using Effort.Test.Internal;

    [TestClass]
    public class ConstraintFixture
    {
        private FeatureObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            // Do not use data loader
            this.context = new LocalFeatureObjectContext(false);
        }

        [TestMethod]
        public void String_NotNullableConstraint()
        {
            this.context.ConstraintSupports.AddObject(
                new ConstraintSupport
                {
                    NotNull = null 
                });

            try
            {
                context.SaveChanges();
                Assert.Fail("Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(
                    ExceptionHelper.ContainsException(
                        ex, 
                        "NMemory.Exceptions.ConstraintException"));
            }
        }

        [TestMethod]
        public void NVarCharConstraintThrowsError()
        {
            // Max lenght is 10, this has a lenght of 11
            this.context.ConstraintSupports.AddObject(
                new ConstraintSupport
                {
                    NotNull = "",
                    MaxLength = "12345678901"
                });
            try
            {
                context.SaveChanges();
                Assert.Fail("Exception was not thrown");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(
                    ExceptionHelper.ContainsException(
                        ex,
                        "NMemory.Exceptions.ConstraintException"));
            }
        }

        [TestMethod]
        public void NVarCharConstraintLetsDataThrough()
        {
            // Max lenght is 10, this has a lenght of 9
            this.context.ConstraintSupports.AddObject(
                new ConstraintSupport
                {
                    NotNull = "",
                    MaxLength = "123456789"
                });
            context.SaveChanges();

            context.ConstraintSupports.Select(x => x.MaxLength).ShouldContain("123456789");
        }

        [TestMethod]
        public void NCharConstraintAppendsData()
        {
            // Max lenght is 10, this has a lenght of 9
            this.context.ConstraintSupports.AddObject(
                new ConstraintSupport
                {
                    NotNull = "",
                    FixedLength = "123456789"
                });
            context.SaveChanges();

            context.ConstraintSupports.First();
            context.ConstraintSupports.Select(x => x.FixedLength).ShouldContain("123456789 ");
        }
    }
}
