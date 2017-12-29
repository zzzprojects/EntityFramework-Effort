// --------------------------------------------------------------------------------------------
// <copyright file="ResultSetFixture.cs" company="Effort Team">
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

namespace Effort.Test
{
    using System.Collections.Generic;
    using Effort.Test.Internal.ResultSets;
    using NUnit.Framework;

    [TestFixture]
    public class ResultSetFixture
    {
        [Test]
        public void SerializeResultSet()
        {
            IResultSet resultSet =
                new DictionaryResultSet(
                    new[] {
                        new Dictionary<string, object> {
                            { "a", 1 },
                            { "b", true },
                            { "c", null }
                        },
                        new Dictionary<string, object> {
                            { "a", 2 },
                            { "b", true },
                            { "c", "string" }
                        }
                    });

            string serialized = ResultSetJsonSerializer.Serialize(resultSet);

            Assert.AreEqual("[{\"a\":1,\"b\":true,\"c\":null},{\"a\":2,\"b\":true,\"c\":\"string\"}]", serialized);    
        }
    }
}
