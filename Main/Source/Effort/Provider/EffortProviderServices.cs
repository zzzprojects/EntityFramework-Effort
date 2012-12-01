// --------------------------------------------------------------------------------------------
// <copyright file="EffortProviderServices.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Provider
{
    using System;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;
    using System.Data.Metadata.Edm;
    using Effort.Internal.Caching;
    using Effort.Internal.DbManagement;
    using Effort.Internal.DbManagement.Schema;

    /// <summary>
    ///     The factory for building command definitions; use the type of this object as the 
    ///     argument to the IServiceProvider.GetService method on the provider factory; 
    /// </summary>
    public class EffortProviderServices : DbProviderServices
    {
        /// <summary>
        ///     Creates a <see cref="T:System.Data.Common.DbCommandDefinition" /> that uses the 
        ///     specified <see cref="T:System.Data.Common.DbCommand" />.
        /// </summary>
        /// <param name="prototype">
        ///     A <see cref="T:System.Data.Common.DbCommand" /> used to create the 
        ///     <see cref="T:System.Data.Common.DbCommandDefinition" />.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Data.Common.DbCommandDefinition" /> object that
        ///     represents the executable command definition object.
        /// </returns>
        public override DbCommandDefinition CreateCommandDefinition(DbCommand prototype)
        {
            return base.CreateCommandDefinition(prototype);
        }

        /// <summary>
        ///     Creates a command definition object for the specified provider manifest and 
        ///     command tree.
        /// </summary>
        /// <param name="providerManifest">
        ///     Provider manifest previously retrieved from the store provider.
        /// </param>
        /// <param name="commandTree">
        ///     Command tree for the statement.
        /// </param>
        /// <returns>
        ///     An executable command definition object.
        /// </returns>
        protected override DbCommandDefinition CreateDbCommandDefinition(
            DbProviderManifest providerManifest, 
            DbCommandTree commandTree)
        {
            EffortEntityCommand command = new EffortEntityCommand(commandTree);

            return new EffortCommandDefinition(command);
        }

        /// <summary>
        ///     When overridden in a derived class, returns an instance of a class that derives 
        ///     from the <see cref="T:System.Data.Common.DbProviderManifest" />.
        /// </summary>
        /// <param name="manifestToken">
        ///     The token information associated with the provider manifest.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Data.Common.DbProviderManifest" /> object that represents
        ///     the provider manifest.
        /// </returns>
        protected override DbProviderManifest GetDbProviderManifest(string manifestToken)
        {
            EffortVersion version = EffortProviderManifestTokens.GetVersion(manifestToken);

            return new EffortProviderManifest(version);
        }

        /// <summary>
        ///     Returns provider manifest token given a connection.
        /// </summary>
        /// <param name="connection">
        ///     Connection to provider.
        /// </param>
        /// <returns>
        ///     The provider manifest token for the specified connection.
        /// </returns>
        protected override string GetDbProviderManifestToken(DbConnection connection)
        {
            return EffortProviderManifestTokens.Version1;
        }

        /// <summary>
        ///     Returns a value indicating whether a given database exists on the server and 
        ///     whether schema objects contained in the storeItemCollection have been created.
        /// </summary>
        /// <param name="connection">
        ///     Connection to a database whose existence is verified by this method.
        /// </param>
        /// <param name="commandTimeout">
        ///     Execution timeout for any commands needed to determine the existence of the 
        ///     database.
        /// </param>
        /// <param name="storeItemCollection">
        ///     The structure of the database whose existence is determined by this method.
        /// </param>
        /// <returns>
        ///     true if the database indicated by the connection and the 
        ///     <paramref name="storeItemCollection" /> parameter exists.
        /// </returns>
        protected override bool DbDatabaseExists(
            DbConnection connection, 
            int? commandTimeout, 
            StoreItemCollection storeItemCollection)
        {
            DbContainer container = GetDbContainer(connection);

            return container.IsInitialized(storeItemCollection);
        }

        /// <summary>
        ///     Creates a database indicated by connection and creates schema objects (tables, 
        ///     primary keys, foreign keys) based on the contents of a 
        ///     <see cref="T:System.Data.Metadata.Edm.StoreItemCollection" />.
        /// </summary>
        /// <param name="connection">
        ///     Connection to a non-existent database that needs to be created and populated 
        ///     with the store objects indicated with the storeItemCollection parameter.
        /// </param>
        /// <param name="commandTimeout">
        ///     Execution timeout for any commands needed to create the database.
        /// </param>
        /// <param name="storeItemCollection">
        ///     The collection of all store items based on which the script should be created.
        /// </param>
        protected override void DbCreateDatabase(
            DbConnection connection, 
            int? commandTimeout, 
            StoreItemCollection storeItemCollection)
        {
            DbContainer container = GetDbContainer(connection);

            if (!container.IsInitialized(storeItemCollection))
            {
                container.Initialize(storeItemCollection);
            }
        }

        /// <summary>
        ///     Deletes all store objects specified in the store item collection from the 
        ///     database and the database itself.
        /// </summary>
        /// <param name="connection">
        ///     Connection to an existing database that needs to be deleted.
        /// </param>
        /// <param name="commandTimeout">
        ///     Execution timeout for any commands needed to delete the database.
        /// </param>
        /// <param name="storeItemCollection">
        ///     The structure of the database to be deleted.
        /// </param>
        protected override void DbDeleteDatabase(
            DbConnection connection, 
            int? commandTimeout, 
            StoreItemCollection storeItemCollection)
        {
            EffortConnectionStringBuilder connectionString =new EffortConnectionStringBuilder(connection.ConnectionString);
            DbContainerStore.RemoveDbContainer(connectionString.InstanceId);
            connection.Dispose();
        }

        /// <summary>
        ///     Generates a data definition language (DDL0 script that creates schema objects 
        ///     (tables, primary keys, foreign keys) based on the contents of the 
        ///     <see cref="T:System.Data.Metadata.Edm.StoreItemCollection" /> parameter and 
        ///     targeted for the version of the database corresponding to the provider manifest 
        ///     token.
        /// </summary>
        /// <param name="providerManifestToken">
        ///     The provider manifest token identifying the target version.
        /// </param>
        /// <param name="storeItemCollection">
        ///     The structure of the database.
        /// </param>
        /// <returns>
        ///     A DDL script that creates schema objects based on the contents of the 
        ///     <see cref="T:System.Data.Metadata.Edm.StoreItemCollection" /> parameter and 
        ///     targeted for the version of the database corresponding to the provider manifest 
        ///     token.
        /// </returns>
        protected override string DbCreateDatabaseScript(
            string providerManifestToken, 
            StoreItemCollection storeItemCollection)
        {
            DbSchemaKey key = new DbSchemaKey(storeItemCollection);

            // Initialize schema
            DbSchemaStore.GetDbSchema(storeItemCollection, DbSchemaFactory.CreateDbSchema);

            return string.Format("CREATE SCHEMA ({0})", key);
        }

        /// <summary>
        ///     Returns the underlying DbContainer object of the specified DbConnection object. 
        /// </summary>
        /// <param name="connection">
        ///     The connection object.
        /// </param>
        /// <returns>
        ///     The DbContainer object.
        /// </returns>
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
