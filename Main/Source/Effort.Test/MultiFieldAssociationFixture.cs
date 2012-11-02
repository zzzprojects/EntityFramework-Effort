// ----------------------------------------------------------------------------------
// <copyright file="MultiFieldAssociationFixture.cs" company="Effort Team">
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
    using System.Linq;
    using Effort.Test.Data.Feature;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

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
            this.context.PrimaryEntities.AddObject(
                new PrimaryEntity { 
                    ID1 = 1, 
                    ID2 = 10, 
                    PrimaryData = "Data" });

            this.context.PrimaryEntities.AddObject(
                new PrimaryEntity { 
                    ID1 = 2, 
                    ID2 = 20, 
                    PrimaryData = "Beta" });

            this.context.ForeignEntities.AddObject(
                new ForeignEntity { 
                    FID1 = 1, 
                    FID2 = 10, 
                    ID = 1});

            this.context.SaveChanges();

            var query = from prim in this.context.PrimaryEntities
                        join sec in this.context.ForeignEntities
                        on new { prim.ID1, prim.ID2 } equals new { ID1 = sec.FID1, ID2 = sec.FID2 }
                        select new { Prim = prim, Sec = sec };

            query.ToList().First().Prim.PrimaryData.ShouldEqual("Data");
        }
    }
}
