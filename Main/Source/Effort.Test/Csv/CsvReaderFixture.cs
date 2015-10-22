// --------------------------------------------------------------------------------------------
// <copyright file="CsvReaderFixture.cs" company="Effort Team">
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
    using System.IO;
    using Effort.Internal.Csv;
    using FluentAssertions;
    using NUnit.Framework;

    [TestFixture]
    public class CsvReaderFixture
    {
        [Test]
        public void CsvReader_EmptyVsNull1()
        {
            string body = ",item";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().BeNull();
            reader[1].Should().Be("item");
        }

        [Test]
        public void CsvReader_EmptyVsNull2()
        {
            string body = "\"\",item";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().Be("");
            reader[1].Should().Be("item");
        }

        [Test]
        public void CsvReader_EmptyVsNull3()
        {
            string body = "item,\"\"";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().Be("item");
            reader[1].Should().Be("");
        }

        [Test]
        public void CsvReader_EmptyVsNull4()
        {
            string body = "item,";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().Be("item");
            reader[1].Should().BeNull();
        }

        [Test]
        public void CsvReader_EmptyVsNull5()
        {
            string body = ",";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().BeNull();
            reader[1].Should().BeNull();
        }

        [Test]
        public void CsvReader_Read1()
        {
            string body = "1,\"2\",3";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().Should().BeTrue();
            reader[0].Should().Be("1");
            reader[1].Should().Be("2");
            reader[2].Should().Be("3");
        }
    }
}
