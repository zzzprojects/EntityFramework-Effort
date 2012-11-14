// ----------------------------------------------------------------------------------
// <copyright file="EntityConnectionHelper.cs" company="Effort Team">
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

namespace Effort.Test.Environment
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Xml.Linq;
    using Effort.DataLoaders;
    using Effort.Internal.Common;
    using Effort.Internal.StorageSchema;
    using Effort.Provider;
    using Effort.Test.Environment.DataReaderInspector;
    using Effort.Test.Environment.ResultSets;
    using EFProviderWrapperToolkit;

    internal static class EntityConnectionHelper
    {
        static EntityConnectionHelper()
        {
            DataReaderInspectorProviderConfiguration.RegisterProvider();
            EffortProviderConfiguration.RegisterProvider();
        }

        public static EntityConnection CreateInspectedFakeEntityConnection(string entityConnectionString, IResultSetComposer resultSetComposer, IDataLoader dataLoader)
        {
            return CreateInspectedFakeEntityConnection(entityConnectionString, resultSetComposer, true, dataLoader);
        }

        public static EntityConnection CreateInspectedEntityConnection(string entityConnectionString, IResultSetComposer resultSetComposer)
        {
            return CreateInspectedFakeEntityConnection(entityConnectionString, resultSetComposer, false, null);
        }

        private static EntityConnection CreateInspectedFakeEntityConnection(string entityConnectionString, IResultSetComposer resultSetComposer, bool createFake, IDataLoader dataLoader)
        {
            EntityConnectionStringBuilder connectionString = new EntityConnectionStringBuilder(entityConnectionString);

            if (!string.IsNullOrEmpty(connectionString.Name))
            {
                string resolvedConnectionString = ConfigurationManager.ConnectionStrings[connectionString.Name].ConnectionString;
                connectionString = new EntityConnectionStringBuilder(resolvedConnectionString);
            }

            List<XElement> csdl = new List<XElement>();
            List<XElement> ssdl = new List<XElement>();
            List<XElement> msl = new List<XElement>();

            MetadataWorkspaceHelper.ParseMetadata(connectionString.Metadata, csdl, ssdl, msl);

            foreach (XElement ssdlFile in ssdl)
            {
                XAttribute providerAttribute = ssdlFile.Attribute("Provider");
                XAttribute providerManifestTokenAttribute = ssdlFile.Attribute("ProviderManifestToken");

                if (createFake)
                {
                    UniversalStorageSchemaModifier.Instance.Modify(ssdlFile, new EffortProviderInformation());
                }

                string oldProviderInvariantName = providerAttribute.Value;
                string oldProviderManifestToken = providerManifestTokenAttribute.Value;

                providerAttribute.Value = DataReaderInspectorProviderConfiguration.ProviderInvariantName;
                providerManifestTokenAttribute.Value = string.Format("{0};{1}", oldProviderInvariantName, oldProviderManifestToken);
            }

            MetadataWorkspace convertedWorkspace = MetadataWorkspaceHelper.CreateMetadataWorkspace(csdl, ssdl, msl);

            DbConnection storeConnection = null;

            if (createFake)
            {
                storeConnection = Effort.DbConnectionFactory.CreateTransient(dataLoader);
            }
            else
            {
                storeConnection = ProviderHelper.CreateConnection(connectionString.Provider);
                storeConnection.ConnectionString = connectionString.ProviderConnectionString;
            }
            
            DbConnectionWrapper inspectorConnection = new DataReaderInspectorConnection(resultSetComposer);
            inspectorConnection.WrappedConnection = storeConnection;

            EntityConnection entityConnection = new EntityConnection(convertedWorkspace, inspectorConnection);

            if (createFake)
            {
                using (ObjectContext objectContext = new ObjectContext(entityConnection))
                {
                    if (!objectContext.DatabaseExists())
                    {
                        objectContext.CreateDatabase();
                    }
                }
            }

            return entityConnection;
        }
    }
}
