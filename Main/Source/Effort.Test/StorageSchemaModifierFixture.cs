// ----------------------------------------------------------------------------------
// <copyright file="StorageSchemaModifierFixture.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Test
{
    using System.Configuration;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.IO;
    using System.Reflection;
    using System.Xml;
    using System.Xml.Linq;
    using Effort.Internal.Common;
    using Effort.Internal.StorageSchema;
    using Effort.Provider;
    using Effort.Test.Data.Northwind;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class StorageSchemaModifierFixture
    {
        [TestMethod]
        public void MetadataWorkspaceHelperRewrite()
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

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        public void RewriteSsdlV2()
        {
            XElement ssdl = LoadXml("Effort.Test.Environment.Resources.StorageSchemas.SSDLv2.xml");

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        [Ignore]
        public void RewriteSsdlV3()
        {
            XElement ssdl = LoadXml("Effort.Test.Environment.Resources.StorageSchemas.SSDLv3.xml");

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            // FAILS: Requires .NET 4.5
            ValidateSsdl(ssdl); 
        }

        private static XElement LoadXml(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
            return XDocument.Load(stream).Root;
        }

        private static void ValidateSsdl(XElement ssdl)
        {
            new StoreItemCollection(new XmlReader[] { ssdl.CreateReader() });
        }
    }
}
