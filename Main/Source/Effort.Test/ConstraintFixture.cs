// ----------------------------------------------------------------------------------
// <copyright file="ConstraintFixture.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Test
{
    using System;
    using System.Linq;
    using Effort.Test.Data.Feature;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class ConstraintFixture
    {
        [TestMethod]
        public void String_NotNullableConstraint()
        {
            // This context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            NorthwindObjectContext context  = new LocalNorthwindObjectContext();
            context.Products.AddObject(
                new Product
                {
                    UnitPrice = -250,

                    // Cannot be null
                    ProductName = null 
                });

            try
            {
                context.SaveChanges();
                Assert.Fail("Exception was not thrown");
            }
            catch (Exception ex)
            {
                ex.InnerException.InnerException.ShouldBeOfType(typeof(NMemory.Exceptions.ConstraintException));
            }
        }

        [TestMethod]
        public void NVarCharConstraintThrowsError()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();
            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 100,
                    ID2 = 200,

                    // Max lenght is 20, this has a lenght of 21
                    PrimaryData = "ABCDEABCDEABCDEABCDEA" 
                });
            try
            {
                context.SaveChanges();
                Assert.Fail("Exception was not thrown");
            }
            catch (Exception ex)
            {
                ex.InnerException.InnerException.ShouldBeOfType(typeof(NMemory.Exceptions.ConstraintException));
            }
        }

        [TestMethod]
        public void NVarCharConstraintLetsDataThrough()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();
            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 100,
                    ID2 = 200,

                    // Max lenght is 20, this has a lenght of 19
                    PrimaryData = "ABCDEABCDEABCDEABCD"
                });
            context.SaveChanges();

            context.PrimaryEntities.Select(x => x.PrimaryData).ShouldContain("ABCDEABCDEABCDEABCD");
        }

        [TestMethod]
        [Ignore]
        public void NCharConstraintAppendsData()
        {
            // The NChar with 20 length is saved to NMemory
            // The NChar with 20 length is read back from NMemory to Effort. It can be even be found in QueryCommandAction.ExecuteDataReaderAction (result variable)
            // Still somehow the trailing empty space that was added by the NConstraint is gone...
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.PrimaryEntities.AddObject(
                new PrimaryEntity
                {
                    ID1 = 1,
                    ID2 = 1,
                    PrimaryData = "ABCDEABCDEABCDEABCD"
                });

            context.ForeignEntities.AddObject(
                new ForeignEntity
                {
                    ID = 1,
                    FID1 = 1,
                    FID2 = 1,                
                    ForeignData = "12345 " // Fixed to length of 20
                });

            context.SaveChanges();
            context.ForeignEntities.First(x => x.FID1 == 1).ForeignData.Length.ShouldEqual(20);
        }
    }
}
