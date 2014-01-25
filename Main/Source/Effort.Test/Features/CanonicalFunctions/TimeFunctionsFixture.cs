// --------------------------------------------------------------------------------------------
// <copyright file="TimeFunctionsFixture.cs" company="Effort Team">
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
#if EFOLD
    using System.Data.Objects;
#endif
    using System;
    using System.Linq;
    using Effort.Test.Data.Features;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;
    using System.Data.Entity;

    [TestClass]
    public class TimeFunctionsFixture
    {
        private FeatureDbContext context;
        private static readonly TimeSpan stored = new TimeSpan(3, 4, 5);

        [TestInitialize]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    Effort.DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<TimeFieldEntity>());

            this.Entities.Add(
                new TimeFieldEntity
                {
                    Time = stored
                });

            this.context.SaveChanges();
        }

        public IDbSet<TimeFieldEntity> Entities
        {
            get
            {
                return this.context.TimeFieldEntities;
            }
        }

        [TestMethod]
        public void TimeHour()
        {
            var q = this.Entities
                .Where(x =>
                    x.Time.Hours == 3);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeMinute()
        {
            var q = this.Entities
                .Where(x =>
                    x.Time.Minutes == 4);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeSecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.Time.Seconds == 5);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeMillisecond()
        {
            var q = this.Entities
                .Where(x =>
                    x.Time.Milliseconds == 0);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddNanoseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromTicks(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddNanoseconds(x.Time, -100) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddNanoseconds(x.Time, -100) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddMicroseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromTicks(-10));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMicroseconds(x.Time, -1) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMicroseconds(x.Time, -1) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddMilliseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromMilliseconds(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMilliseconds(x.Time, -1) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMilliseconds(x.Time, -1) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddSeconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromSeconds(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddSeconds(x.Time, -1) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddSeconds(x.Time, -1) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddMinutes()
        {
            TimeSpan time = stored.Add(TimeSpan.FromMinutes(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddMinutes(x.Time, -1) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddMinutes(x.Time, -1) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeAddHours()
        {
            TimeSpan time = stored.Add(TimeSpan.FromHours(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.AddHours(x.Time, -1) == time);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.AddHours(x.Time, -1) == time);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffNanoseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromTicks(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffNanoseconds(time, x.Time) == 100);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffNanoseconds(time, x.Time) == 100);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffMicroseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromTicks(-10));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMicroseconds(time, x.Time) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMicroseconds(time, x.Time) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffMilliseconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromMilliseconds(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMilliseconds(time, x.Time) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMilliseconds(time, x.Time) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffSeconds()
        {
            TimeSpan time = stored.Add(TimeSpan.FromSeconds(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffSeconds(time, x.Time) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffSeconds(time, x.Time) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffMinutes()
        {
            TimeSpan time = stored.Add(TimeSpan.FromMinutes(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffMinutes(time, x.Time) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffMinutes(time, x.Time) == 1);
#endif

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void TimeDiffHours()
        {
            TimeSpan time = stored.Add(TimeSpan.FromHours(-1));

#if !EFOLD
            var q = this.Entities
                .Where(x =>
                    DbFunctions.DiffHours(time, x.Time) == 1);
#else
            var q = this.Entities
                .Where(x =>
                    EntityFunctions.DiffHours(time, x.Time) == 1);
#endif

            q.ShouldNotBeEmpty();
        }


        [TestMethod]
        public void CreateTime()
        {
#if !EFOLD
            var q = this.Entities
                .Select(x => 
                    DbFunctions.CreateTime(x.Time.Hours - 1, 2, 1));
#else
            var q = this.Entities
                .Select(x => 
                    EntityFunctions.CreateTime(x.Time.Hours - 1, 2, 1));
#endif

            var q2 = q.AsEnumerable().Where(x => x.Value.Hours == 2);
                
            q2.ShouldNotBeEmpty();
        }
    }
}
