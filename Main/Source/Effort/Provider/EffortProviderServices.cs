#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Data.Common;
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using Effort.Internal.DbManagement;
using System.Data;
using Effort.Internal.Caching;

namespace Effort.Provider
{
    public class EffortProviderServices : DbProviderServices
    {
        public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            return base.CreateCommandDefinition(prototype);
        }

        protected override DbCommandDefinition CreateDbCommandDefinition(DbProviderManifest providerManifest, DbCommandTree commandTree)
        {
            EffortEntityCommand command = new EffortEntityCommand(commandTree);

            return new EffortCommandDefinition(command);
        }

        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            EffortVersion version = EffortProviderManifestTokens.GetVersion(manifestToken);

            return new EffortProviderManifest(version);
        }

        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            return EffortProviderManifestTokens.Version1;
        }

        protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            DbContainer container = GetDbContainer(connection);

            return container.IsInitialized(storeItemCollection);
        }

        protected override void DbCreateDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            DbContainer container = GetDbContainer(connection);

            if (!container.IsInitialized(storeItemCollection))
            {
                container.Initialize(storeItemCollection);
            }
         
        }

        protected override void DbDeleteDatabase(DbConnection connection, int? commandTimeout, StoreItemCollection storeItemCollection)
        {
            DbContainer container = GetDbContainer(connection);

            // TODO: Delete database

            base.DbDeleteDatabase(connection, commandTimeout, storeItemCollection);
        }

        protected override string DbCreateDatabaseScript(string providerManifestToken, StoreItemCollection storeItemCollection)
        {
            DbSchemaKey key = new DbSchemaKey(storeItemCollection);

            // Initialize schema
            DbSchemaStore.GetDbSchema(storeItemCollection, DbContainer.CreateDbSchema);

            return string.Format("CREATE SCHEMA ({0})", key);
        }

        private static DbContainer GetDbContainer(DbConnection connection)
        {
            EffortConnection effortConnection = connection as EffortConnection;

            if (effortConnection == null)
            {
                throw new ArgumentException("", "connection");
            }

            // Open if needed
            if (effortConnection.State == System.Data.ConnectionState.Closed)
            {
                effortConnection.Open();
            }

            return effortConnection.DbContainer;
        }



    }
}
