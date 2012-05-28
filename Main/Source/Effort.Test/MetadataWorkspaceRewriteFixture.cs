using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Effort.Internal.Common;
using Effort.Provider;

namespace Effort.Test
{
    [TestClass]
    public class MetadataWorkspaceRewriteFixture
    {
        [TestMethod]
        public void Rewrite()
        {
            MetadataWorkspaceHelper.Rewrite("res://*/Data.Northwind.csdl|res://*/Data.Northwind.ssdl|res://*/Data.Northwind.msl", EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }
    }
}
