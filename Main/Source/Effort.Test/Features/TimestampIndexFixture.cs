// --------------------------------------------------------------------------------------------
// <copyright file="TimestampIndexFixture.cs" company="Effort Team">
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

#if EF61

namespace Effort.Test.Features
{
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class TimestampIndexFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<
                        TimestampIndexedFieldEntity>());
        }

        [TestMethod]
        public void TimestampIndexedSingleInsert()
        {
            TimestampIndexedFieldEntity timestamp = new TimestampIndexedFieldEntity();
            timestamp.Data = "New record";

            context.TimestampIndexedFieldEntities.Add(timestamp);
            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
        }

        [TestMethod]
        public void TimestampIndexedDoubleInsert()
        {
            TimestampIndexedFieldEntity timestamp = new TimestampIndexedFieldEntity();
            timestamp.Data = "New record1";
            context.TimestampIndexedFieldEntities.Add(timestamp);

            TimestampIndexedFieldEntity timestamp2 = new TimestampIndexedFieldEntity();
            timestamp2.Data = "New record2";
            context.TimestampIndexedFieldEntities.Add(timestamp2);

            context.SaveChanges();

            Assert.IsTrue(timestamp.Timestamp.Any(b => b > 0));
            Assert.IsTrue(timestamp2.Timestamp.Any(b => b > 0));
            context.TimestampIndexedFieldEntities.Count().ShouldEqual(2);
        }

    }
}

#endif
