
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
    using Effort.Provider;
    using EFProviderWrapperToolkit;
    using Effort.Test.Environment.ResultSets;
    using Effort.Test.Environment.DataReaderInspector;
    using Effort.Internal.StorageSchema;

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
                XAttribute xProvider = ssdlFile.Attribute("Provider");
                XAttribute xProviderManifestToken = ssdlFile.Attribute("ProviderManifestToken");

                if (createFake)
                {
                    UniversalStorageSchemaModifier.Instance.Modify(ssdlFile, new EffortProviderInformation());
                }

                string oldProviderInvariantName = xProvider.Value;
                string oldProviderManifestToken = xProviderManifestToken.Value;

                xProvider.Value = DataReaderInspectorProviderConfiguration.ProviderInvariantName;
                xProviderManifestToken.Value = string.Format("{0};{1}", oldProviderInvariantName, oldProviderManifestToken);
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
