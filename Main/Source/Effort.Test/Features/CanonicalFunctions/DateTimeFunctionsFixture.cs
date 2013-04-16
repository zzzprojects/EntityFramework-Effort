// --------------------------------------------------------------------------------------------
// <copyright file="DateTimeFunctionsFixture.cs" company="Effort Team">
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
#if !EFOLD
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity;
#else
    using System.Data.Objects;
#endif
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;
    using Effort.Test.Data.Staff;

    [TestClass]
    public class DateTimeFunctionsFixture
    {
        private StaffDbContext context;
        private static readonly DateTime storedDate = new DateTime(2012, 1, 2, 3, 4, 5, 100);

        [TestInitialize]
        public void Initialize()
        {
            this.context =
                new StaffDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<DateTimeFieldEntity>());

            this.context.DateTimeFieldEntities.Add(
                new DateTimeFieldEntity
                {
                    DateTime = storedDate
                });

            this.context.SaveChanges();
        }

        public IQueryable<DateTimeFieldEntity> Entities
        {
            get
            {
                return this.context.DateTimeFieldEntities;
            }
        }

        [TestMethod]
        public void DateTimeYear()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Year == 2012);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMonth()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Month == 1);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDay()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Day == 2);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDayHour()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Hour == 3);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMinute()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Minute == 4);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeSecond()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Second == 5);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMillisecond()
        {
            var query = this.Entities
                .Where(x =>
                    x.DateTime.Millisecond == 100);

            query.ShouldNotBeEmpty();
        }
      

        [TestMethod]
        public void DateTimeAddMilliseconds()
        {
            DateTime date = storedDate.AddMilliseconds(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddMilliseconds(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddMilliseconds(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddSeconds()
        {
            DateTime date = storedDate.AddSeconds(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddSeconds(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddSeconds(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddMinutes()
        {
            DateTime date = storedDate.AddMinutes(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddMinutes(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddMinutes(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddHours()
        {
            DateTime date = storedDate.AddHours(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddHours(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddHours(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddDays()
        {
            DateTime date = storedDate.AddDays(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddDays(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddDays(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddMonths()
        {
            DateTime date = storedDate.AddMonths(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddMonths(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddMonths(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddYears()
        {
            DateTime date = storedDate.AddYears(-1);
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.AddYears(x.DateTime, -1) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.AddYears(x.DateTime, -1) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeTruncateTime()
        {
            DateTime date = storedDate.Date;
#if !EFOLD
            var query = this.Entities
                .Where(x =>
                    DbFunctions.TruncateTime(x.DateTime) == date);
#else
            var query = this.Entities
                .Where(x =>
                    EntityFunctions.TruncateTime(x.DateTime) == date);
#endif

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeNow()
        {
            this.context.DateTimeFieldEntities.Add(
                new DateTimeFieldEntity()
                {
                    DateTime = DateTime.Now
                });

            this.context.SaveChanges();

            var query = this.Entities
                .Where(x =>
                    x.DateTime.Year == DateTime.Now.Year);

            query.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeUtcNow()
        {
            this.context.DateTimeFieldEntities.Add(
                new DateTimeFieldEntity()
                {
                    DateTime = DateTime.UtcNow
                });

            this.context.SaveChanges();

            var query = this.Entities
                .Where(x =>
                    x.DateTime.Year == DateTime.UtcNow.Year);

            query.ShouldNotBeEmpty();
        }
    }
}
