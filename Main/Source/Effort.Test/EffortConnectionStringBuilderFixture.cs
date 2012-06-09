using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Provider;

namespace Effort.Test
{
    [TestClass]
    public class EffortConnectionStringBuilderFixture
    {
        [TestMethod]
        public void EffortConnectionStringBuilder_InstanceId()
        {
            EffortConnectionStringBuilder writer = new EffortConnectionStringBuilder();
            writer.InstanceId = "InstanceId";

            EffortConnectionStringBuilder reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual("InstanceId", reader.InstanceId);
        }

        [TestMethod]
        public void EffortConnectionStringBuilder_DataLoaderArgument()
        {
            EffortConnectionStringBuilder writer = new EffortConnectionStringBuilder();
            writer.DataLoaderArgument = "LoaderArgument";

            EffortConnectionStringBuilder reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual("LoaderArgument", reader.DataLoaderArgument);
        }

        [TestMethod]
        public void EffortConnectionStringBuilder_DataLoaderType()
        {
            EffortConnectionStringBuilder writer = new EffortConnectionStringBuilder();
            writer.DataLoaderType = typeof(Effort.DataLoaders.EmptyDataLoader);

            EffortConnectionStringBuilder reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual(typeof(Effort.DataLoaders.EmptyDataLoader), reader.DataLoaderType);
        }

        [TestMethod]
        public void EffortConnectionStringBuilder_DataLoaderCached()
        {
            EffortConnectionStringBuilder writer = new EffortConnectionStringBuilder();
            writer.DataLoaderCached = true;

            EffortConnectionStringBuilder reader = new EffortConnectionStringBuilder(writer.ConnectionString);

            Assert.AreEqual(true, reader.DataLoaderCached);
        }
    }
}
