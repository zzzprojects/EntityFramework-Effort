// --------------------------------------------------------------------------------------------
// <copyright file="TimestampFixture.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class TimestampFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = 
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<
                        TimestampFieldEntity, 
                        LargeTimestampFieldEntity>());
        }

        [TestMethod]
        public void TimestampInsert()
        {
            TimestampFieldEntity timestamp = new TimestampFieldEntity();
            timestamp.Data = "New record";
            
            context.TimestampFieldEntities.Add(timestamp);
            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void TimestampUpdate()
        {
            TimestampFieldEntity timestamp = new TimestampFieldEntity();
            timestamp.Data = "New record";

            context.TimestampFieldEntities.Add(timestamp);
            context.SaveChanges();

            byte[] currentValue = timestamp.Timestamp;

            timestamp.Data += "(updated)"; 
            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Select((v, i) => v != currentValue[i]).Any(x => x));
            //Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void TimestampEntityMaterialize()
        {
            TimestampFieldEntity timestamp = new TimestampFieldEntity();
            timestamp.Data = "New record";

            context.TimestampFieldEntities.Add(timestamp);
            context.SaveChanges();

            var data = context.TimestampFieldEntities.ToList();

            data.ShouldHaveCountOf(1);
            data[0].ShouldNotBeNull();
            data[0].Timestamp.All(x => x != 0).ShouldBeFalse();
        }

        [TestMethod]
        public void TimestampLargeEntityMaterialize()
        {
            LargeTimestampFieldEntity timestamp = new LargeTimestampFieldEntity();

            context.LargeTimestampFieldEntities.Add(timestamp);
            context.SaveChanges();

            var data = context.LargeTimestampFieldEntities.ToList();

            data.ShouldHaveCountOf(1);
            data[0].ShouldNotBeNull();
            data[0].Timestamp.All(x => x != 0).ShouldBeFalse();
        }
    }
}
