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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Effort.DataLoaders;
using Effort.Internal.Caching;
using Effort.Internal.Common;
using Effort.Internal.DbCommandTreeTransformation;
using Effort.Internal.Diagnostics;
using Effort.Internal.TypeConversion;
using NMemory;
using NMemory.StoredProcedures;

namespace Effort.Internal.DbManagement
{
    internal class DbContainer : ITableProvider
    {
        private Database database;
        private DbSchema schema;
        private ITypeConverter converter;
        private IDataLoader dataLoader;

        private ILogger logger;
        private ConcurrentDictionary<string, IStoredProcedure> transformCache;

        public DbContainer(IDataLoader dataLoader)
        {
            this.dataLoader = dataLoader;

            this.logger = new Logger();
            this.transformCache = new ConcurrentDictionary<string, IStoredProcedure>();
            this.converter = new DefaultTypeConverter();
        }

        public object GetTable(string name)
        {
            return database.GetTable(name);
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

        public DbSchema Schema
        {
            get { return this.schema; }
        }

        public bool IsInitialized
        {
            get { return this.database != null; }
        }

        public void Initialize(StoreItemCollection edmStoreSchema)
        {
            if (this.IsInitialized)
            {
                return;
            }

            // TODO: locking

            this.schema = DbSchemaStore.GetDbSchema(edmStoreSchema, this.CreateDbSchema);

            Stopwatch swDatabase = Stopwatch.StartNew();
            Database database = new Database();

            using (ITableDataLoaderFactory loaderFactory = this.CreateDataLoaderFactory())
            {
                foreach (DbTableInformation tableInfo in this.schema.Tables)
                {
                    Stopwatch swTable = Stopwatch.StartNew();

                    IEnumerable<object> initialData = this.GetInitialData(loaderFactory, tableInfo);

                    // Initialize the table
                    DatabaseReflectionHelper.CreateTable(
                        database,
                        tableInfo.EntityType,
                        tableInfo.PrimaryKeyFields,
                        tableInfo.IdentityField,
                        initialData);

                    swTable.Stop();

                    this.Logger.Write("{1} created in {0:0.0} ms", swTable.Elapsed.TotalMilliseconds, tableInfo.TableName);
                }
            }

            this.Logger.Write("Setting up assocations...");

            foreach (DbRelationInformation relation in this.schema.Relations)
            {
                // TODO: add relation
                // DatabaseReflectionHelper.CreateAssociation(database, constraint);
            }

            swDatabase.Stop();
            this.Logger.Write("Database buildup finished in {0:0.0} ms", swDatabase.Elapsed.TotalMilliseconds);

            this.database = database;
        }

        private IEnumerable<object> GetInitialData(ITableDataLoaderFactory loaderFactory, DbTableInformation tableInfo)
        {
            if (this.dataLoader != null && this.dataLoader.Cached)
            {
                DbTableInitialData initialData =  
                    TableInitialDataStore.GetDbInitialData(this.dataLoader, tableInfo.EntityType, 
                        () => new DbTableInitialData(CreateInitialData(loaderFactory, tableInfo)));

                return initialData.GetClonedInitialData();
            }
            else
            {
                return CreateInitialData(loaderFactory, tableInfo);
            }
        }

        private IEnumerable<object> CreateInitialData(ITableDataLoaderFactory loaderFactory, DbTableInformation tableInfo)
        {
            return ObjectLoader.Load(loaderFactory, tableInfo.TableName, tableInfo.EntityType); ;
        }

        private ITableDataLoaderFactory CreateDataLoaderFactory()
        {
            if (this.dataLoader == null)
            {
                return new EmptyTableDataLoaderFactory();
            }

            return this.dataLoader.CreateTableDataLoaderFactory();
        }

        private DbSchema CreateDbSchema(StoreItemCollection edmStoreSchema)
        {
            EntityContainer entityContainer = edmStoreSchema.GetItems<EntityContainer>().FirstOrDefault();
            DbSchema schema = new DbSchema();

            // Dynamic Library for Effort
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("Effort_DynamicEntityLib({0})", Guid.NewGuid())),
                    AssemblyBuilderAccess.Run);

            // Module for the entity types
            ModuleBuilder entityModule = assembly.DefineDynamicModule("Entities");
            // Initialize type converter
            EdmTypeConverter typeConverter = new EdmTypeConverter(converter);

            foreach (EntitySet entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                EntityType type = entitySet.ElementType;
                TypeBuilder entityTypeBuilder = entityModule.DefineType(entitySet.Name, TypeAttributes.Public);

                List<PropertyInfo> primaryKeyFields = new List<PropertyInfo>();
                List<PropertyInfo> identityFields = new List<PropertyInfo>();
                List<PropertyInfo> properties = new List<PropertyInfo>();

                // Add properties as entity fields
                foreach (EdmProperty field in type.Properties)
                {
                    TypeFacets facets = typeConverter.GetTypeFacets(field.TypeUsage);
                    Type fieldClrType = typeConverter.Convert(field.TypeUsage);

                    PropertyInfo prop = EmitHelper.AddProperty(entityTypeBuilder, field.Name, fieldClrType);
                    properties.Add(prop);

                    // Register primary key field
                    if (type.KeyMembers.Contains(field))
                    {
                        primaryKeyFields.Add(prop);
                    }

                    // Register identity field
                    if (facets.Identity)
                    {
                        identityFields.Add(prop);
                    }
                }

                if (identityFields.Count > 1)
                {
                    throw new InvalidOperationException("More than one identity fields is not supported");
                }

                Type entityType = entityTypeBuilder.CreateType();

                schema.RegisterTable(
                    entityType.Name,
                    entityType,
                    primaryKeyFields.ToArray(),
                    identityFields.FirstOrDefault(),
                    properties.ToArray());
            }

            foreach (AssociationSet associationSet in entityContainer.BaseEntitySets.OfType<AssociationSet>())
            {
                var constraints = associationSet.ElementType.ReferentialConstraints;

                if (constraints.Count != 1)
                {
                    continue;
                }

                ReferentialConstraint constraint = constraints[0];

                string fromTableName = GetTableName(constraint.FromRole);
                string toTableName = GetTableName(constraint.ToRole);

                DbTableInformation fromTable = schema.GetTable(fromTableName);
                DbTableInformation toTable = schema.GetTable(toTableName);

                PropertyInfo[] fromTableProperties = GetRelationProperties(constraint.FromProperties, fromTable);
                PropertyInfo[] toTableProperties = GetRelationProperties(constraint.ToProperties, toTable);

                schema.RegisterRelation(fromTableName, fromTableProperties, toTableName, toTableProperties);
            }

            return schema;
        }

        private string GetTableName(RelationshipEndMember relationEndpoint)
        {
            RefType refType = relationEndpoint.TypeUsage.EdmType as RefType;
            return refType.ElementType.Name;
        }

        private PropertyInfo[] GetRelationProperties(ReadOnlyMetadataCollection<EdmProperty> properties, DbTableInformation table)
        {
            return properties.Select(edmp => table.Properties.Single(clrp => clrp.Name == edmp.Name)).ToArray();
        }
    }
}
