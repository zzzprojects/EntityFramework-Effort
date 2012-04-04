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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.DatabaseManagement;
using System.Diagnostics;
using NMemory;
using NMemory.Tables;
using NMemory.Indexes;
using NMemory.Linq;
using System.Collections;
using NMemory.StoredProcedures;

namespace Effort.Helpers
{
    internal static class DatabaseReflectionHelper
    {
        public static ITable CreateTable(
            Database database, 
            Type entityType, 
            
            PropertyInfo[] primaryKeyFields, 
            PropertyInfo identityField, 
            
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

            object table = typeof(DatabaseReflectionHelper.WrapperMethods)
                            .GetMethod("CreateTable")
                            .MakeGenericMethod(entityType, primaryKeyType)
                            .Invoke(null, new object[] { database, primaryKeyExpression, identity, initialEntities});

            return table as ITable;
        }

        public static int UpdateEntities(IQueryable source, Expression updater)
        {
            int count = (int)
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("UpdateEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source, updater });

            return count;
        }

        public static int DeleteEntities(IQueryable source)
        {
            int count = (int)
                typeof(DatabaseReflectionHelper.WrapperMethods)
                .GetMethod("DeleteEntities")
                .MakeGenericMethod(source.ElementType)
                .Invoke(null, new object[] { source });

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

        public static IStoredProcedure CreateStoredProcedure(Expression query, Database database)
        {
            if (query.Type.GetGenericTypeDefinition() != typeof(IQueryable<>))
            {
                throw new ArgumentException("not IQueryable<>", "query");
            }

            Type entityType = TypeHelper.GetElementType(query.Type);

            IStoredProcedure procedure =
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateStoredProcedure")
                    .MakeGenericMethod(entityType)
                    .Invoke(null, new object[] { query, database }) as IStoredProcedure;

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

        public static void CreateAssociation(Database database, ReferentialConstraint constraint)
        {
            if (1 < constraint.FromProperties.Count || 1 < constraint.ToProperties.Count)
            {
                // Complex association is not supported
                Debug.Print("Complex association is not supported");
                return;
            }

            // Get the referenced tables
            ITable fromTable = GetTable(database, constraint.FromRole);
            ITable toTable = GetTable(database, constraint.ToRole);

            Type[] toTableGenericsArgs = toTable.GetType().GetGenericArguments();

            IIndex toTablePrimaryIndex = toTable.PrimaryKeyIndex;
            IIndex fromTablePrimaryIndex = fromTable.PrimaryKeyIndex;

            // Check if the primary index exist
            if (fromTablePrimaryIndex == null)
            {
                return;
            }

            // Get the properties of the foreign key
            PropertyInfo foreignKeyProp = toTable.EntityType.GetProperty(constraint.ToProperties[0].Name);

            IIndex foreignKeyIndex = null;

            // Consider if the foreign key is also a primary key
            if (toTablePrimaryIndex != null && 
                toTablePrimaryIndex.KeyInfo.KeyMembers.Length == 1 &&
                toTablePrimaryIndex.KeyInfo.KeyMembers[0].Name == foreignKeyProp.Name)
            {
                foreignKeyIndex = toTablePrimaryIndex;
            }
            else
            {
                // Create foreign key expression
                LambdaExpression toExpression =
                    LambdaExpressionHelper.CreateSelectorExpression(
                        toTable.EntityType,
                        new PropertyInfo[] { foreignKeyProp });


                // Build foreign key index
                foreignKeyIndex = typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateForeignKeyIndex")
                    .MakeGenericMethod(toTableGenericsArgs[0], toTableGenericsArgs[1], toExpression.Body.Type)
                    .Invoke(null, new object[] { toTable, toExpression }) as IIndex;
            }


            // Register association in the database

            // Identical index type
            if (fromTablePrimaryIndex.KeyInfo.KeyType == foreignKeyIndex.KeyInfo.KeyType)
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateRelationWithSameKeyType")
                    .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyInfo.KeyType, toTable.EntityType)
                    .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });

            }
            // Identical, but foreign key is nullable
            else if (
                TypeHelper.IsNullable(foreignKeyIndex.KeyInfo.KeyType) &&
                TypeHelper.MakeNotNullable(foreignKeyIndex.KeyInfo.KeyType) == fromTablePrimaryIndex.KeyInfo.KeyType)
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                   .GetMethod("CreateRelationWithSameKeyTypeNullable")
                   .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyInfo.KeyType, toTable.EntityType)
                   .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });
            }
            else
            {
                throw new NotSupportedException();
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
            ////public static void AddEntity<TEntity, TPrimaryKey>(Table<TEntity, TPrimaryKey> table, TEntity entity)
            ////    where TEntity : class
            ////{
            ////    table.Insert(entity);
            ////}

            public static Table<TEntity, TPrimaryKey> CreateTable<TEntity, TPrimaryKey>(
                Database database, 
                Expression<Func<TEntity, TPrimaryKey>> primaryKey,
                Expression<Func<TEntity, long>> identity,
                IEnumerable<object> initialEntities)

                where TEntity : class
            {
                return database.CreateTable<TEntity, TPrimaryKey>(
                    primaryKey,
                    identity != null ? new IdentitySpecification<TEntity>(identity) : null,
                    initialEntities.Cast<TEntity>());
            }

            public static IIndex CreateForeignKeyIndex<TEntity, TPrimaryKey, TForeignKey>(Table<TEntity, TPrimaryKey> table, Expression<Func<TEntity, TForeignKey>> foreignKey)
                where TEntity : class
            {
                var indexFactory = new RedBlackTreeIndexFactory<TEntity>();
				////var indexFactory = new DictionaryIndexFactory<TEntity, TPrimaryKey, TEntity>();

                return table.CreateIndex<TForeignKey>(indexFactory, foreignKey);
            }

            public static TableQuery<T> CreateTableQuery<T>(Expression expression, Database database)
            {
                TableQuery<T> query = new TableQuery<T>(database, expression);

                return query;
            }

            public static IStoredProcedure CreateStoredProcedure<T>(Expression expression, Database database)
            {
                TableQuery<T> query = new TableQuery<T>(database, expression);

                return database.StoredProcedures.Create(query);
            }

            public static int UpdateEntities<TEntity>(IQueryable<TEntity> query, Expression<Func<TEntity, TEntity>> updater)
                where TEntity : class
            {
                return NMemory.Linq.QueryableEx.Update(query, updater);
            }

            public static int DeleteEntities<TEntity>(IQueryable<TEntity> query)
                where TEntity : class
            {
                return NMemory.Linq.QueryableEx.Delete(query);
            }


            public static void CreateRelation<TPrimary, TPrimaryKey, TForeign, TForeignKey>(Database database, UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, IIndex<TForeign, TForeignKey> foreignIndex)
                where TPrimary : class
                where TForeign : class
            {
                throw new NotImplementedException();
            }

            public static void CreateRelationWithSameKeyType<TPrimary, TPrimaryKey, TForeign>(Database database, UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, IIndex<TForeign, TPrimaryKey> foreignIndex)
                where TPrimary : class
                where TForeign : class
            {
                database.CreateRelation(primaryIndex, foreignIndex, x => x, x => x);
            }

            public static void CreateRelationWithSameKeyTypeNullable<TPrimary, TPrimaryKey, TForeign>(Database database, UniqueIndex<TPrimary, TPrimaryKey> primaryIndex, IIndex<TForeign, Nullable<TPrimaryKey>> foreignIndex)
                where TPrimary : class
                where TForeign : class
                where TPrimaryKey : struct
            {
                database.CreateRelation(primaryIndex, foreignIndex, x => x.Value, x => x);
            }
        }


    }
}
