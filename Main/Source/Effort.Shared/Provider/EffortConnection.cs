// --------------------------------------------------------------------------------------------
// <copyright file="EffortConnection.cs" company="Effort Team">
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


using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using Effort.Internal.CommandActions; 
using Effort.Internal.DbManagement.Schema;
using Effort.Shared.Provider;

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Effort.DataLoaders;
    using Effort.Internal.Caching;
    using Effort.Internal.DbManagement;

    /// <summary>
    ///     Represents a virtual connection towards an in-memory fake database.
    /// </summary>
    public class EffortConnection : DbConnection
    {
        private string connectionString;

        private string lastContainerId;
        private DbContainerManagerWrapper containerConfiguration;
        private DbContainer container;

        private Guid identifier;
        private ConnectionState state;
        private bool isPrimaryTransient;
        public Snapshot Snapshot;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortConnection" /> class.
        /// </summary>
        public EffortConnection()
        {
            this.identifier = Guid.NewGuid();
            this.state = ConnectionState.Closed;
        }
        
        public bool IsCaseSensitive
        {
            get
            {
                if (this.DbContainer != null)
                {
                    return this.DbContainer.IsCaseSensitive;
                }
                else
                {
                    throw new Exception("The connection must be open to gets or sets 'IsCaseSensitive' value. Please open the connection first with 'effortConnection.Open()'");
                }
            }
            set
            {
                if (this.DbContainer != null)
                {
                    this.DbContainer.IsCaseSensitive = value;
                }
                else
                {
                    throw new Exception("The connection must be open to gets or sets 'IsCaseSensitive' value. Please open the connection first with 'effortConnection.Open()'");
                }
            }
        }

#if !EFOLD 
        /// <summary>
        ///   NEED TEXT ==> return DbTableInfo
        /// </summary> 
        public DbTableInfo GetEffortProperties(string schema, string name)
        {
            DbTableInfo TableInfo = null;

            if (this.DbContainer != null)
            {
                var table = this.DbContainer.GetTable(new TableName(schema, name));

                var _TableInfo = table.GetType().GetProperty("TableInfo",
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy);

                if (_TableInfo != null)
                {
                    TableInfo = (DbTableInfo)_TableInfo.GetValue(table, BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.FlattenHierarchy, null, null, null);
                }
            }

            return TableInfo;
        }
	
	
         /// <summary>
        ///    Take Snapshot about Table on Effort.
        /// </summary>
        public void TakeSnapshot()
        {
            this.Snapshot = new Snapshot();

            if (this.DbContainer != null)
            {
                ActionContext actionContext = new ActionContext(this.DbContainer);

                var tables = DbCommandActionHelper.GetAllTables(actionContext.DbContainer).ToList()
                    .Where(x => !x.EntityType.Name.Contains("_____MigrationHistory")).ToList();

                foreach (var table in tables)
                {
                    foreach (var index in table.Indexes)
                    {
                        var dynamicIndex = (dynamic) index;
                        var uniqueDataStructure = dynamicIndex.uniqueDataStructure;
                        var entities = uniqueDataStructure.inner.Values;
                        var list = new List<object>();

                        foreach (var entity in entities)
                        {
                            list.Add(entity);
                        }
                         
                        // TODO : when many index that take number of index * entity != good ==> that throw Error, because Key is Table.
                        this.Snapshot.AllIndex.Add(table, list);
                    }
                }
            }
        }
		
		/// <summary>
        ///    Use Snapshot about Table on Effort.
        /// </summary>
        public bool UseSnapshot()
        {
            if (this.Snapshot != null)
            {
                // note : that use the seed IdentityField normal logic.
                this.Snapshot.UseSnapshot();

                return true;
            }

            return false;
        }
        
                
        /// <summary>
        ///     Clear all tables from the effort connection. You must use a new context instance to clear all tracked entities, otherwise, use the ClearTables(DbContext) overload.
        /// </summary>
        public void ClearTables(bool takeSnapshot = false)
        {
            ClearTables(null, takeSnapshot);
        }

        /// <summary>
        ///     Clear all tables from the effort connection and ChangeTracker entries.
        /// </summary>
        public void ClearTables(DbContext context, bool takeSnapshot = false)
        {
            if (takeSnapshot)
            {
                TakeSnapshot();
            }
            
            if (this.DbContainer != null)
            {
                ActionContext actionContext = new ActionContext(this.DbContainer);

                var tables = DbCommandActionHelper.GetAllTables(actionContext.DbContainer).ToList().Where(x => !x.EntityType.Name.Contains("_____MigrationHistory")).ToList();

                foreach (var table in tables)
                {
                    foreach (var index in table.Indexes)
                    {
                        index.Clear();
                    }

                    var _RestoreIdentityField = table.GetType().GetMethod("RestoreIdentityField",
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                        BindingFlags.FlattenHierarchy);


                    if (_RestoreIdentityField != null)
                    {
                        _RestoreIdentityField.Invoke(table,
                            BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static |
                            BindingFlags.FlattenHierarchy, null, null, null);
                    }
                }

                if (context != null)
                {
                    var changedEntriesCopy = context.ChangeTracker.Entries().ToList();
                    changedEntriesCopy.ForEach(x => x.State = EntityState.Detached);
                }
            }
        }
        
#endif

        /// <summary>
        ///     Gets or sets the string used to open the connection.
        /// </summary>
        /// <returns>
        ///     The connection string used to establish the initial connection. The exact
        ///     contents of the connection string depend on the specific data source for this
        ///     connection. The default value is an empty string.
        /// </returns>
        public override string ConnectionString
        {
            get
            {
                return this.connectionString;
            }

            set
            {
                var builder = new EffortConnectionStringBuilder(value);

                // Read the transient information now, because it is removed in the setter
                // This is required because EF clones the connection string and these should
                // not receive the IsTransient flag
                if (builder.IsTransient)
                {
                    this.isPrimaryTransient = builder.IsTransient;	 
                }

                // Remove informations that should not inherit
                builder.Normalize();

                this.connectionString = builder.ConnectionString;
            }
        }

        /// <summary>
        ///     Gets the name of the database server to which to connect.
        /// </summary>
        /// <returns>
        ///     The name of the database server to which to connect. The default value is an
        ///     empty string.
        /// </returns>
        public override string DataSource
        {
            get
            {
                return "in-process";
            }
        }

        /// <summary>
        ///     Gets a string that represents the version of the server to which the object is 
        ///     connected.
        /// </summary>
        /// <returns>
        ///     The version of the database. The format of the string returned depends on the 
        ///     specific type of connection you are using.
        /// </returns>
        public override string ServerVersion
        {
            get
            {
                return typeof(NMemory.Database).Assembly.GetName().Version.ToString();
            }
        }

        /// <summary>
        ///     Gets a string that describes the state of the connection.
        /// </summary>
        /// <returns>
        ///     The state of the connection. The format of the string returned depends on the 
        ///     specific type of connection you are using.
        /// </returns>
        public override ConnectionState State
        {
            get
            {
                return this.state;
            }
        }

        /// <summary>
        ///     Gets the internal <see cref="DbContainer" /> instance.
        /// </summary>
        /// <value>
        ///     The internal <see cref="DbContainer" /> instance.
        /// </value>
        internal DbContainer DbContainer
        {
            get
            {
                return this.container;
            }
        }

        /// <summary>
        ///     Gets the <see cref="T:System.Data.Common.DbProviderFactory" /> for this 
        ///     <see cref="T:System.Data.Common.DbConnection" />.
        /// </summary>
        /// <returns> 
        ///     A <see cref="T:System.Data.Common.DbProviderFactory" />.
        /// </returns>
        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return EffortProviderFactory.Instance;
            }
        }

        /// <summary>
        ///     Changes the current database for an open connection.
        /// </summary>
        /// <param name="databaseName">
        ///     Specifies the name of the database for the connection to use.
        /// </param>
        public override void ChangeDatabase(string databaseName)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets the name of the current database after a connection is opened, or the 
        ///     database name specified in the connection string before the connection is 
        ///     opened.
        /// </summary>
        /// <returns>
        ///     The name of the current database or the name of the database to be used after a 
        ///     connection is opened. The default value is an empty string.
        /// </returns>
        public override string Database
        {
            get 
            {
                EffortConnectionStringBuilder connectionString = 
                    new EffortConnectionStringBuilder(this.ConnectionString);

                return connectionString.InstanceId;
            }
        }

        /// <summary>
        ///     Gets the configuration object that allows to alter the current configuration
        ///     of the database.
        /// </summary>
        /// <returns>
        ///     The configuration object.
        /// </returns>
        public IDbManager DbManager
        {
            get
            {
                if (this.State != ConnectionState.Open)
                {
                    throw new InvalidOperationException();
                }

                return this.containerConfiguration;
            }
        }

        /// <summary>
        ///     Opens a database connection with the settings specified by the 
        ///     <see cref="P:System.Data.Common.DbConnection.ConnectionString" />.
        /// </summary>
        public override void Open()
        {
            EffortConnectionStringBuilder connectionString = 
                new EffortConnectionStringBuilder(this.ConnectionString);

            string instanceId = connectionString.InstanceId;

            if (this.lastContainerId == instanceId)
            {
                // The id was not changed, so the appropriate container is associated
                this.ChangeConnectionState(ConnectionState.Open);
                return;
            }

            this.container = 
                DbContainerStore.GetDbContainer(instanceId, this.CreateDbContainer);

            this.containerConfiguration = new DbContainerManagerWrapper(this.container);

            this.lastContainerId = instanceId;
            this.ChangeConnectionState(ConnectionState.Open);
        }

        /// <summary>
        ///     Closes the connection to the database. This is the preferred method of closing
        ///     any open connection.
        /// </summary>
        public override void Close()
        {
            this.ChangeConnectionState(ConnectionState.Closed);
        }

        /// <summary>
        ///     Marks the connection object as transient, so the underlying database instance
        ///     will be disposed when this connection object is disposed or garbage collected.
        /// </summary>
        internal void MarkAsPrimaryTransient()
        {
            this.isPrimaryTransient = true;
        }

        /// <summary>
        ///     Creates and returns a <see cref="T:System.Data.Common.DbCommand" /> object 
        ///     associated with the current connection.
        /// </summary>
        /// <returns>
        ///     A <see cref="T:System.Data.Common.DbCommand" /> object.
        /// </returns>
        protected override DbCommand CreateDbCommand()
        {
            return new EffortCommand() { Connection = this };
        }

        /// <summary>
        ///     Starts a database transaction.
        /// </summary>
        /// <param name="isolationLevel">
        ///     Specifies the isolation level for the transaction.
        /// </param>
        /// <returns>
        ///     An object representing the new transaction.
        /// </returns>
        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new EffortTransaction(this, isolationLevel);
        }

        /// <summary>
        ///     Enlists in the specified transaction.
        /// </summary>
        /// <param name="transaction">
        ///     A reference to an existing <see cref="T:System.Transactions.Transaction" /> in
        ///     which to enlist.
        /// </param>
        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
        }

        /// <summary>
        ///     Releases the unmanaged resources used by the 
        ///     <see cref="T:System.ComponentModel.Component" /> and optionally releases the 
        ///     managed resources.
        /// </summary>
        /// <param name="disposing">
        ///     true to release both managed and unmanaged resources; false to release only 
        ///     unmanaged resources.
        /// </param>
        protected override void Dispose(bool disposing)
        {
            if (this.isPrimaryTransient)
            {
                this.UnregisterContainer();
            }

            base.Dispose(disposing);
        }

        internal virtual void UnregisterContainer()
        {
            var builder = new EffortConnectionStringBuilder(this.ConnectionString);

            DbContainerStore.RemoveDbContainer(builder.InstanceId);
        }

        private void ChangeConnectionState(ConnectionState state)
        {
            ConnectionState oldState = this.state;

            if (oldState != state)
            {
                this.state = state;

                this.OnStateChange(
                    new StateChangeEventArgs(oldState, this.state));
            }
        }

        private DbContainer CreateDbContainer()
        {
            EffortConnectionStringBuilder connectionString =
                new EffortConnectionStringBuilder(this.ConnectionString);

            IDataLoader dataLoader = null;
            DbContainerParameters parameters = new DbContainerParameters();
            Type dataLoaderType = connectionString.DataLoaderType;

            if (dataLoaderType != null)
            {
                //// TODO: check parameterless constructor

                dataLoader = Activator.CreateInstance(dataLoaderType) as IDataLoader;
                dataLoader.Argument = connectionString.DataLoaderArgument;

                parameters.DataLoader = dataLoader;
            }

            parameters.IsTransient = this.isPrimaryTransient;

            return new DbContainer(parameters);
        }
    }
}
