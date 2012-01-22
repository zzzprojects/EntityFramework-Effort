using System;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using Effort.Helpers;

namespace Effort.DataInitialization
{
    internal class DbDataSource : DataSourceBase
    {
        private EntityConnection connection;
        private MetadataWorkspace workspace;
        private EntitySet entitySet;

        public DbDataSource(Type entityType, EntityConnection connection, string tableName)
            : base(entityType)
        {
            this.connection = connection;

            this.workspace = connection.GetMetadataWorkspace();
            this.entitySet = MetadataWorkspaceHelper.GetEntityContainer(this.workspace).GetEntitySetByName(tableName, true);
        }

        protected override IDataReader CreateDataReader()
        {
            // Build a command tree, which queries all records
            var commandTree = CommandTreeBuilder.CreateSelectAll(this.workspace, this.entitySet);
            // Get the provider services of the wrapped connection
            var providerServices = DbProviderServices.GetProviderServices(connection.StoreConnection);

            // Create a command definition from the command tree
            var commandDefinition = providerServices.CreateCommandDefinition(
                providerServices.GetProviderManifest(providerServices.GetProviderManifestToken(connection.StoreConnection)),
                commandTree);


            // Compile command and execute
            DbCommand command = commandDefinition.CreateCommand();
            command.Connection = connection.StoreConnection;

            DbDataReader reader = command.ExecuteReader();

            return reader;
        }

        protected override object ConvertValue(object value, Type type)
        {
            if (value is DBNull)
            {
                return null;
            }

            return base.ConvertValue(value, type);
        }
    }
}
