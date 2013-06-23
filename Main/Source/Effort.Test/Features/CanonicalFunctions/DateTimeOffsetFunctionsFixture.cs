// --------------------------------------------------------------------------------------------
// <copyright file="DateTimeOffsetFunctionsFixture.cs" company="Effort Team">
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

namespace Effort.Test.Features.CanonicalFunctions
{
    using System;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class DateTimeOffsetFunctionsFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context = 
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<DateTimeOffsetFieldEntity>());

            this.context.DateTimeOffsetFieldEntities.Add(
                new DateTimeOffsetFieldEntity
                {
                    Offset = new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan())
                });

            this.context.SaveChanges();
        }

        public IQueryable<DateTimeOffsetFieldEntity> Entities
        {
            get
            {
                return this.context.DateTimeOffsetFieldEntities;
            }
        }

        [TestMethod]
        public void DateTimeOffsetYear()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Year == 2012);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMonth()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Month == 1);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDay()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Day == 2);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetHour()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Hour == 3);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMinute()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Minute == 4);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetSecond()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Second == 5);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMillisecond()
        {
            var query = this.Entities
                .Where(x =>
                    x.Offset.Millisecond == 100);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetCurrentDateTimeOffset()
        {
            this.context.DateTimeOffsetFieldEntities.Add(
                new DateTimeOffsetFieldEntity()
                {
                    Offset = DateTimeOffset.Now
                });

            this.context.SaveChanges();


            var query = this.Entities
                .Where(x =>
                    x.Offset.Year == DateTimeOffset.Now.Year);

            query.ShouldNotBeEmpty();
        }
    }
}
