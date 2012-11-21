// --------------------------------------------------------------------------------------------
// <copyright file="AssociationFixture.cs" company="Effort Team">
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
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class AssociationFixture
    {
        [TestMethod]
        public void SingleFieldAssocitationValidation()
        {
            NorthwindObjectContext context = new LocalNorthwindObjectContext();

            // RegionID = 0 is invalid
            context.Territories.AddObject(
                new Territory() 
                {
                    TerritoryID = "0000",
                    TerritoryDescription = "New territory",
                    RegionID = 0
                });

            bool found = false;

            try
            {
                // The new region addition must fail
                context.SaveChanges();

                Assert.Fail();
            }
            catch (Exception ex)
            {
                // Search for ForeignKeyViolationException exception
                while (ex != null)
                {
                    if (ex is NMemory.Exceptions.ForeignKeyViolationException)
                    {
                        found = true;
                        break;
                    }

                    ex = ex.InnerException;
                }
            }

            Assert.IsTrue(found);
        }

        [TestMethod]
        public void MultiFieldAssociationViolation()
        {
            FeatureObjectContext context = new LocalFeatureObjectContext();

            context.ForeignEntities.AddObject(
                new ForeignEntity
                {
                    FID1 = 1,
                    FID2 = 10,
                    ID = 1
                });

            bool found = false;

            try
            {
                // The new region addition must fail
                context.SaveChanges();

                Assert.Fail();
            }
            catch (Exception ex)
            {
                // Search for ForeignKeyViolationException exception
                while (ex != null)
                {
                    if (ex is NMemory.Exceptions.ForeignKeyViolationException)
                    {
                        found = true;
                        break;
                    }

                    ex = ex.InnerException;
                }
            }

            Assert.IsTrue(found);
        }

        [TestMethod]
        public void SingleFieldAssocitationEmpty()
        {
            NorthwindObjectContext context = new LocalNorthwindObjectContext();

            // All field is empty, null foreign keys
            context.Orders.AddObject(
                new Order() 
                {
                });

            context.SaveChanges();
        }
    }
}
