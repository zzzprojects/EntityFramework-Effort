using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Northwind;
using SoftwareApproach.TestingExtensions;

namespace Effort.Test
{
    [TestClass]
    public class ConstraintFixture
    {
        private NorthwindObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            //this context is initialized from the csv files in Effort.Test.Data/Northwind/Content
            this.context = new LocalNorthwindObjectContext();
        }

        [TestMethod]
        public void String_NotNullableConstraint()
        {
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
    }
}
