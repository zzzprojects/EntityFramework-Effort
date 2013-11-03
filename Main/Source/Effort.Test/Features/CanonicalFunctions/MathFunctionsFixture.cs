// --------------------------------------------------------------------------------------------
// <copyright file="MathFunctionsFixture.cs" company="Effort Team">
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
    public class MathFunctionsFixture
    {
        private FeatureDbContext context;

        [TestInitialize]
        public void Initialize()
        {
            this.context =
                new FeatureDbContext(
                    DbConnectionFactory.CreateTransient(),
                    CompiledModels.GetModel<MathEntity>());
        }

        [TestMethod]
        public void DecimalCeiling()
        {
            this.context.MathEntities.Add(new MathEntity { Decimal = 1.4M });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Ceiling(x.Decimal) == 2.0M);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalCeilingNull()
        {
            this.context.MathEntities.Add(new MathEntity { DecimalN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Ceiling(x.DecimalN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleCeiling()
        {
            this.context.MathEntities.Add(new MathEntity { Double = 1.4 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Ceiling(x.Double) == 2.0);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleCeilingNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Ceiling(x.DoubleN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalFloor()
        {
            this.context.MathEntities.Add(new MathEntity { Decimal = 1.7M });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Floor(x.Decimal) == 1.0M);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalFloorNull()
        {
            this.context.MathEntities.Add(new MathEntity { DecimalN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Floor(x.DecimalN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleFloor()
        {
            this.context.MathEntities.Add(new MathEntity { Double = 1.7 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Floor(x.Double) == 1.0);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleFloorNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Floor(x.DoubleN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalRound()
        {
            this.context.MathEntities.Add(new MathEntity { Decimal = 1.7M });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.Decimal) == 2.0M);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalRoundNull()
        {
            this.context.MathEntities.Add(new MathEntity { DecimalN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.DecimalN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleRound()
        {
            this.context.MathEntities.Add(new MathEntity { Double = 1.7 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.Double) == 2.0);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleRoundNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.DoubleN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalRoundDigit()
        {
            this.context.MathEntities.Add(new MathEntity { Decimal = 1.77777M });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.Decimal, 2) == 1.78M);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalRoundDigitNull()
        {
            this.context.MathEntities.Add(new MathEntity { DecimalN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.DecimalN.Value, 2) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleRoundDigit()
        {
            this.context.MathEntities.Add(new MathEntity { Double = 1.77777 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.Double, 2) == 1.78);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleRoundDigitNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Round(x.DoubleN.Value, 2) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void Power()
        {
            this.context.MathEntities.Add(new MathEntity { Double = 2 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Pow(x.Double, 2) == 4);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void PowerNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x =>  Math.Pow(x.DoubleN.Value, 2) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalAbs()
        {
            this.context.MathEntities.Add(new MathEntity { Decimal = -1.7M });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.Decimal) == 1.7M);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DecimalAbsNull()
        {
            this.context.MathEntities.Add(new MathEntity { DecimalN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.DecimalN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleAbs()
        {
            this.context.MathEntities.Add(new MathEntity { Double = -1.7 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.Double) == 1.7);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void DoubleAbsNull()
        {
            this.context.MathEntities.Add(new MathEntity { DoubleN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.DoubleN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void SByteAbs()
        {
            // Entity Framework does not support SByte Abs
             
            ////this.context.MathEntities.Add(new MathEntity { SByte = -1 });
            ////this.context.SaveChanges();

            ////var q = this.context
            ////    .MathEntities
            ////    .Where(x => Math.Abs(x.SByte) == 1);

            ////q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void SByteAbsNull()
        {
            // Entity Framework does not support SByte Abs

            ////this.context.MathEntities.Add(new MathEntity { SByteN = null });
            ////this.context.SaveChanges();

            ////var q = this.context
            ////    .MathEntities
            ////    .Where(x => Math.Abs(x.SByteN.Value) == null);

            ////q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void ShortAbs()
        {
            this.context.MathEntities.Add(new MathEntity { Short = -1 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.Short) == 1);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void ShortAbsNull()
        {
            this.context.MathEntities.Add(new MathEntity { ShortN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.ShortN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void IntAbs()
        {
            this.context.MathEntities.Add(new MathEntity { Int = -1 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.Int) == 1);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void IntAbsNull()
        {
            this.context.MathEntities.Add(new MathEntity { IntN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.IntN.Value) == null);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void LongAbs()
        {
            this.context.MathEntities.Add(new MathEntity { Long = -1 });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.Long) == 1);

            q.ShouldNotBeEmpty();
        }

        [TestMethod]
        public void LongAbsNull()
        {
            this.context.MathEntities.Add(new MathEntity { LongN = null });
            this.context.SaveChanges();

            var q = this.context
                .MathEntities
                .Where(x => Math.Abs(x.LongN.Value) == null);

            q.ShouldNotBeEmpty();
        }
    }
}
