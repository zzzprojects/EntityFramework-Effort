using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;
using SoftwareApproach.TestingExtensions;
using Effort.Test.Data.Feature;

namespace Effort.Test
{
    [TestClass]
    public class ConstraintFixture
    {

        [TestMethod]
        public void String_NotNullableConstraint()
        {
            //this context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            NorthwindObjectContext context  = new LocalNorthwindObjectContext();
            context.Products.AddObject(new Product
            {
                UnitPrice = -250,
                ProductName = null //cannot be null
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
            context.PrimaryEntities.AddObject(new PrimaryEntity
            {
                ID1 = 100,
                ID2 = 200,
                PrimaryData = "ABCDEABCDEABCDEABCDEA" //Max lenght is 20, this has a lenght of 21
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
            context.PrimaryEntities.AddObject(new PrimaryEntity
            {
                ID1 = 100,
                ID2 = 200,
                PrimaryData = "ABCDEABCDEABCDEABCD" //Max lenght is 20, this has a lenght of 19
            });
            context.SaveChanges();
            context.PrimaryEntities.Select(x => x.PrimaryData).ShouldContain("ABCDEABCDEABCDEABCD");
        }
    }
}
