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
using NMemory.StoredProcedures;
using NMemory.Tables;
using NMemory.Transactions;
using NMemory.Modularity;

namespace Effort.Internal.Common
{
    internal static class DatabaseReflectionHelper
    {
        public static ITable CreateTable(
            Database database, 
            Type entityType, 
            
            PropertyInfo[] primaryKeyFields,
           
            PropertyInfo identityField,
             object[] constraints,
            IEnumerable<object> initialEntities)
        {
            if (primaryKeyFields.Length == 0)
            {
                throw new ArgumentException("At least one primary key", "primaryKeys");
            }

            LambdaExpression primaryKeyExpression = LambdaExpressionHelper.CreateSelectorExpression(entityType, primaryKeyFields.OrderBy(f => f.Name).ToArray());
            Type primaryKeyType = primaryKeyExpression.Body.Type;

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
                .MakeGenericMethod(entityType, primaryKeyType)
                .Invoke(null, new object[] { database, primaryKeyExpression, identity, initialEntities, constraints });

            return table as ITable;
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


        private static ITable GetTable(Database database, RelationshipEndMember rel)
        {
            if (rel.TypeUsage.EdmType.BuiltInTypeKind != BuiltInTypeKind.RefType)
            {
                return null;
            }

            RefType refType = rel.TypeUsage.EdmType as RefType;

            return database.GetTable(refType.ElementType.Name);
        }


        /// <summary>
        /// todo: RelationKeyConverterFactory
        /// </summary>
        /// <param name="database"></param>
        /// <param name="relation"></param>
        public static void CreateAssociation(Database database, DbRelationInformation relation)
        {
            
            // Get the referenced tables
            ITable primaryTable = database.GetTable(relation.PrimaryTable);
            ITable foreignTable = database.GetTable(relation.ForeignTable);

            

            //CreatePrimaryToForeignConverter //constraints
            

            Type[] toTableGenericsArgs = foreignTable.GetType().GetGenericArguments();

            if (relation.PrimaryProperties.Length == 1 && relation.ForeignProperties.Length == 1)
            {
                // Simple association

                // Get the properties of the foreign key
                PropertyInfo foreignKeyProp = relation.ForeignProperties[0];
                PropertyInfo primaryKeyProp = relation.PrimaryProperties[0];

                IIndex uniqueKeyIndex = null;

                // Check for existing indexes on the foreign table
                foreach (IIndex existingPrimaryTableIndex in primaryTable.Indexes)
                {
                    // Check if not unique index
                    if (!(existingPrimaryTableIndex is IUniqueIndex))
                    {
                        continue;
                    }
                    MemberInfo[] indexMembers = existingPrimaryTableIndex.KeyInfo.EntityKeyMembers;

                    if (indexMembers.Length == 1 && indexMembers[0].Name == primaryKeyProp.Name)
                    {
                        uniqueKeyIndex = existingPrimaryTableIndex;
                    }
                }

                if (uniqueKeyIndex == null)
                {
                    // TODO: Create unique index
                    Debug.Print("Unique key index is not defined");
                    return;
                }

                IIndex foreignKeyIndex = null;

                // Check for existing indexes on the foreign table
                foreach (IIndex existingForeignTableIndex in foreignTable.Indexes)
                {
                    MemberInfo[] indexMembers = existingForeignTableIndex.KeyInfo.EntityKeyMembers;

                    if (indexMembers.Length == 1 && indexMembers[0].Name == foreignKeyProp.Name)
                    {
                        foreignKeyIndex = existingForeignTableIndex;
                    }
                }

                // If the approriate index does not exist, it has to be created
                if (foreignKeyIndex == null)
                {
                    // Create foreign key selector expression
                    LambdaExpression toExpression =
                        LambdaExpressionHelper.CreateSelectorExpression(
                            foreignTable.EntityType,
                            relation.ForeignProperties);

                    // Build foreign key index
                    foreignKeyIndex = typeof(DatabaseReflectionHelper.WrapperMethods)
                        .GetMethod("CreateForeignKeyIndex")
                        .MakeGenericMethod(toTableGenericsArgs[0], toTableGenericsArgs[1], toExpression.Body.Type)
                        .Invoke(null, new object[] { foreignTable, toExpression }) as IIndex;
                }

                // Register association in the database

                //CreateRelationWithComplexTypes!!!
                //todo: database.Tables.CreateRelation() meghívni. 

                // Identical index type
                if (uniqueKeyIndex.KeyInfo.KeyType == foreignKeyIndex.KeyInfo.KeyType)
                {
                    typeof(DatabaseReflectionHelper.WrapperMethods)
                        .GetMethod("CreateRelationWithSameKeyType")
                        .MakeGenericMethod(primaryTable.EntityType, uniqueKeyIndex.KeyInfo.KeyType, foreignTable.EntityType)
                        .Invoke(null, new object[] { database, uniqueKeyIndex, foreignKeyIndex });

                }
                // Identical, but foreign key is nullable
                else if (
                    TypeHelper.IsNullable(foreignKeyIndex.KeyInfo.KeyType) &&
                    TypeHelper.MakeNotNullable(foreignKeyIndex.KeyInfo.KeyType) == uniqueKeyIndex.KeyInfo.KeyType)
                {
                    typeof(DatabaseReflectionHelper.WrapperMethods)
                       .GetMethod("CreateRelationWithSameKeyTypeNullable")
                       .MakeGenericMethod(primaryTable.EntityType, uniqueKeyIndex.KeyInfo.KeyType, foreignTable.EntityType)
                       .Invoke(null, new object[] { database, uniqueKeyIndex, foreignKeyIndex });
                }
                else
                {
                    throw new NotSupportedException();
                }
            }
            else
            {
                // Complex association is not supported
                Debug.Print("Complex association is not supported");
                return;
            }

            ////// Not identical => probably multifield index
            ////else
            ////{
            ////    typeof(DatabaseReflectionHelper.WrapperMethods)
            ////        .GetMethod("CreateRelation")
            ////        .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyType, toTable.EntityType, foreignKeyIndex.KeyType)
            ////        .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });
            ////}

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
                Expression<Func<TEntity, TPrimaryKey>> primaryKey,
                Expression<Func<TEntity, long>> identity,
                IEnumerable<object> initialEntities, object[] constraints)

                where TEntity : class
            {
                Table<TEntity, TPrimaryKey> table = database.Tables.Create<TEntity, TPrimaryKey>(
                    primaryKey,
                    identity != null ? new IdentitySpecification<TEntity>(identity) : null);

                ((IInitializableTable<TEntity>)table).Initialize(initialEntities.Cast<TEntity>());

                foreach (var constraint in constraints.Cast<NMemory.Constraints.IConstraint<TEntity>>())
                {
                    table.AddConstraint(constraint);
                }

                return table;
            }

            public static IIndex CreateForeignKeyIndex<TEntity, TPrimaryKey, TForeignKey>(Table<TEntity, TPrimaryKey> table, Expression<Func<TEntity, TForeignKey>> foreignKey)
                where TEntity : class
            {
                var indexFactory = new RedBlackTreeIndexFactory();
				////var indexFactory = new DictionaryIndexFactory<TEntity, TPrimaryKey, TEntity>();

                return table.CreateIndex<TForeignKey>(indexFactory, foreignKey);
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



            public static void CreateRelationWithComplexTypes<TPrimary, TPrimaryKey, TForeign, TForeignKey>(
                Database database, 
                UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, 
                IIndex<TForeign, TForeignKey> foreignIndex,
                Expression<Func<TForeignKey, TPrimaryKey>> foreignToPrimary,
                Expression<Func<TPrimaryKey, TForeignKey>> primaryToForeign)

                where TPrimary : class
                where TForeign : class
            {
                database.Tables.CreateRelation(primaryIndex, foreignIndex, foreignToPrimary.Compile(), primaryToForeign.Compile());
            }

            public static void CreateRelationWithSameKeyType<TPrimary, TPrimaryKey, TForeign>(
                Database database, 
                UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, 
                IIndex<TForeign, TPrimaryKey> foreignIndex)

                where TPrimary : class
                where TForeign : class
            {
                database.Tables.CreateRelation(primaryIndex, foreignIndex, x => x, x => x);
            }

            public static void CreateRelationWithSameKeyTypeNullable<TPrimary, TPrimaryKey, TForeign>(
                Database database, 
                UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, 
                IIndex<TForeign, Nullable<TPrimaryKey>> foreignIndex)

                where TPrimary : class
                where TForeign : class
                where TPrimaryKey : struct
            {
                database.Tables.CreateRelation(primaryIndex, foreignIndex, x => x.Value, x => x);
            }
        }


    }
}
