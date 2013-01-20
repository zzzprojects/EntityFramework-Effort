// --------------------------------------------------------------------------------------------
// <copyright file="StorageSchemaModifierFixture.cs" company="Effort Team">
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
    using Effort.Test.Internal;
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
            XElement ssdl = LoadXml(1);

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        public void RewriteSsdlV2()
        {
            XElement ssdl = LoadXml(2);

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            ValidateSsdl(ssdl); 
        }

        [TestMethod]
        public void RewriteSsdlV3()
        {
            // SSDL v3 requires .NET45
            if (!PlatformDetector.IsNet45OrNewer)
            {
                return;
            }

            XElement ssdl = LoadXml(3);

            UniversalStorageSchemaModifier.Instance.Modify(ssdl, new EffortProviderInformation());

            ValidateSsdl(ssdl); 
        }


        private static XElement LoadXml(int version)
        {
            XElement result = null;

            result = LoadXml(GetResourceName(version, true));

            if (result != null)
            {
                return result;
            }

            result = LoadXml(GetResourceName(version, false));

            return result;
        }

        private static string GetResourceName(int version, bool includeFramework)
        {
            string framework = string.Empty;

            if (includeFramework)
            {
                if (PlatformDetector.IsNet45OrNewer)
                {
                    framework = ".net45";
                }
                else
                {
                    framework = ".net40";
                }
            }

            return string.Format(
                "Effort.Test.Internal.Resources.StorageSchemas.SSDLv{0}{1}.xml",
                version,
                framework);
        }

        private static XElement LoadXml(string resourceName)
        {
            Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);

            if (stream == null)
            {
                return null;
            }

            return XDocument.Load(stream).Root;
        }

        private static void ValidateSsdl(XElement ssdl)
        {
            new StoreItemCollection(new XmlReader[] { ssdl.CreateReader() });
        }
    }
}
