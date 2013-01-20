// --------------------------------------------------------------------------------------------
// <copyright file="CsvReaderFixture.cs" company="Effort Team">
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

namespace Effort.Test.Csv
{
    using System.IO;
    using Effort.Internal.Csv;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SoftwareApproach.TestingExtensions;

    [TestClass]
    public class CsvReaderFixture
    {
        [TestMethod]
        public void CsvReader_EmptyVsNull1()
        {
            string body = ",item";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldBeNull();
            reader[1].ShouldEqual("item");
        }

        [TestMethod]
        public void CsvReader_EmptyVsNull2()
        {
            string body = "\"\",item";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldEqual("");
            reader[1].ShouldEqual("item");
        }

        [TestMethod]
        public void CsvReader_EmptyVsNull3()
        {
            string body = "item,\"\"";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldEqual("item");
            reader[1].ShouldEqual("");
        }

        [TestMethod]
        public void CsvReader_EmptyVsNull4()
        {
            string body = "item,";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldEqual("item");
            reader[1].ShouldBeNull();
        }

        [TestMethod]
        public void CsvReader_EmptyVsNull5()
        {
            string body = ",";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldBeNull();
            reader[1].ShouldBeNull();
        }

        [TestMethod]
        public void CsvReader_Read1()
        {
            string body = "1,\"2\",3";

            CsvReader reader = new CsvReader(new StringReader(body), false);

            reader.ReadNextRecord().ShouldBeTrue();
            reader[0].ShouldEqual("1");
            reader[1].ShouldEqual("2");
            reader[2].ShouldEqual("3");
        }
    }
}
