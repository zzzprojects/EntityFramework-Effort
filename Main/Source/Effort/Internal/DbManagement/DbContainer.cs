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
using System.Collections;
using System.Linq.Expressions;

namespace Effort.Internal.DbManagement
{
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

        public bool IsInitialized(StoreItemCollection edmStoreSchema)
        {
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

            // TODO: locking

            DbSchema schema = DbSchemaStore.GetDbSchema(edmStoreSchema, CreateDbSchema);
            this.Initialize(schema);
        }

        public void Initialize(DbSchema schema)
        {
            Stopwatch swDatabase = Stopwatch.StartNew();

            // Database initialization (put it in the constructor?)
            if (this.database == null)
            {
                this.database = new Database();
            }

            using (ITableDataLoaderFactory loaderFactory = this.CreateDataLoaderFactory())
            {
                foreach (DbTableInformation tableInfo in schema.Tables)
                {
                    Stopwatch swTable = Stopwatch.StartNew();

                    IEnumerable<object> initialData = this.GetInitialData(loaderFactory, tableInfo);

                    //todo: DefaultKeyInfoFactory használata IKeyInfo létrehozására
                    //todo: Identity közvetlen létrehozása. 

                    // Initialize the table
                    DatabaseReflectionHelper.CreateTable(
                        this.database,
                        tableInfo.EntityType,
                        tableInfo.PrimaryKeyFields,
                        tableInfo.IdentityField,
                        tableInfo.Constraints,
                        initialData);

                    swTable.Stop();

                    this.Logger.Write("{1} created in {0:0.0} ms", swTable.Elapsed.TotalMilliseconds, tableInfo.TableName);
                }
            }

            this.Logger.Write("Setting up assocations...");

            foreach (DbRelationInformation relation in schema.Relations)
            {
                DatabaseReflectionHelper.CreateAssociation(database, relation);
            }

            swDatabase.Stop();
            this.Logger.Write("Database buildup finished in {0:0.0} ms", swDatabase.Elapsed.TotalMilliseconds);
        }

        private IEnumerable<object> GetInitialData(ITableDataLoaderFactory loaderFactory, DbTableInformation tableInfo)
        {
            if (this.parameters.DataLoader != null && !this.parameters.IsDataLoaderCached)
            {
                DbTableInitialData initialData =  
                    TableInitialDataStore.GetDbInitialData(this.parameters.DataLoader, tableInfo.EntityType, 
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
            if (this.parameters.DataLoader == null)
            {
                return new EmptyTableDataLoaderFactory();
            }

            return this.parameters.DataLoader.CreateTableDataLoaderFactory();
        }

        public static DbSchema CreateDbSchema(StoreItemCollection edmStoreSchema)
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
            EdmTypeConverter typeConverter = new EdmTypeConverter(new DefaultTypeConverter());

            foreach (EntitySet entitySet in entityContainer.BaseEntitySets.OfType<EntitySet>())
            {
                EntityType type = entitySet.ElementType;
                TypeBuilder entityTypeBuilder = entityModule.DefineType(entitySet.Name, TypeAttributes.Public);

                List<PropertyInfo> primaryKeyFields = new List<PropertyInfo>();
                List<PropertyInfo> identityFields = new List<PropertyInfo>();
                List<PropertyInfo> properties = new List<PropertyInfo>();

                List<string> primaryKeyFieldNames = new List<string>();
                List<string> identityFieldNames = new List<string>();
                List<string> notNullableFields = new List<string>();

                // Add properties as entity fields
                foreach (EdmProperty field in type.Properties)
                {
                    TypeFacets facets = typeConverter.GetTypeFacets(field.TypeUsage);
                    Type fieldClrType = typeConverter.Convert(field.TypeUsage);

                    PropertyBuilder propBuilder = EmitHelper.AddProperty(entityTypeBuilder, field.Name, fieldClrType);

                    // Register primary key field
                    if (type.KeyMembers.Contains(field))
                    {
                        primaryKeyFieldNames.Add(propBuilder.Name);
                    }

                    // Register identity field
                    if (facets.Identity)
                    {
                        identityFieldNames.Add(propBuilder.Name);
                    }
                    if (facets.Nullable == false)
                    {
                        notNullableFields.Add(propBuilder.Name);
                    }
                }

                if (identityFields.Count > 1)
                {
                    throw new InvalidOperationException("More than one identity fields is not supported");
                }

                Type entityType = entityTypeBuilder.CreateType();

                Type constraintListType = typeof(List<>)
                                      .MakeGenericType(typeof(NMemory.Constraints.IConstraint<>).MakeGenericType(entityType));

                var listInstanceOfConstraints = (IList)constraintListType
                      .GetConstructor(Type.EmptyTypes)
                      .Invoke(null);
                foreach (PropertyInfo prop in entityType.GetProperties())
                {
                    properties.Add(prop);

                    if (primaryKeyFieldNames.Contains(prop.Name))
                    {
                        primaryKeyFields.Add(prop);
                    }

                    if (identityFieldNames.Contains(prop.Name))
                    {
                        identityFields.Add(prop);
                    }

                    if (notNullableFields.Contains(prop.Name))
                    {
                        var param = Expression.Parameter(entityType, "x");
                        constraintListType.GetMethod("Add").Invoke(listInstanceOfConstraints, new object[]{
                            typeof(NMemory.Constraints.NotNullableConstraint<>).MakeGenericType(entityType).GetConstructors().First().Invoke(
                            new object[]{
                                    Expression.Lambda(Expression.Convert(Expression.PropertyOrField(param,prop.Name),typeof(object)),param)
                            }
                            
                            )});
                    }
                }

                schema.RegisterTable(
                    entityType.Name,
                    entityType,
                    primaryKeyFields.ToArray(),
                    identityFields.SingleOrDefault(),
                    properties.ToArray(), listInstanceOfConstraints);
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

        private static string GetTableName(RelationshipEndMember relationEndpoint)
        {
            RefType refType = relationEndpoint.TypeUsage.EdmType as RefType;
            return refType.ElementType.Name;
        }

        private static PropertyInfo[] GetRelationProperties(ReadOnlyMetadataCollection<EdmProperty> properties, DbTableInformation table)
        {
            return properties.Select(edmp => table.Properties.Single(clrp => clrp.Name == edmp.Name)).ToArray();
        }
    }
}
