using System.Data.Metadata.Edm;
using Effort.Internal.Common;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Effort.Test.Data.Northwind;
using System.Data.EntityClient;

namespace Effort.Test
{
    [TestClass]
    public class MetadataWorkspaceRewriteFixture
    {
        [TestMethod]
        public void RewriteMetadataWorkspace()
        {
            EntityConnectionStringBuilder nameResolver = new EntityConnectionStringBuilder(NorthwindObjectContext.DefaultConnectionString);
            string resolvedConnectionString = ConfigurationManager.ConnectionStrings[nameResolver.Name].ConnectionString;
            EntityConnectionStringBuilder connectionString = new EntityConnectionStringBuilder(resolvedConnectionString);

            MetadataWorkspace workspace = MetadataWorkspaceHelper.Rewrite(connectionString.Metadata, EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }
    }
}
