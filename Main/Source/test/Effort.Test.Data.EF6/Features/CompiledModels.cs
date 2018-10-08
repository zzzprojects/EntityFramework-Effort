// --------------------------------------------------------------------------------------------
// <copyright file="CompiledModels.cs" company="Effort Team">
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

namespace Effort.Test.Data.Features
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Threading;

    public static class CompiledModels
    {
        private static readonly Type[] entityTypes;

        private static readonly Lazy<DbCompiledModel> defaultModel;
        private static readonly Lazy<DbCompiledModel> disabledIdentityModel;
        private static readonly Lazy<DbCompiledModel> tableNameModel;
        private static readonly Lazy<DbCompiledModel> decimalIdentityFieldModel;
        private static readonly Lazy<DbCompiledModel> indexedFieldModel;

        private static readonly ConcurrentDictionary<string, DbCompiledModel> models;
        
        static CompiledModels()
        {
            PropertyInfo[] props = typeof(FeatureDbContext).GetProperties();
            List<Type> types = new List<Type>();

            foreach (PropertyInfo prop in props)
            {
                Type type = prop.PropertyType;

                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IDbSet<>))
                {
                    types.Add(type.GetGenericArguments()[0]);
                }
            }

            entityTypes = types.ToArray();

            models = new ConcurrentDictionary<string, DbCompiledModel>(); 

            LazyThreadSafetyMode mode = LazyThreadSafetyMode.PublicationOnly;

            defaultModel =
                new Lazy<DbCompiledModel>(
                    () => CreateSimpleModel(typeof(StringFieldEntity)),
                    mode);

            disabledIdentityModel =
                new Lazy<DbCompiledModel>(
                    () => CreateDisabledIdentityModel(),
                    mode);

            tableNameModel =
                new Lazy<DbCompiledModel>(
                    () => CreateTableNameModel(),
                    mode);

            decimalIdentityFieldModel =
                new Lazy<DbCompiledModel>(
                    () => CreateDecimalIdentityFieldModel(),
                    mode);

            indexedFieldModel =
                new Lazy<DbCompiledModel>(
                    () => CreateIndexedFieldModel(),
                    mode);
        }

        public static DbCompiledModel DefaultModel
        {
            get { return defaultModel.Value; }
        }

        public static DbCompiledModel DisabledIdentityModel
        {
            get { return disabledIdentityModel.Value; }
        }

        public static DbCompiledModel TableNameModel
        {
            get { return tableNameModel.Value; }
        }

        public static DbCompiledModel DecimalIdenityFieldModel
        {
            get { return decimalIdentityFieldModel.Value; }
        }

        public static DbCompiledModel IndexedFieldModel
        {
            get { return indexedFieldModel.Value; }
        }

        public static DbCompiledModel GetModel(params Type[] allowedEntityTypes)
        {
            // The following key identifies the entity model
            string key = 
                string.Join(
                    ";", 
                    allowedEntityTypes
                        .Select(x => x.FullName)
                        .OrderBy(x => x));


            return models.GetOrAdd(key, _ => CreateSimpleModel(allowedEntityTypes));
        }

        public static DbCompiledModel GetModel<T1>()
        {
            return GetModel(typeof(T1));
        }

        public static DbCompiledModel GetModel<T1, T2>()
        {
            return GetModel(typeof(T1), typeof(T2));
        }

        private static DbCompiledModel CreateDisabledIdentityModel()
        {
            DbModelBuilder modelBuilder = new DbModelBuilder();

            ////RegisterEntityTypes(modelBuilder, null);

            modelBuilder
                .Entity<StringFieldEntity>()
                .Property(p => p.Id)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            return CompileModel(modelBuilder);
        }

        private static DbCompiledModel CreateTableNameModel()
        {
            DbModelBuilder modelBuilder = new DbModelBuilder();

            ////RegisterEntityTypes(modelBuilder, null);

            modelBuilder
                .Entity<StringFieldEntity>()
                .ToTable("Foo");

            return CompileModel(modelBuilder);
        }

        private static DbCompiledModel CreateSimpleModel(params Type[] allowedEntityTypes)
        {
            DbModelBuilder modelBuilder = new DbModelBuilder();

            RegisterEntityTypes(modelBuilder, allowedEntityTypes);

            return CompileModel(modelBuilder);
        }

        private static DbCompiledModel CreateDecimalIdentityFieldModel()
        {
            DbModelBuilder modelBuilder = new DbModelBuilder();

            modelBuilder.Entity<DecimalIdentityFieldEntity>().Property(e => e.Id)
                .HasPrecision(16, 0)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            return CompileModel(modelBuilder);
        }

        private static DbCompiledModel CreateIndexedFieldModel()
        {
            DbModelBuilder modelBuilder = new DbModelBuilder();

#if EF61
            modelBuilder.Entity<IndexedFieldEntity>();
#endif

            return CompileModel(modelBuilder);
        }

        private static void RegisterEntityTypes(
            DbModelBuilder modelBuilder, 
            params Type[] allowedEntityTypes)
        {
            MethodInfo entityMethod = GetDbModelBuilderGenericEntityMethod();

            if (allowedEntityTypes == null || allowedEntityTypes.Length == 0)
            {
                // All entities are allowed
                allowedEntityTypes = entityTypes;
            }

            MethodInfo method = GetDbModelBuilderGenericEntityMethod();

            foreach (Type entityType in allowedEntityTypes)
            {
                method.MakeGenericMethod(entityType).Invoke(modelBuilder, new object[0]);
            }
        }

        private static DbCompiledModel CompileModel(DbModelBuilder modelBuilder)
        {
            DbProviderInfo providerInfo =
                new DbProviderInfo(
                    Effort.Provider.EffortProviderConfiguration.ProviderInvariantName,
                    Effort.Provider.EffortProviderManifestTokens.Version1);

            return modelBuilder.Build(providerInfo).Compile();
        }

        private static MethodInfo GetDbModelBuilderGenericEntityMethod()
        {
            Expression<Action<DbModelBuilder>> expression = x => x.Entity<object>();
            MethodCallExpression methodCall = expression.Body as MethodCallExpression;
            return methodCall.Method.GetGenericMethodDefinition();
        }
    }
}
