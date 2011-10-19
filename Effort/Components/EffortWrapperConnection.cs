using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Data.EntityClient;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Effort.Caching;
using Effort.Configuration;
using Effort.DatabaseManagement;
using Effort.DataInitialization;
using Effort.DbCommandTreeTransform;
using Effort.Helpers;
using EFProviderWrapperToolkit;
using MMDB;
using MMDB.Exceptions;
using MMDB.Locking;
using MMDB.Logging;
using MMDB.Table;


namespace Effort.Components
{
    public class EffortWrapperConnection : DbConnectionWrapper
    {

        #region Private members

        private Database databaseCache;
        // Virtual connection state
        private ConnectionState connectionState;

        private string connectionString;
        private ProviderModes mode;

        public List<string> NonCached { get; set; }
        public List<string> Cached { get; set; }
        public bool DefaultCacheMode { get; set; }

        #endregion

        #region Ctor

        public EffortWrapperConnection() : this(ProviderModes.DatabaseAccelerator)
        {

        }

        public EffortWrapperConnection(ProviderModes mode)
        {
            this.mode = mode;
            var config = System.Configuration.ConfigurationManager.GetSection( "MMDBCacheConfiguration" ) as MMDBCacheConfigurationSection;
            if( config != null )
            {
                var tables = config.Tables.Cast<TableElement>();
                this.DefaultCacheMode = config.DefaultCacheMode;
                
                this.NonCached = tables.Where( t => t.Cached == false ).Select( t => t.Name ).ToList();
                this.Cached = tables.Where( t => t.Cached ).Select( t => t.Name ).ToList();
            }
            else
            {
                this.Cached = new List<string>();
                this.NonCached = new List<string>();
                this.DefaultCacheMode = true;
            }
        }

        #endregion


        #region Properties

        /// <summary>
        /// Gets the reference if the in-memory database that connection object is using.
        /// </summary>
        internal Database DatabaseCache
        {
            get
            {
                if (this.databaseCache == null)
                {
                    throw new InvalidOperationException("The database cache is not intilialized until the first open.");
                }

                return this.databaseCache;
            }
            private set 
            { 
                this.databaseCache = value; 
            }
        }

        /// <summary>
        /// Gets or sets the string used to open the connection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The connection string used to establish the initial connection. The exact contents of the connection string depend on the specific data source for this connection. The default value is an empty string.
        /// </returns>
        public override string ConnectionString
        {
            get
            {
                if (this.ProviderMode == ProviderModes.DatabaseEmulator)
                {
                    // Wrapped connection does not exist, use the stored connection string
                    return this.connectionString;
                }
                else
                {
                    return base.ConnectionString;
                }
            }
            set
            {
                if (this.ProviderMode == ProviderModes.DatabaseEmulator)
                {
                    this.connectionString = value;
                    // The emulator will not build the wrapped connections, so dont call the base function
                }
                else
                {
                    base.ConnectionString = value;

                    // The store connection can change connection string after open, so we store it
                    this.connectionString = this.WrappedConnection.ConnectionString;
                }
            }
        }

        /// <summary>
        /// Gets the time to wait while establishing a connection before terminating the attempt and generating an error.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The time (in seconds) to wait for a connection to open. The default value is determined by the specific type of connection that you are using.
        /// </returns>
        public override int ConnectionTimeout
        {
            get
            {
                if (this.ProviderMode == ProviderModes.DatabaseEmulator)
                {
                    // Wrapped provider does not exist, workaround:

                    // 15 is the default value
                    return 15;
                }

                return base.ConnectionTimeout;
            }
        }

        /// <summary>
        /// Gets a string that describes the state of the connection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The state of the connection. The format of the string returned depends on the specific type of connection you are using.
        /// </returns>
        public override ConnectionState State
        {
            get
            {
                if (this.DesignMode)
                {
                    return base.State;
                }

                return this.connectionState;
            }
        }

        /// <summary>
        /// Gets the mode of the wrapper connection.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The mode of the wrapper connection.
        /// </returns>
        public ProviderModes ProviderMode
        {
            get
            {
                return this.mode;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Opens a database connection with the settings specified by the <see cref="P:System.Data.Common.DbConnection.ConnectionString"/>.
        /// </summary>
        public override void Open()
        {
            if (this.DesignMode)
            {
                base.Open();
                return;
            }

            if (this.connectionString == null)
            {
                this.connectionString = this.WrappedConnection.ConnectionString;
            }

            if (this.databaseCache == null)
            {
                if (this.ProviderMode == ProviderModes.DatabaseEmulator &&
                    !ConnectionStringHelper.GetValue<bool>(this.connectionString, "Shared Data"))
                {
                    // Per connection
                    this.databaseCache = CreateDatabaseSandboxed();
                }
                else
                {
                    // Get a reference to the the database cache (or create if does not exist)
                    this.databaseCache = DbInstanceStore.GetDbInstance(this.connectionString, CreateDatabaseSandboxed);
                }

            }



            // Virtualize connection state
            this.connectionState = ConnectionState.Open;
        }

        /// <summary>
        /// Closes the connection to the database. This is the preferred method of closing any open connection.
        /// </summary>
        /// <exception cref="T:System.Data.Common.DbException">
        /// The connection-level error that occurred while opening the connection.
        /// </exception>
        public override void Close()
        {
            if (this.DesignMode)
            {
                base.Close();
                return;
            }

            // Virtualize connection state
            this.connectionState = ConnectionState.Closed;

            if (this.mode == ProviderModes.DatabaseAccelerator && this.WrappedConnection.State != ConnectionState.Closed)
            {
                this.WrappedConnection.Close();
            }
        }

        #endregion

        #region Overrided virtual methods

        /// <summary>
        /// Enlists in the specified transaction.
        /// </summary>
        /// <param name="transaction">A reference to an existing <see cref="T:System.Transactions.Transaction"/> in which to enlist.</param>
        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            // Transaction enlistment is delayed
            ////base.EnlistTransaction(transaction);
        }

        /// <summary>
        /// Starts a database transaction.
        /// </summary>
        /// <param name="isolationLevel">Specifies the isolation level for the transaction.</param>
        /// <returns>
        /// An object representing the new transaction.
        /// </returns>
        protected override DbTransaction BeginDbTransaction(System.Data.IsolationLevel isolationLevel)
        {
            if (this.DesignMode)
            {
                return base.BeginDbTransaction(isolationLevel);
            }

            return new EffortWrapperTransaction(this, isolationLevel);
        }


        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                if (this.DesignMode)
                {
                    return DbProviderFactories.GetFactory(this.WrappedProviderInvariantName);
                }

                switch (this.ProviderMode)
                {
                    case ProviderModes.DatabaseAccelerator:
                        return DatabaseAcceleratorProviderFactory.Instance;
                    case ProviderModes.DatabaseEmulator:
                        return DatabaseEmulatorProviderFactory.Instance;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        protected override string DefaultWrappedProviderName
        {
            get { throw new NotSupportedException(); }
        }

        #endregion

        #region Database creation

        private Database CreateDatabaseSandboxed()
        {
            Database database = null;
            Exception exception = null;

            // Create a sandbox thread for the initialization
            Thread thread = new Thread(() =>
                {
                    try
                    {
                        database = this.CreateDatabase();
                    }
                    catch (Exception ex)
                    {
                        exception = ex;
                    }
                });

            thread.Name = "MMDB_Init";
            thread.Start();
            thread.Join();

            if (exception != null)
            {
                throw new DataException("An unhandled exception occured during the database initialization", exception);
            }

            return database;
        }


        private Database CreateDatabase()
        {
            Stopwatch swDatabase = Stopwatch.StartNew();

            Database database = new Database(new ReaderWriterLockSlimFactory(), EnumDeadlockManagement.DeadlockDetection);
            database.LoggingPort = new Logger();

            var entityConnectionString = this.FindEntityConnectionString();

            // Get metadata paths
            string[] metadataFiles = entityConnectionString
                .Metadata
                .Split('|')
                .Select(p => p.Trim())
                .ToArray();

            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            // Parse metadata
            MetadataWorkspace workspace = new MetadataWorkspace(metadataFiles, assemblies);
            // Get entity container
            EntityContainer entityContainer = MetadataWorkspaceHelper.GetEntityContainer(workspace);

            // Get or create the schema
            DatabaseSchema schema = DbSchemaStore.GetDbSchema(metadataFiles, () => GenerateSchema(entityContainer));

            using (IDataSourceFactory sourceFactory = this.CreateDataSourceFactory(workspace))
            {
                foreach (string tableName in schema.GetTableNames())
                {
                    Stopwatch swTable = Stopwatch.StartNew();

                    // Get the schema info of the table
                    TableInformation tableInfo = schema.GetInformation(tableName);

                    // Initialize the source of the records of the table 
                    IDataSource source = sourceFactory.Create(tableName, tableInfo.EntityType);

                    // Initialize the table
                    IReflectionTable table = DatabaseReflectionHelper.CreateTable(database, tableInfo.EntityType, tableInfo.PrimaryKeys, source.GetInitialRecords());

                    swTable.Stop();
                    database.LoggingPort.Send(new StandardMMDBMessage("{1} loaded in {0:0.0} ms", swTable.Elapsed.TotalMilliseconds, tableName));
                }
            }


            database.LoggingPort.Send(new StandardMMDBMessage("Setting up assocations..."));

            foreach (AssociationSet associationSet in entityContainer.BaseEntitySets.OfType<AssociationSet>())
            {
                var constraints = associationSet.ElementType.ReferentialConstraints;

                if (constraints.Count != 1)
                {
                    continue;
                }

                ReferentialConstraint constraint = constraints[0];

                DatabaseReflectionHelper.CreateAssociation(database, constraint);
            }

            swDatabase.Stop();
            database.LoggingPort.Send(new StandardMMDBMessage("Database buildup finished in {0:0.0} ms", swDatabase.Elapsed.TotalMilliseconds));

            return database;
        }

        private IDataSourceFactory CreateDataSourceFactory(MetadataWorkspace workspace)
        {
            if (this.ProviderMode == ProviderModes.DatabaseEmulator)
            {
                DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
                builder.ConnectionString = this.ConnectionString;

                bool hasSource = builder.ContainsKey("Data Source") && !string.IsNullOrEmpty(builder["Data Source"] as string);

                if (!hasSource)
                {
                    return new EmptyDataSourceFactory();
                }

                return new CsvDataSourceFactory(this.ConnectionString);
            }

            // Build up the wrapped connection in accelerator mode
            if (this.ProviderMode == ProviderModes.DatabaseAccelerator)
            {
                return new DbDataSourceFactory(() => new EntityConnection(workspace, this.CreateNewWrappedConnection()));
            }

            throw new NotSupportedException("Current mode is not supported");
        }

        private DbConnection CreateNewWrappedConnection()
        {
            DbConnection con = DbProviderServices.GetProviderFactory(this.WrappedConnection).CreateConnection();
            con.ConnectionString = this.WrappedConnection.ConnectionString;

            return con;
        }

        private DatabaseSchema GenerateSchema(EntityContainer entityContainer)
        {
            DatabaseSchema schema = new DatabaseSchema();

            // Dynamic Library for MMDB
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("MMDB_DynamicEntityLib({0})", Guid.NewGuid())),
                    AssemblyBuilderAccess.Run);

            // Module for the entity types
            ModuleBuilder entityModule = assembly.DefineDynamicModule("Entities");
            // Initialize type converter
            EdmTypeConverter typeConverter = new EdmTypeConverter();

            foreach (var entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                TypeBuilder entityTypeBuilder = entityModule.DefineType(entitySet.Name, TypeAttributes.Public);

                List<PropertyInfo> primaryKeyFields = new List<PropertyInfo>();

                // Add properties as entity fields
                foreach (EdmProperty field in entitySet.ElementType.Properties)
                {
                    Type fieldClrType = typeConverter.Convert(field.TypeUsage);

                    PropertyBuilder prop = EmitHelper.AddProperty(entityTypeBuilder, field.Name, fieldClrType);

                    // Register primary key field
                    if (entitySet.ElementType.KeyMembers.Contains(field))
                    {
                        primaryKeyFields.Add(prop);
                    }
                }

                Type entityType = entityTypeBuilder.CreateType();

                schema.Register(entityType.Name, entityType, primaryKeyFields.ToArray());
            }

            return schema;
        }

        private EntityConnectionStringBuilder FindEntityConnectionString()
        {
            // First try to get the entity connectionstring from OUR storage
            {
                string connectionString = null;

                if (ConnectionStringStore.TryGet(this.connectionString, out connectionString))
                {
                    return new EntityConnectionStringBuilder(connectionString);
                }
            }

            List<EntityConnectionStringBuilder> connStrings = new List<EntityConnectionStringBuilder>();

            // Search for the identical connection string in the application settings
            foreach (ConnectionStringSettings item in ConfigurationManager.ConnectionStrings)
            {
                if (item.ProviderName == "System.Data.EntityClient")
                {
                    try
                    {
                        EntityConnectionStringBuilder entityConnection = new EntityConnectionStringBuilder(item.ConnectionString);

                        if (ConnectionStringHelper.HasIdenticalAttributes(entityConnection.ProviderConnectionString, this.connectionString))
                        {
                            connStrings.Add(entityConnection);
                        }
                    }
                    catch
                    {
                        // Cannot read Connection String
                        continue;
                    }
                }

            }

            if (connStrings.Count == 0)
            {
                throw new InvalidOperationException("No schema was found for the connection string");
            }

            if (connStrings.Count > 1)
            {
                throw new NotSupportedException("Multiple schemas were found for the connection string");
            }

            EntityConnectionStringBuilder result = connStrings.First();

            return result;
        }

        #endregion

    }
}
