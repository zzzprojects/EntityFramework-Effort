// --------------------------------------------------------------------------------------------
// <copyright file="DatabaseReflectionHelper.cs" company="Effort Team">
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

namespace Effort.Internal.Common
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Diagnostics;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.DbManagement;
    using NMemory;
    using NMemory.Indexes;
    using NMemory.Linq;
    using NMemory.Modularity;
    using NMemory.StoredProcedures;
    using NMemory.Tables;
    using NMemory.Transactions;
    using Effort.Internal.DbManagement.Schema;

    internal static class DatabaseReflectionHelper
    {
        public static ITable CreateTable(
            Database database,
            Type entityType,
            IKeyInfo primaryKeyInfo,
            PropertyInfo identityField,
            object[] constraints)
        {
            object identity = null;

            if (identityField != null)
            {
                ParameterExpression p = Expression.Parameter(entityType, "x");

                identity = Expression.Lambda(
                    Expression.Convert(
                        Expression.Property(p, identityField.Name),
                        typeof(long)),
                    p);
            }

            object table =
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("CreateTable")
                .MakeGenericMethod(entityType, primaryKeyInfo.KeyType)
                .Invoke(null, new object[] { database, primaryKeyInfo, identity, constraints });

            return table as ITable;
        }

        public static void InitializeTableData(ITable table, IEnumerable<object> entities)
        {
            typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("InitializeTableData")
                .MakeGenericMethod(table.EntityType)
                .Invoke(null, new object[] { table, entities });
        }

        public static void InsertEntity(ITable table, object entity, Transaction transaction)
        {
            typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("InsertEntity")
                .MakeGenericMethod(table.ElementType)
                .Invoke(null, new object[] { table, entity, transaction });
        }

        public static IEnumerable<object> UpdateEntities(IQueryable source, Expression updater, Transaction transaction)
        {
            return
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("UpdateEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source, updater, transaction }) as IEnumerable<object>;
        }

        public static int DeleteEntities(IQueryable source, Transaction transaction)
        {
            int count = (int)
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("DeleteEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source, transaction });

            return count;
        }

        public static IQueryable CreateTableQuery(Expression query, Database database)
        {
            if (query.Type.GetGenericTypeDefinition() != typeof(IQueryable<>))
            {
                throw new ArgumentException("query is not IQueryable<>");
            }

            Type entityType = TypeHelper.GetElementType(query.Type);

            IQueryable tableQuery =
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateTableQuery")
                    .MakeGenericMethod(entityType)
                    .Invoke(null, new object[] { query, database }) as IQueryable;

            return tableQuery;
        }

        public static ISharedStoredProcedure CreateSharedStoredProcedure(LambdaExpression query)
        {
            if (!query.Type.IsGenericType || query.Type.GetGenericTypeDefinition() != typeof(Func<,>))
            {
                throw new ArgumentException("Invalid query", "query");
            }

            Type[] queryArgs = query.Type.GetGenericArguments();

            if (queryArgs[0] != typeof(IDatabase))
            {
                throw new ArgumentException("Invalid query", "query");
            }

            Type queryType = queryArgs[1];

            if (!queryType.IsGenericType || queryType.GetGenericTypeDefinition() != typeof(IQueryable<>))
            {
                throw new ArgumentException("Not IQueryable<>", "query");
            }

            Type entityType = TypeHelper.GetElementType(queryType);

            ISharedStoredProcedure procedure =
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateSharedStoredProcedure")
                    .MakeGenericMethod(entityType)
                    .Invoke(null, new object[] { query }) as ISharedStoredProcedure;

            return procedure;
        }

        public static void CreateAssociation(Database database, DbRelationInformation relation)
        {
            // Get the referenced tables
            ITable primaryTable = database.GetTable(relation.PrimaryTable);
            ITable foreignTable = database.GetTable(relation.ForeignTable);

            Type[] toTableGenericsArgs = foreignTable.GetType().GetGenericArguments();

            IIndex primaryIndex = null;

            // Check for existing indexes on the primary table. 
            // The identification of primary index is made by comparing the primaryproperties of relation with the entitykeymembers of the index
            string[] primarymembers = relation.PrimaryProperties.Select(x => x.Name).OrderBy(x => x).ToArray();
            foreach (IIndex existingPrimaryTableIndex in primaryTable.Indexes)
            {
                // Check if not unique index
                if (!(existingPrimaryTableIndex is IUniqueIndex))
                {
                    continue;
                }

                string[] indexMembers = existingPrimaryTableIndex.KeyInfo.EntityKeyMembers.Select(x => x.Name).OrderBy(x => x).ToArray();

                if (primarymembers.SequenceEqual(indexMembers))
                {
                    primaryIndex = existingPrimaryTableIndex;
                }
            }

            if (primaryIndex == null)
            {
                // TODO: Create primary index
                Debug.Print("Unique key index is not defined");
                return;
            }

            IIndex foreignKeyIndex = null;

            // Check for existing indexes on the foreign table
            // The identification of foreign index is made by comparing the foreignproperties of relation with the entitykeymembers of the index
            string[] foreignmembers = relation.ForeignProperties.Select(x => x.Name).OrderBy(x => x).ToArray();
            foreach (IIndex existingForeignTableIndex in foreignTable.Indexes)
            {
                string[] indexMembers = existingForeignTableIndex.KeyInfo.EntityKeyMembers.Select(x => x.Name).OrderBy(x => x).ToArray();

                if (foreignmembers.SequenceEqual(indexMembers))
                {
                    foreignKeyIndex = existingForeignTableIndex;
                }
            }

            // If the approriate index does not exist, it has to be created
            if (foreignKeyIndex == null)
            {
                // Build foreign key index (with the keyinfo from the relation)
                foreignKeyIndex = typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateForeignKeyIndex")
                    .MakeGenericMethod(toTableGenericsArgs[0], toTableGenericsArgs[1], ((IKeyInfo)relation.ForeignKeyInfo).KeyType)
                    .Invoke(null, new object[] { foreignTable, relation.ForeignKeyInfo }) as IIndex;
            }

            // Create the relation
            typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("CreateRelation")
                .MakeGenericMethod(primaryTable.EntityType, primaryIndex.KeyInfo.KeyType, foreignTable.EntityType, foreignKeyIndex.KeyInfo.KeyType)
                .Invoke(null, new object[] { database, primaryIndex, foreignKeyIndex, relation.ForeignToPrimaryConverter, relation.PrimaryToForeignConverter });
        }

        private static ITable GetTable(Database database, RelationshipEndMember rel)
        {
            if (rel.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.RefType)
            {
                return null;
            }

            RefType refType = rel.TypeUsage.EdmType as RefType;

            return database.GetTable(refType.ElementType.Name);
        }

        private static class WrapperMethods
        {
            public static void InsertEntity<TEntity>(ITable<TEntity> table, TEntity entity, Transaction transaction)
                where TEntity : class
            {
                if (transaction != null)
                {
                    table.Insert(entity, transaction);
                }
                else
                {
                    table.Insert(entity);
                }
            }

            public static Table<TEntity, TPrimaryKey> CreateTable<TEntity, TPrimaryKey>(
                Database database,
                IKeyInfo<TEntity, TPrimaryKey> primaryKeyInfo,
                Expression<Func<TEntity, long>> identity, 
                object[] constraints)

                where TEntity : class
            {
                Table<TEntity, TPrimaryKey> table = database.Tables.Create<TEntity, TPrimaryKey>(
                    primaryKeyInfo,
                    identity != null ? new IdentitySpecification<TEntity>(identity) : null);

                

                foreach (var constraint in constraints.Cast<NMemory.Constraints.IConstraint<TEntity>>())
                {
                    table.AddConstraint(constraint);
                }

                return table;
            }

            public static void InitializeTableData<TEntity>(
               ITable<TEntity> table,
               IEnumerable<object> entities)

               where TEntity : class
            {
                ((IInitializableTable<TEntity>)table).Initialize(entities.Cast<TEntity>());
            }

            public static IIndex CreateForeignKeyIndex<TEntity, TPrimaryKey, TForeignKey>(Table<TEntity, TPrimaryKey> table, IKeyInfo<TEntity, TForeignKey> foreignKeyinfo)
                where TEntity : class
            {
                var indexFactory = new RedBlackTreeIndexFactory();
                ////var indexFactory = new DictionaryIndexFactory<TEntity, TPrimaryKey, TEntity>();
                return table.CreateIndex<TForeignKey>(indexFactory, foreignKeyinfo);
            }

            public static TableQuery<T> CreateTableQuery<T>(Expression expression, Database database)
            {
                TableQuery<T> query = new TableQuery<T>(database, expression);

                return query;
            }

            public static ISharedStoredProcedure<T> CreateSharedStoredProcedure<T>(Expression<Func<IDatabase, IQueryable<T>>> expression)
            {
                return new SharedStoredProcedure<IDatabase, T>(expression);
            }

            public static IEnumerable<TEntity> UpdateEntities<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, TEntity>> updater, Transaction transaction)
                where TEntity : class
            {
                if (transaction != null)
                {
                    return NMemory.Linq.QueryableEx.Update(query, updater, transaction);
                }
                else
                {
                    return NMemory.Linq.QueryableEx.Update(query, updater);
                }
            }

            public static int DeleteEntities<TEntity>(IQueryable<TEntity> query, Transaction transaction)
                where TEntity : class
            {
                if (transaction != null)
                {
                    return NMemory.Linq.QueryableEx.Delete(query, transaction);
                }
                else
                {
                    return NMemory.Linq.QueryableEx.Delete(query);
                }
            }

            public static void CreateRelation<TPrimary, TPrimaryKey, TForeign, TForeignKey>(
                Database database,
                UniqueIndex<TPrimary, TPrimaryKey> primaryIndex,
                IIndex<TForeign, TForeignKey> foreignIndex,
                Func<TForeignKey, TPrimaryKey> foreignToPrimary,
                Func<TPrimaryKey, TForeignKey> primaryToForeign)

                where TPrimary : class
                where TForeign : class
            {
                database.Tables.CreateRelation(primaryIndex, foreignIndex, foreignToPrimary, primaryToForeign);
            }
        }
    }
}
