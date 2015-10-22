// --------------------------------------------------------------------------------------------
// <copyright file="DatabaseReflectionHelper.cs" company="Effort Team">
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

namespace Effort.Internal.Common
{
    using System;
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Exceptions;
    using Effort.Internal.DbManagement;
    using Effort.Internal.DbManagement.Engine;
    using Effort.Internal.DbManagement.Schema;
    using NMemory;
    using NMemory.Constraints;
    using NMemory.Indexes;
    using NMemory.Linq;
    using NMemory.Modularity;
    using NMemory.StoredProcedures;
    using NMemory.Tables;
    using NMemory.Transactions;

    internal static class DatabaseReflectionHelper
    {
        public static ITable CreateTable(
            Database database,
            Type entityType,
            IKeyInfo primaryKeyInfo,
            MemberInfo identityField,
            object[] constraintFactories)
        {
            object identity = null;

            if (identityField != null)
            {
                ParameterExpression p = Expression.Parameter(entityType, "x");

                identity = Expression.Lambda(
                    Expression.Convert(
                        Expression.MakeMemberAccess(p, identityField),
                        typeof(long)),
                    p);
            }

            object table = typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("CreateTable")
                .MakeGenericMethod(entityType, primaryKeyInfo.KeyType)
                .Invoke(null, new object[] { 
                    database, 
                    primaryKeyInfo, 
                    identity,
                    constraintFactories });

            return table as ITable;
        }

        public static IIndex CreateIndex(ITable table, IKeyInfo key, bool unique)
        {
            return typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("CreateIndex")
                .MakeGenericMethod(
                    table.EntityType, 
                    table.PrimaryKeyIndex.KeyInfo.KeyType, 
                    key.KeyType)
                .Invoke(null, new object[] { 
                    table, 
                    key, 
                    unique }) as IIndex;
        }

        public static void InitializeTableData(
            ITable table, 
            IEnumerable<object> entities)
        {
            try
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("InitializeTableData")
                    .MakeGenericMethod(table.EntityType)
                    .Invoke(null, new object[] { table, entities });
            }
            catch (TargetInvocationException ex)
            {
                string message =
                    string.Format(
                        ExceptionMessages.TableInitializationFailed,
                        table);

                throw new EffortException(message, ex.InnerException);
            }
        }

        public static void InsertEntity(
            ITable table, 
            object entity, 
            Transaction transaction)
        {
            typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("InsertEntity")
                .MakeGenericMethod(table.ElementType)
                .Invoke(null, new object[] { table, entity, transaction });
        }

        public static IEnumerable<object> UpdateEntities(
            IQueryable source, 
            Expression updater, 
            Transaction transaction)
        {
            return
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("UpdateEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source, updater, transaction }) as IEnumerable<object>;
        }

        public static int DeleteEntities(
            IQueryable source, 
            Transaction transaction)
        {
            int count = (int)
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("DeleteEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source, transaction });

            return count;
        }

        public static IQueryable CreateTableQuery(
            Expression query, 
            Database database)
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

        public static ISharedStoredProcedure CreateSharedStoredProcedure(
            LambdaExpression query)
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

            if (!queryType.IsGenericType || 
                queryType.GetGenericTypeDefinition() != typeof(IQueryable<>))
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

        public static void CreateAssociation(
            Database database, 
            DbRelationInfo relation)
        {
            ITable primaryTable = database.GetTable(relation.PrimaryTable);
            ITable foreignTable = database.GetTable(relation.ForeignTable);

            IIndex primaryIndex = FindIndex(primaryTable, relation.PrimaryKeyInfo, true);
            IIndex foreignIndex = FindIndex(foreignTable, relation.ForeignKeyInfo, false);

            RelationOptions options = 
                new RelationOptions(
                    cascadedDeletion: relation.CascadedDelete);

            typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("CreateRelation")
                .MakeGenericMethod(
                    primaryTable.EntityType, 
                    primaryIndex.KeyInfo.KeyType, 
                    foreignTable.EntityType,
                    foreignIndex.KeyInfo.KeyType)
                .Invoke(null, new object[] { 
                    database, 
                    primaryIndex, 
                    foreignIndex, 
                    relation.ForeignToPrimaryConverter, 
                    relation.PrimaryToForeignConverter,
                    options
                });
        }

        private static IIndex FindIndex(ITable table, IKeyInfo key, bool unique)
        {
            IEnumerable<IIndex> indexes = table.Indexes;

            if (unique)
            {
                indexes = indexes.OfType<IUniqueIndex>();
            }

            foreach (IIndex index in indexes)
            {
                if (index.KeyInfo == key)
                {
                    return index;
                }
            }

            throw new InvalidOperationException("Index was not found");
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
            public static void InsertEntity<TEntity>(
                ITable<TEntity> table, 
                TEntity entity, 
                Transaction transaction)
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
                object[] constraintFactories)

                where TEntity : class
            {
                Table<TEntity, TPrimaryKey> table = database.Tables.Create<TEntity, TPrimaryKey>(
                    primaryKeyInfo,
                    identity != null ? new IdentitySpecification<TEntity>(identity) : null);

                foreach (var constraintFactory in 
                    constraintFactories.Cast<IConstraintFactory<TEntity>>())
                {
                    table.Contraints.Add(constraintFactory);
                }
                
                return table;
            }

            public static IIndex<TEntity, TKey> CreateIndex<TEntity, TPrimaryKey, TKey>(
                Table<TEntity, TPrimaryKey> table, 
                IKeyInfo<TEntity, TKey> key, 
                bool unique) 
                
                where TEntity : class
            {
                IIndexFactory factory = new RedBlackTreeIndexFactory();

                if (unique)
                {
                    return table.CreateUniqueIndex(factory, key);
                }
                else
                {
                    return table.CreateIndex(factory, key);
                }
            }

            public static void InitializeTableData<TEntity>(
               ITable<TEntity> table,
               IEnumerable<object> entities)

               where TEntity : class
            {
                IExtendedTable<TEntity> exTable = table as IExtendedTable<TEntity>;

                if (exTable != null)
                {
                    exTable.Initialize(entities.Cast<TEntity>());
                }
            }

            public static IIndex CreateForeignKeyIndex<TEntity, TPrimaryKey, TForeignKey>(
                Table<TEntity, TPrimaryKey> table, 
                IKeyInfo<TEntity, TForeignKey> foreignKeyinfo)
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

            public static ISharedStoredProcedure<T> CreateSharedStoredProcedure<T>(
                Expression<Func<IDatabase, IQueryable<T>>> expression)
            {
                return new SharedStoredProcedure<IDatabase, T>(expression);
            }

            public static IEnumerable<TEntity> UpdateEntities<TEntity>(
                IQueryable<TEntity> query, 
                Expression<Func<TEntity, TEntity>> updater, 
                Transaction transaction)
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

            public static int DeleteEntities<TEntity>(
                IQueryable<TEntity> query, 
                Transaction transaction)
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
                Func<TPrimaryKey, TForeignKey> primaryToForeign,
                RelationOptions options)

                where TPrimary : class
                where TForeign : class
            {
                try
                {
                    database.Tables.CreateRelation(
                        primaryIndex, 
                        foreignIndex, 
                        foreignToPrimary, 
                        primaryToForeign,
                        options);
                }
                catch (NMemory.Exceptions.NMemoryException ex)
                {
                    var message = string.Format("An exception has occurred during table initialization while relating "
                        + "key '{0}' to '{1}'.", primaryIndex, foreignIndex);

                    throw new InvalidOperationException(message, ex);
                }
            }
        }
    }
}
