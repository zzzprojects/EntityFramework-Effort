using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Test.Environment.ResultSets;

namespace Effort.Test
{
    [TestClass]
    public class ResultSetFixture
    {
        [TestMethod]
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
