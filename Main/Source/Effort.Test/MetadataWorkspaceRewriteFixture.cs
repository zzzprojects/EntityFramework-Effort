using System.Data.Metadata.Edm;
using Effort.Internal.Common;
using Effort.Provider;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Effort.Test.Data.Northwind;
using System.Data.EntityClient;
using System.Reflection;
using System.Xml.Linq;
using System.IO;
using System.Data.Common;
using System.Xml;

namespace Effort.Test
{
    [TestClass]
    public class MetadataWorkspaceRewriteFixture
    {
        private DbProviderManifest sqlProviderManifest;
        private DbProviderManifest effortProviderManifest;

        [TestInitialize]
        public void Initialize()
        {
            this.sqlProviderManifest = ProviderHelper.GetProviderManifest("System.Data.SqlClient", "2008");
            this.effortProviderManifest = ProviderHelper.GetProviderManifest(EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }

        [TestMethod]
        public void RewriteMetadataWorkspace()
        {
            EntityConnectionStringBuilder nameResolver = new EntityConnectionStringBuilder(NorthwindObjectContext.DefaultConnectionString);
            string resolvedConnectionString = ConfigurationManager.ConnectionStrings[nameResolver.Name].ConnectionString;
            EntityConnectionStringBuilder connectionString = new EntityConnectionStringBuilder(resolvedConnectionString);

            MetadataWorkspace workspace = MetadataWorkspaceHelper.Rewrite(connectionString.Metadata, EffortProviderConfiguration.ProviderInvariantName, EffortProviderManifestTokens.Version1);
        }

        [TestMethod]
        public void RewriteSsdlV1()
        {
            XElement ssdl = LoadXml("Effort.Test.Environment.Resources.StorageSchemas.SSDLv1.xml");
            ChangeProviderAttributes(ssdl);

            MetadataWorkspaceHelper.RewriteTypeAttributeValues(ssdl, this.effortProviderManifest, this.sqlProviderManifest);

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        public void RewriteSsdlV2()
        {
            XElement ssdl = LoadXml("Effort.Test.Environment.Resources.StorageSchemas.SSDLv2.xml");
            ChangeProviderAttributes(ssdl);

            MetadataWorkspaceHelper.RewriteTypeAttributeValues(ssdl, this.effortProviderManifest, this.sqlProviderManifest);

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        public void RewriteSsdlV3()
        {
            XElement ssdl = LoadXml("Effort.Test.Environment.Resources.StorageSchemas.SSDLv3.xml");
            ChangeProviderAttributes(ssdl);

            MetadataWorkspaceHelper.RewriteTypeAttributeValues(ssdl, this.effortProviderManifest, this.sqlProviderManifest);

            ValidateSsdl(ssdl); 
        }

        private static XElement LoadXml(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return XDocument.Load(stream).Root;
        }

        private static void ChangeProviderAttributes(XElement ssdl)
        {
            ssdl.Attribute("Provider").Value = EffortProviderConfiguration.ProviderInvariantName;
            ssdl.Attribute("ProviderManifestToken").Value = EffortProviderManifestTokens.Version1;
        }

        private static void ValidateSsdl(XElement ssdl)
        {
            new StoreItemCollection(new XmlReader[] { ssdl.CreateReader() });
        }
    }
}
