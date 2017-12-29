// --------------------------------------------------------------------------------------------
// <copyright file="FieldValueFixture.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

namespace Effort.Test.Csv
{
    using System;
    using Effort.Internal.Csv;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class FieldValueFixture
    {
        private const string TestString = "Hello World!";

        [Test]
        public void DefaultFieldValue()
        {
            FieldValue[] value = new FieldValue[1];

            value[0].IsMissing.Should().BeTrue();;
        }

        [Test]
        public void ValuedFieldValue()
        {
            FieldValue value = TestString;

            value.IsMissing.Should().BeFalse();
            value.Value.Should().Be(TestString);
        }

        [Test]
        public void NullValuedFieldValue()
        {
            FieldValue value = null;

            value.IsMissing.Should().BeFalse();
            value.Value.Should().Be(null);
        }

        [Test]
        public void MissingFieldValue()
        {
            FieldValue value = FieldValue.Missing;

            value.IsMissing.Should().BeTrue();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void MissingFieldValueEvaluation()
        {
            FieldValue value = FieldValue.Missing;

            string eval = value.Value;
        }

        [Test]
        public void MissingFieldAndStringAddition()
        {
            FieldValue left = FieldValue.Missing;
            string right = TestString;

            FieldValue result = left + right;

            result.IsMissing.Should().BeFalse();
            result.Value.Should().Be(TestString);
        }

        [Test]
        public void MissingFieldAndStringAddition2()
        {
            string left = TestString;
            FieldValue right = FieldValue.Missing;

            FieldValue result = left + right;

            result.IsMissing.Should().BeFalse();
            result.Value.Should().Be(TestString);
        }
    }
}
