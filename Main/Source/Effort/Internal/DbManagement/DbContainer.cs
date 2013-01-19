// --------------------------------------------------------------------------------------------
// <copyright file="DbContainer.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
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

namespace Effort.Internal.DbManagement
{
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using Effort.DataLoaders;
    using Effort.Internal.Caching;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
    using Effort.Internal.DbManagement.Engine;
    using Effort.Internal.DbManagement.Schema;
    using Effort.Internal.Diagnostics;
    using Effort.Internal.TypeConversion;
    using NMemory;
    using NMemory.Indexes;
    using NMemory.Modularity;
    using NMemory.StoredProcedures;
    using NMemory.Tables;

    internal class DbContainer : ITableProvider
    {
        private Database database;
        private ITypeConverter converter;
        private DbContainerParameters parameters;

        private ILogger logger;
        private ConcurrentDictionary<string, IStoredProcedure> transformCache;

        public DbContainer(DbContainerParameters parameters)
        {
            this.parameters = parameters;

            this.logger = new Logger();
            this.transformCache = new ConcurrentDictionary<string, IStoredProcedure>();
            this.converter = new DefaultTypeConverter();
        }

        public Database Internal
        {
            get { return this.database; }
        }

        public ILogger Logger
        {
            get { return this.logger; }
        }

        public ConcurrentDictionary<string, IStoredProcedure> TransformCache
        {
            get { return this.transformCache; }
        }

        public ITypeConverter TypeConverter
        {
            get { return this.converter; }
        }

        public object GetTable(string name)
        {
            return this.database.GetTable(name);
        }

        public bool IsInitialized(StoreItemCollection edmStoreSchema)
        {
            // TODO: Lock
            if (this.database == null)
            {
                return false;
            }

            // Find container
            EntityContainer entityContainer = edmStoreSchema.GetItems<EntityContainer>().FirstOrDefault();

            foreach (EntitySet entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                // TODO: Verify fields
                if (!this.database.ContainsTable(entitySet.GetTableName()))
                {
                    return false;
                }
            }

            return true;
        }

        public void Initialize(StoreItemCollection edmStoreSchema)
        {
            if (this.IsInitialized(edmStoreSchema))
            {
                return;
            }

            DbSchema schema = 
                DbSchemaStore.GetDbSchema(
                    edmStoreSchema, 
                    sic => DbSchemaFactory.CreateDbSchema(sic));

            this.Initialize(schema);
        }

        public void Initialize(DbSchema schema)
        {
            // TODO: locking
            Stopwatch fullTime = Stopwatch.StartNew();
            Stopwatch partialTime = Stopwatch.StartNew();

            this.Logger.Write("Database creation started...");

            this.EnsureInitializedDatabase();

            // Temporary dictionary
            Dictionary<string, ITable> tables = new Dictionary<string,ITable>();

            this.Logger.Write("Creating tables...");
            partialTime.Restart();

            // Create tables
            foreach (DbTableInformation tableInfo in schema.Tables)
            {
                ITable table = DatabaseReflectionHelper.CreateTable(
                    this.database,
                    tableInfo.EntityType,
                    (IKeyInfo)tableInfo.PrimaryKeyInfo,
                    tableInfo.IdentityField,
                    tableInfo.Constraints);

                tables.Add(tableInfo.TableName, table);
            }

            this.Logger.Write(
                "Tables created in {0:0.0} ms", 
                partialTime.Elapsed.TotalMilliseconds);

            this.Logger.Write("Adding initial data...");
            partialTime.Restart();
            
            // Add initial data to the tables
            using (ITableDataLoaderFactory loaderFactory = this.CreateDataLoaderFactory())
            {
                foreach (DbTableInformation tableInfo in schema.Tables)
                { 
                    // Get the table reference from the temporary dictionary
                    ITable table = tables[tableInfo.TableName];

                    // Return initial entity data and materialize them
                    IEnumerable<object> data = ObjectLoader.Load(loaderFactory, tableInfo);

                    DatabaseReflectionHelper.InitializeTableData(table, data);
                }
            }

            this.Logger.Write(
                "Initial data added in {0:0.0} ms",
                partialTime.Elapsed.TotalMilliseconds);

            this.Logger.Write("Creating and verifying associations...");
            partialTime.Restart();

            foreach (DbRelationInformation relation in schema.Relations)
            {
                DatabaseReflectionHelper.CreateAssociation(this.database, relation);
            }

            this.Logger.Write(
               "Associations created and verfied in {0:0.0} ms",
               partialTime.Elapsed.TotalMilliseconds);

            this.Logger.Write(
                "Database creation finished in {0:0.0} ms", 
                fullTime.Elapsed.TotalMilliseconds);
        }

        private void EnsureInitializedDatabase()
        {
            if (this.database == null)
            {
                IDatabaseComponentFactory componentFactory = null;

                if (this.parameters.IsTransient)
                {
                    componentFactory = new TransientDatabaseComponentFactory();
                }
                else
                {
                    componentFactory = new DatabaseComponentFactory();
                }

                this.database = new Database(componentFactory);
            }
        }

        private ITableDataLoaderFactory CreateDataLoaderFactory()
        {
            if (this.parameters.DataLoader == null)
            {
                return new EmptyTableDataLoaderFactory();
            }

            return this.parameters.DataLoader.CreateTableDataLoaderFactory();
        }
    }
}
