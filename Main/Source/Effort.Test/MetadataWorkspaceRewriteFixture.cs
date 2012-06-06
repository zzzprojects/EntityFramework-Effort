using System.Data.Metadata.Edm;
using Effort.Internal.Common;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Effort.Test
{
    [TestClass]
    public class MetadataWorkspaceRewriteFixture
    {
        [TestMethod]
        public void RewriteMetadataWorkspace()
        {
            MetadataWorkspace workspace = MetadataWorkspaceHelper.Rewrite("res://*/Data.Northwind.csdl|res://*/Data.Northwind.ssdl|res://*/Data.Northwind.msl", EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }
    }
}
