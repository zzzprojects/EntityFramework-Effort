using System;
using System.Collections.Generic;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.DatabaseManagement;
using MMDB;
using MMDB.Index;
using MMDB.Table;

namespace Effort.Helpers
{
    internal static class DatabaseReflectionHelper
    {
        public static IReflectionTable CreateTable(Database database, Type entityType, PropertyInfo[] primaryKeyFields, IEnumerable<object> initialEntities)
        {
            if (primaryKeyFields.Length == 0)
            {
                throw new ArgumentException("At least one primary key", "primaryKeys");
            }

            LambdaExpression primaryKeyExpression = LambdaExpressionHelper.CreateSelectorExpression(entityType, primaryKeyFields.OrderBy(f => f.Name).ToArray());
            Type primaryKeyType = primaryKeyExpression.Body.Type;

            object table = typeof(DatabaseReflectionHelper.WrapperMethods)
                            .GetMethod("CreateTable")
                            .MakeGenericMethod(entityType, primaryKeyType)
                            .Invoke(null, new object[] { database, primaryKeyExpression, initialEntities});

            return table as IReflectionTable;
        }

        private static IReflectionTable GetTable(Database database, RelationshipEndMember rel)
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
            // Get the referenced tables
            IReflectionTable fromTable = GetTable(database, constraint.FromRole);
            IReflectionTable toTable = GetTable(database, constraint.ToRole);

            Type[] toTableGenericsArgs = toTable.GetType().GetGenericArguments();

            // Get the properties of the foreign key
            MemberInfo[] foreignKeyProps = constraint
                .ToProperties
                .Select(ep => toTable.EntityType.GetProperty(ep.Name))
                .OrderBy(mi => mi.Name)
                .ToArray();

            IIndex toTablePrimaryIndex = toTable.Indexes.FirstOrDefault();
            IIndex fromTablePrimaryIndex = fromTable.Indexes.FirstOrDefault();

            // Check if the primary index exist
            if (fromTablePrimaryIndex == null)
            {
                return;
            }

            IIndex foreignKeyIndex = null;

            // Consider if the foreign key is also a primary key
            if (toTablePrimaryIndex != null && toTablePrimaryIndex.KeyMembers.OrderBy(mi => mi.Name).SequenceEqual(foreignKeyProps))
            {
                foreignKeyIndex = toTablePrimaryIndex;
            }
            else
            {
                // Create foreign key expression
                LambdaExpression toExpression =
                    LambdaExpressionHelper.CreateSelectorExpression(
                        toTable.EntityType,
                        constraint.ToProperties.Select(ep => toTable.EntityType.GetProperty(ep.Name)).ToArray());


                // Build foreign key index
                foreignKeyIndex = typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateForeignKeyIndex")
                    .MakeGenericMethod(toTableGenericsArgs[0], toTableGenericsArgs[1], toExpression.Body.Type)
                    .Invoke(null, new object[] { toTable, toExpression }) as IIndex;
            }


            // Register association in the database

            // Identical index type
            if (fromTablePrimaryIndex.KeyType == foreignKeyIndex.KeyType)
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateRelationWithSameKeyType")
                    .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyType, toTable.EntityType)
                    .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });

            }
            // Identical, but foreign key is nullable
            else if (
                TypeHelper.IsNullable(foreignKeyIndex.KeyType) && 
                TypeHelper.MakeNotNullable(foreignKeyIndex.KeyType) == fromTablePrimaryIndex.KeyType)
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                   .GetMethod("CreateRelationWithSameKeyTypeNullable")
                   .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyType, toTable.EntityType)
                   .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });
            }
            // Not identical => probably multifield index
            else
            {
                typeof(DatabaseReflectionHelper.WrapperMethods)
                    .GetMethod("CreateRelation")
                    .MakeGenericMethod(fromTable.EntityType, fromTablePrimaryIndex.KeyType, toTable.EntityType, foreignKeyIndex.KeyType)
                    .Invoke(null, new object[] { database, fromTablePrimaryIndex, foreignKeyIndex });
            }

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
                IEnumerable<object> initialEntities)

                where TEntity : class
            {
                return database.CreateTable<TEntity, TPrimaryKey>(
                    primaryKey,
                    null,
                    initialEntities.Cast<TEntity>());
            }

            public static IIndex CreateForeignKeyIndex<TEntity, TPrimaryKey, TForeignKey>(Table<TEntity, TPrimaryKey> table, Expression<Func<TEntity, TForeignKey>> foreignKey)
                where TEntity : class
            {
				// zoldl: legyen inkább hashtable
				// tamas: nem jo, mert FK lehet null is
                var indexFactory = new RedBlackTreeIndexFactory<TEntity, TPrimaryKey, TEntity>();
				//var indexFactory = new DictionaryIndexFactory<TEntity, TPrimaryKey, TEntity>();

                return table.CreateIndex<TForeignKey>(indexFactory, foreignKey);
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
