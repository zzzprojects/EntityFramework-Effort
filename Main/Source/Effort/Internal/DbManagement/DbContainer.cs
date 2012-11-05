// ----------------------------------------------------------------------------------
// <copyright file="DbContainer.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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

namespace Effort.Internal.DbManagement
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.DataLoaders;
    using Effort.Internal.Caching;
    using Effort.Internal.Common;
    using Effort.Internal.DbCommandTreeTransformation;
    using Effort.Internal.Diagnostics;
    using Effort.Internal.TypeConversion;
    using Effort.Internal.TypeGeneration;
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

        ~DbContainer()
        {
            Console.WriteLine("DbContainer destructor");
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
                if (!this.database.ContainsTable(entitySet.Name))
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

            DbSchema schema = DbSchemaStore.GetDbSchema(edmStoreSchema, DbSchemaFactory.CreateDbSchema);

            this.Initialize(schema);
        }

        public void Initialize(DbSchema schema)
        {
            // TODO: locking
            Stopwatch databaseCreationMeasure = Stopwatch.StartNew();

            if (this.database == null)
            {
                if (this.parameters.IsTransient)
                {
                    this.database = new Database(new TransientDatabaseComponentFactory());
                }
                else
                {
                    this.database = new Database();
                }
            }

            using (ITableDataLoaderFactory loaderFactory = this.CreateDataLoaderFactory())
            {
                foreach (DbTableInformation tableInfo in schema.Tables)
                {
                    Stopwatch tableCreationMeasure = Stopwatch.StartNew();

                    IEnumerable<object> initialData = this.GetInitialData(loaderFactory, tableInfo);

                    // Initialize the table
                    DatabaseReflectionHelper.CreateTable(
                        this.database,
                        tableInfo.EntityType,
                        (IKeyInfo)tableInfo.PrimaryKeyInfo,
                        tableInfo.IdentityField,
                        tableInfo.Constraints,
                        initialData);

                    tableCreationMeasure.Stop();

                    this.Logger.Write("{1} created in {0:0.0} ms", tableCreationMeasure.Elapsed.TotalMilliseconds, tableInfo.TableName);
                }
            }

            this.Logger.Write("Setting up assocations...");

            foreach (DbRelationInformation relation in schema.Relations)
            {
                DatabaseReflectionHelper.CreateAssociation(this.database, relation);
            }

            databaseCreationMeasure.Stop();
            this.Logger.Write("Database buildup finished in {0:0.0} ms", databaseCreationMeasure.Elapsed.TotalMilliseconds);
        }

        private IEnumerable<object> CreateInitialData(ITableDataLoaderFactory loaderFactory, DbTableInformation tableInfo)
        {
            return ObjectLoader.Load(loaderFactory, tableInfo.TableName, tableInfo.EntityType);
        }

        private ITableDataLoaderFactory CreateDataLoaderFactory()
        {
            if (this.parameters.DataLoader == null)
            {
                return new EmptyTableDataLoaderFactory();
            }

            return this.parameters.DataLoader.CreateTableDataLoaderFactory();
        }

        private IEnumerable<object> GetInitialData(ITableDataLoaderFactory loaderFactory, DbTableInformation tableInfo)
        {
            if (this.parameters.DataLoader != null && !this.parameters.IsDataLoaderCached)
            {
                DbTableInitialData initialData =
                    TableInitialDataStore.GetDbInitialData(
                        this.parameters.DataLoader,
                        tableInfo.EntityType,
                        () => new DbTableInitialData(this.CreateInitialData(loaderFactory, tableInfo)));

                return initialData.GetClonedInitialData();
            }
            else
            {
                return this.CreateInitialData(loaderFactory, tableInfo);
            }
        }
    }
}
