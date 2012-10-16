using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Data.Feature;
using SoftwareApproach.TestingExtensions;

namespace Effort.Test
{
    [TestClass]
    public class MultiFieldAssociationFixture
    {
        private FeatureObjectContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = new LocalFeatureObjectContext();
        }

        [TestMethod]
        public void MultipleAssociation()
        {
            this.context.PrimaryEntities.AddObject(new PrimaryEntity { ID1 = 1, ID2 = 10, PrimaryData = "Data" });
            this.context.PrimaryEntities.AddObject(new PrimaryEntity { ID1 = 2, ID2 = 20, PrimaryData = "Beta" });
            this.context.ForeignEntities.AddObject(new ForeignEntity { FID1 = 1, FID2 = 10, ID = 1 });
            this.context.SaveChanges();

            var query = from prim in this.context.PrimaryEntities
                        join sec in this.context.ForeignEntities
                        on new { prim.ID1, prim.ID2 } equals new { ID1 = sec.FID1, ID2 = sec.FID2 }
                        select new { Prim = prim, Sec = sec };

            query.ToList().First().Prim.PrimaryData.ShouldEqual("Data");
        }
    }
}
