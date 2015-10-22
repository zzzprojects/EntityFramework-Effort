// --------------------------------------------------------------------------------------------
// <copyright file="EntityConnectionFactory.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

namespace Effort
{
    using System;
    using System.Configuration;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.EntityClient;
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Data.Entity.Core.Objects;
#else
    using System.Data.EntityClient;
    using System.Data.Metadata.Edm;
    using System.Data.Objects;
    using System.Reflection;
#endif
    using Effort.DataLoaders;
    using Effort.Internal.Caching;
    using Effort.Internal.Common;
    using Effort.Provider;

    /// <summary>
    ///     Provides factory methods that are able to create <see cref="T:EntityConnection"/>
    ///     objects that rely on in-process and in-memory databases. All of the data operations 
    ///     initiated from these connection objects are executed by the appropriate in-memory 
    ///     database, so using these connection objects does not require any external 
    ///     dependency outside of the scope of the application.
    /// </summary>
    public static class EntityConnectionFactory
    {
        /// <summary>
        ///     Initializes static members of the <see cref="EntityConnectionFactory" /> class.
        /// </summary>
        static EntityConnectionFactory()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        #region Persistent

        /// <summary>
        ///     Creates a <see cref="T:EntityConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the complete application lifecycle. If the 
        ///     database is accessed the first time, then it will be constructed based on the 
        ///     metadata referenced by the provided entity connection string and its state is 
        ///     initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:EntityConnection"/> object.
        /// </returns>
        public static EntityConnection CreatePersistent(
            string entityConnectionString, 
            IDataLoader dataLoader)
        {
            MetadataWorkspace metadata = GetEffortCompatibleMetadataWorkspace(ref entityConnectionString);

            DbConnection connection = 
                DbConnectionFactory.CreatePersistent(entityConnectionString, dataLoader);

            return CreateEntityConnection(metadata, connection);
        }

        /// <summary>
        ///     Creates a <see cref="T:EntityConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the complete application lifecycle. If the 
        ///     database is accessed the first time, then it will be constructed based on the 
        ///     metadata referenced by the provided entity connection string.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and references
        ///     the metadata that is required for constructing the schema.
        /// </param>
        /// <returns>
        ///     The <see cref="T:SEntityConnection"/> object.
        /// </returns>
        public static EntityConnection CreatePersistent(
            string entityConnectionString)
        {
            return CreatePersistent(entityConnectionString, null);
        }

        #endregion

        #region Transient

        /// <summary>
        ///     Creates a <see cref="T:EntityConnection"/> object that rely on an in-memory
        ///     database instance that lives during the connection object lifecycle. If the 
        ///     connection object is disposed or garbage collected, then underlying database
        ///     will be garbage collected too. The database is constructed based on the 
        ///     metadata referenced by the provided entity connection string and its state is
        ///     initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the metadata that is required for 
        ///     constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:EntityConnection"/> object.
        /// </returns>
        public static EntityConnection CreateTransient(
            string entityConnectionString, 
            IDataLoader dataLoader)
        {
            MetadataWorkspace metadata = 
                GetEffortCompatibleMetadataWorkspace(ref entityConnectionString);

            DbConnection connection = DbConnectionFactory.CreateTransient(dataLoader);

            return CreateEntityConnection(metadata, connection);
        }

        /// <summary>
        ///     Creates a <see cref="T:EntityConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the connection object lifecycle. If the 
        ///     connection object is disposed or garbage collected, then underlying database 
        ///     will be garbage collected too. The database is constructed based on the 
        ///     metadata referenced by the provided entity connection string.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the metadata that is required for 
        ///     constructing the schema.
        /// </param>
        /// <returns>
        ///     The <see cref="T:DbConnection"/> object.
        /// </returns>
        public static EntityConnection CreateTransient(string entityConnectionString)
        {
            return CreateTransient(entityConnectionString, null);
        }

        #endregion

        /// <summary>
        ///     Creates a new EntityConnection instance that wraps an EffortConnection object
        ///     with the specified connection string.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the metadata and identifies the 
        ///     persistent database.
        /// </param>
        /// <param name="effortConnectionString">
        ///     The effort connection string that is passed to the EffortConnection object.
        /// </param>
        /// <param name="persistent">
        ///     if set to <c>true</c> the ObjectContext uses a persistent database, otherwise 
        ///     transient.
        /// </param>
        /// <returns>
        ///     The EntityConnection object.
        /// </returns>
        public static EntityConnection Create(
            string entityConnectionString, 
            string effortConnectionString, 
            bool persistent)
        {
            MetadataWorkspace metadata = 
                GetEffortCompatibleMetadataWorkspace(ref entityConnectionString);

            EffortConnectionStringBuilder ecsb = 
                new EffortConnectionStringBuilder(effortConnectionString);

            if (persistent)
            {
                ecsb.InstanceId = entityConnectionString;
            }
            else
            {
                ecsb.InstanceId = Guid.NewGuid().ToString();
            }

            EffortConnection connection = 
                new EffortConnection() { ConnectionString = ecsb.ConnectionString };

            if (!persistent)
            {
                connection.MarkAsPrimaryTransient();
            }

            return CreateEntityConnection(metadata, connection);
        }

        /// <summary>
        ///     Returns the full entity connection string if it formed as 
        ///     "name=connectionStringName".
        /// </summary>
        /// <param name="entityConnectionString"> The entity connection string. </param>
        /// <returns> The full entity connection string. </returns>
        private static string GetFullEntityConnectionString(
            string entityConnectionString)
        {
            EntityConnectionStringBuilder builder = 
                new EntityConnectionStringBuilder(entityConnectionString);

            if (!string.IsNullOrWhiteSpace(builder.Name))
            {
                string connectionStringName = builder.Name;

                ConnectionStringSettings setting = 
                    ConfigurationManager.ConnectionStrings[connectionStringName];

                if (setting == null)
                {
                    throw new ArgumentException(
                        "Connectionstring was not found", 
                        "entityConnectionString");
                }

                entityConnectionString = setting.ConnectionString;
            }

            return entityConnectionString;
        }

        /// <summary>
        ///     Creates a new EntityConnection object and initializes its underlying database.
        /// </summary>
        /// <param name="metadata"> The metadata of the database. </param>
        /// <param name="connection"> The wrapped connection object. </param>
        /// <returns> The EntityConnection object. </returns>
        private static EntityConnection CreateEntityConnection(
            MetadataWorkspace metadata, 
            DbConnection connection)
        {

#if !EFOLD
            EntityConnection entityConnection = 
                new EntityConnection(metadata, connection, true);
#else
            EntityConnection entityConnection = 
                new EntityConnection(metadata, connection);

            FieldInfo owned = 
                typeof(EntityConnection)
                .GetField(
                    "_userOwnsStoreConnection", 
                    BindingFlags.Instance | BindingFlags.NonPublic);

            owned.SetValue(entityConnection, false);
#endif

            using (ObjectContext objectContext = new ObjectContext(entityConnection))
            {
                if (!objectContext.DatabaseExists())
                {
                    objectContext.CreateDatabase();
                }
            }

            return entityConnection;
        }

        /// <summary>
        ///     Returns a metadata workspace that is rewritten in order to be compatible the
        ///     Effort provider.
        /// </summary>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the original metadata.
        /// </param>
        /// <returns>
        ///     The rewritten metadata.
        /// </returns>
        private static MetadataWorkspace GetEffortCompatibleMetadataWorkspace(
            ref string entityConnectionString)
        {
            EffortProviderConfiguration.VerifyProvider();

            entityConnectionString = GetFullEntityConnectionString(entityConnectionString);

            EntityConnectionStringBuilder connectionStringBuilder = 
                new EntityConnectionStringBuilder(entityConnectionString);

            return MetadataWorkspaceStore.GetMetadataWorkspace(
                connectionStringBuilder.Metadata,
                metadata => MetadataWorkspaceHelper.Rewrite(
                    metadata, 
                    EffortProviderConfiguration.ProviderInvariantName, 
                    EffortProviderManifestTokens.Version1));
        }
    }
}
