// --------------------------------------------------------------------------------------------
// <copyright file="DateTimeFunctionsFixture.cs" company="Effort Team">
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

namespace Effort.Test.Features.CanonicalFunctions
{
#if !EFOLD
    using System.Data.Entity;
#else
    using System.Data.Objects;
#endif
    using System;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class DateTimeFunctionsFixture
    {
        private FeatureDbContext context;
        private static readonly DateTime stored = new DateTime(1988, 1, 2, 3, 4, 5, 100);

        [TestInitialize]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<DateTimeFieldEntity>());

            this.context.DateTimeFieldEntities.Add(
                new DateTimeFieldEntity
                {
                    DateTime = stored,
                    DateTimeN = null
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
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Year == 1988);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMonth()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Month == 1);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDay()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Day == 2);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDayHour()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Hour == 3);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMinute()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Minute == 4);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeSecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Second == 5);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeMillisecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.DateTime.Millisecond == 100);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddNanoseconds()
        {
            DateTime date = stored.AddTicks(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddNanoseconds(x.DateTime, -100) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddNanoseconds(x.DateTime, -100) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddMicroseconds()
        {
            DateTime date = stored.AddTicks(-10);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMicroseconds(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMicroseconds(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddMilliseconds()
        {
            DateTime date = stored.AddMilliseconds(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMilliseconds(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMilliseconds(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddSeconds()
        {
            DateTime date = stored.AddSeconds(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddSeconds(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddSeconds(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddMinutes()
        {
            DateTime date = stored.AddMinutes(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMinutes(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMinutes(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddHours()
        {
            DateTime date = stored.AddHours(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddHours(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddHours(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddDays()
        {
            DateTime date = stored.AddDays(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddDays(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddDays(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddMonths()
        {
            DateTime date = stored.AddMonths(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMonths(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMonths(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeAddYears()
        {
            DateTime date = stored.AddYears(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddYears(x.DateTime, -1) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddYears(x.DateTime, -1) == date);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffNanoseconds()
        {
            DateTime date = stored.AddTicks(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffNanoseconds(date, x.DateTime) == 100);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffNanoseconds(date, x.DateTime) == 100);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffMicroseconds()
        {
            DateTime date = stored.AddTicks(-10);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMicroseconds(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMicroseconds(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffMilliseconds()
        {
            DateTime date = stored.AddMilliseconds(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMilliseconds(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMilliseconds(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffSeconds()
        {
            DateTime date = stored.AddSeconds(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffSeconds(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffSeconds(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffMinutes()
        {
            DateTime date = stored.AddMinutes(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMinutes(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMinutes(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffHours()
        {
            DateTime date = stored.AddHours(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffHours(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffHours(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffDays()
        {
            DateTime date = stored.AddDays(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffDays(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffDays(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffMonths()
        {
            DateTime date = stored.AddYears(-1).AddMonths(-1);

#if !EFOLD
            var q = this.Entities
                .Select(x =>
                    DbFunctions.DiffMonths(date, x.DateTime) == 13);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMonths(date, x.DateTime) == 13);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeDiffYears()
        {
            DateTime date = stored.AddYears(-1);
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffYears(date, x.DateTime) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffYears(date, x.DateTime) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeTruncateTime()
        {
            DateTime date = stored.Date;
#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.TruncateTime(x.DateTime) == date);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.TruncateTime(x.DateTime) == date);
#endif

            q.ShouldNotBeEmpty();
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

            var q = this.Entities
                .Where(x =>
                    x.DateTime.Year == DateTime.Now.Year);

            q.ShouldNotBeEmpty();
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

            var q = this.Entities
                .Where(x =>
                    x.DateTime.Year == DateTime.UtcNow.Year);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void CreateDateTime()
        {
#if !EFOLD
            var q = this.Entities
                .Select(x => DbFunctions.CreateDateTime(x.DateTime.Year, 5, 1, 0, 0, 0));
#else
            var q = this.Entities
                .Select(x => EntityFunctions.CreateDateTime(x.DateTime.Year, 5, 1, 0, 0, 0));
#endif


            var q2 = q.AsEnumerable()
                .Where(x => x.Value.Month == 5);
                
            q2.ShouldNotBeEmpty();
        }
    }
}
