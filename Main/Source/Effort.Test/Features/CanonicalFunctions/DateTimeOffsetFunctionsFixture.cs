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
#if !EFOLD
#else
    using System.Data.Objects;
#endif
    using System;
    using System.Data.Entity;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;


    [TestClass]
    public class DateTimeOffsetFunctionsFixture
    {
        private FeatureDbContext context;

        private static readonly DateTimeOffset stored = 
            new DateTimeOffset(2012, 1, 2, 3, 4, 5, 100, new TimeSpan(1, 0, 0));


        [TestInitialize]
        public void Initialize()
        {
            this.context = 
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<DateTimeOffsetFieldEntity>());

            this.Entities.Add(
                new DateTimeOffsetFieldEntity
                {
                    Offset = stored
                });

            this.context.SaveChanges();
        }

        public IDbSet<DateTimeOffsetFieldEntity> Entities
        {
            get
            {
                return this.context.DateTimeOffsetFieldEntities;
            }
        }

        [TestMethod]
        public void DateTimeOffsetYear()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Year == 2012);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMonth()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Month == 1);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDay()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Day == 2);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetHour()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Hour == 3);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMinute()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Minute == 4);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetSecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Second == 5);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetMillisecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.Offset.Millisecond == 100);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddNanoseconds()
        {
            DateTimeOffset offset = stored.AddTicks(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddNanoseconds(x.Offset, -100) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddNanoseconds(x.Offset, -100) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddMicroseconds()
        {
            DateTimeOffset offset = stored.AddTicks(-10);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMicroseconds(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMicroseconds(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddMilliseconds()
        {
            DateTimeOffset offset = stored.AddMilliseconds(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMilliseconds(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMilliseconds(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddSeconds()
        {
            DateTimeOffset offset = stored.AddSeconds(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddSeconds(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddSeconds(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddMinutes()
        {
            DateTimeOffset offset = stored.AddMinutes(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMinutes(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMinutes(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddHours()
        {
            DateTimeOffset offset = stored.AddHours(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddHours(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddHours(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddDays()
        {
            DateTimeOffset offset = stored.AddDays(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddDays(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddDays(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddMonths()
        {
            DateTimeOffset offset = stored.AddMonths(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMonths(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMonths(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetAddYears()
        {
            DateTimeOffset offset = stored.AddYears(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddYears(x.Offset, -1) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddYears(x.Offset, -1) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffNanoseconds()
        {
            DateTimeOffset offset = stored.AddTicks(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffNanoseconds(offset, x.Offset) == 100);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffNanoseconds(offset, x.Offset) == 100);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffMicroseconds()
        {
            DateTimeOffset offset = stored.AddTicks(-10);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMicroseconds(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMicroseconds(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffMilliseconds()
        {
            DateTimeOffset offset = stored.AddMilliseconds(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMilliseconds(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMilliseconds(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffSeconds()
        {
            DateTimeOffset offset = stored.AddSeconds(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffSeconds(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffSeconds(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffMinutes()
        {
            DateTimeOffset offset = stored.AddMinutes(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMinutes(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMinutes(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffHours()
        {
            DateTimeOffset offset = stored.AddHours(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffHours(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffHours(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffDays()
        {
            DateTimeOffset offset = stored.AddDays(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffDays(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffDays(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffMonths()
        {
            DateTimeOffset offset = stored.AddYears(-1).AddMonths(-1);

#if !EFOLD
            var q = this.Entities
                .Select(x =>
                    DbFunctions.DiffMonths(offset, x.Offset) == 13);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMonths(offset, x.Offset) == 13);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetDiffYears()
        {
            DateTimeOffset offset = stored.AddYears(-1);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffYears(offset, x.Offset) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffYears(offset, x.Offset) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DateTimeOffsetTruncateTime()
        {
            DateTimeOffset offset = new DateTimeOffset(stored.Date, stored.Offset);

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.TruncateTime(x.Offset) == offset);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.TruncateTime(x.Offset) == offset);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void CurrentDateTimeOffset()
        {
            this.Entities.Add(
                new DateTimeOffsetFieldEntity()
                {
                    Offset = DateTimeOffset.Now
                });

            this.context.SaveChanges();

            var q = this.Entities
                .Where(x =>
                    x.Offset.Year == DateTimeOffset.Now.Year);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void OffsetOfDateTimeOffset()
        {
            this.Entities.Add(
                new DateTimeOffsetFieldEntity()
                {
                    Offset = DateTimeOffset.Now
                });

            this.context.SaveChanges();

#if !EFOLD
            var q = this.Entities
                .Where(x => 
                    DbFunctions.GetTotalOffsetMinutes(x.Offset) == 60);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.GetTotalOffsetMinutes(x.Offset) == 60);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void CreateDateTimeOffset()
        {
            this.context.DateTimeOffsetFieldEntities.Add(
                new DateTimeOffsetFieldEntity()
                {
                    Offset = DateTimeOffset.Now
                });

            this.context.SaveChanges();

#if !EFOLD          
            var q = this.Entities
                .Select(x =>
                    DbFunctions.CreateDateTimeOffset(x.Offset.Year, 1, 1, 0, 0, 0, 60));
#else
            var q = this.Entities
                .Select(x =>
                    EntityFunctions.CreateDateTimeOffset(x.Offset.Year, 1, 1, 0, 0, 0, 60));
#endif

            var q2 = q.AsEnumerable().Where(x => x.Value.Offset.TotalHours == 1);

            q.ShouldNotBeEmpty();
        }
    }
}
