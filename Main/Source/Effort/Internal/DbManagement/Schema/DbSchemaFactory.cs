// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaFactory.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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

namespace Effort.Internal.DbManagement.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.Internal.Common;
    using Effort.Internal.TypeConversion;
    using Effort.Internal.TypeGeneration;
    using NMemory.Indexes;
    using NMemory.Tables;
    using Effort.Internal.DbManagement.Engine;
    using NMemory.Constraints;

    internal static class DbSchemaFactory
    {
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
                string name = entitySet.GetTableName();
                TypeBuilder entityTypeBuilder = entityModule.DefineType(name, TypeAttributes.Public);

                List<PropertyInfo> primaryKeyFields = new List<PropertyInfo>();
                List<PropertyInfo> identityFields = new List<PropertyInfo>();
                List<PropertyInfo> properties = new List<PropertyInfo>();

                List<string> primaryKeyFieldNames = new List<string>();
                List<string> identityFieldNames = new List<string>();
                List<string> notNullableFields = new List<string>();
                List<string> generatedGuidFields = new List<string>();
                Dictionary<string, int> maxLenghtNVarcharFields = new Dictionary<string, int>();
                Dictionary<string, int> maxLenghtNCharFields = new Dictionary<string, int>();

                // Add properties as entity fields
                foreach (EdmProperty field in type.Properties)
                {
                    FacetInformation facets = typeConverter.GetTypeFacets(field.TypeUsage);
                    string fieldName = field.GetColumnName();
                    Type fieldType = typeConverter.Convert(field.TypeUsage);

                    PropertyBuilder propBuilder =
                        EmitHelper.AddProperty(entityTypeBuilder, fieldName, fieldType);

                    // Register primary key field
                    if (type.KeyMembers.Contains(field))
                    {
                        primaryKeyFieldNames.Add(propBuilder.Name);
                    }

                    // Register identity field
                    if (facets.Identity && typeof(long).IsAssignableFrom(fieldType))
                    {
                        identityFieldNames.Add(propBuilder.Name);
                    }

                    if (facets.Identity && fieldType == typeof(Guid))
                    {
                        generatedGuidFields.Add(propBuilder.Name);
                    }

                    // Register nullable field
                    if (facets.Nullable == false)
                    {
                        notNullableFields.Add(propBuilder.Name);
                    }

                    // Register nchar(x)
                    if (facets.LimitedLength && field.TypeUsage.EdmType.Name == "string" && facets.FixedLength)
                    {
                        maxLenghtNCharFields.Add(propBuilder.Name, facets.MaxLenght);
                    }

                    // Register nvarchar(x) 
                    if (facets.LimitedLength && field.TypeUsage.EdmType.Name == "string")
                    {
                        maxLenghtNVarcharFields.Add(propBuilder.Name, facets.MaxLenght);
                    }
                }

                if (identityFields.Count > 1)
                {
                    throw new InvalidOperationException("More than one identity fields is not supported");
                }

                Type entityType = entityTypeBuilder.CreateType();

                List<object> constraint = new List<object>();

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
                        constraint.Add(
                            typeof(NotNullableConstraint<>).MakeGenericType(entityType).GetConstructors().First().Invoke(
                            new object[] 
                            {
                                Expression.Lambda(Expression.Convert(Expression.PropertyOrField(param, prop.Name), typeof(object)), param)
                            }));
                    }

                    if (maxLenghtNVarcharFields.Keys.Contains(prop.Name))
                    {
                        var param = Expression.Parameter(entityType, "x");
                        constraint.Add(
                            typeof(NVarCharConstraint<>).MakeGenericType(entityType).GetConstructors().First().Invoke(
                            new object[] 
                            {
                                Expression.Lambda(Expression.PropertyOrField(param, prop.Name), param), maxLenghtNVarcharFields[prop.Name]
                            }));
                    }

                    if (maxLenghtNCharFields.Keys.Contains(prop.Name))
                    {
                        var param = Expression.Parameter(entityType, "x");
                        constraint.Add(
                            typeof(NCharConstraint<>).MakeGenericType(entityType).GetConstructors().First().Invoke(
                            new object[] 
                            {
                                Expression.Lambda(Expression.PropertyOrField(param, prop.Name), param), maxLenghtNCharFields[prop.Name]
                            }));
                    }

                    if (generatedGuidFields.Contains(prop.Name))
                    {
                        var param = Expression.Parameter(entityType, "x");
                        constraint.Add(
                            typeof(GeneratedGuidConstraint<>).MakeGenericType(entityType).GetConstructors().First().Invoke(
                            new object[] 
                            {
                                    Expression.Lambda(Expression.PropertyOrField(param, prop.Name), param)
                            }));
                    }
                }

                LambdaExpression primaryKeySelector = LambdaExpressionHelper.CreateSelectorExpression(entityType, primaryKeyFields.ToArray());
                Type primaryKeyAnonymType = primaryKeySelector.Body.Type;

                IKeyInfo primaryKeyInfo = CreateKeyInfo(primaryKeySelector);

                schema.RegisterTable(
                    new DbTableInformation(
                        entityType.Name,
                        entityType,
                        primaryKeyFields.ToArray(),
                        identityFields.SingleOrDefault(),
                        properties.ToArray(),
                        constraint.ToArray(),
                        primaryKeyInfo));
            }

            foreach (AssociationSet associationSet in entityContainer.BaseEntitySets.OfType<AssociationSet>())
            {
                var constraints = associationSet.ElementType.ReferentialConstraints;

                if (constraints.Count != 1)
                {
                    continue;
                }

                ReferentialConstraint constraint = constraints[0];

                string fromTableName = GetTableName(constraint.FromRole, entityContainer);
                string toTableName = GetTableName(constraint.ToRole, entityContainer);

                // entityType is the primary table, toTable is the foreign table...
                DbTableInformation fromTable = schema.GetTable(fromTableName);
                DbTableInformation toTable = schema.GetTable(toTableName);

                // Creating the parameters for NMemory.Tables.RelationKeyConverterFactory
                PropertyInfo[] fromTableProperties = GetRelationProperties(constraint.FromProperties, fromTable);
                PropertyInfo[] toTableProperties = GetRelationProperties(constraint.ToProperties, toTable);

                // Create primary key info 
                LambdaExpression primaryKeySelector = LambdaExpressionHelper.CreateSelectorExpression(fromTable.EntityType, fromTableProperties);
                Type primaryKeyType = primaryKeySelector.Body.Type;
                IKeyInfo primaryKeyInfo = CreateKeyInfo(primaryKeySelector);

                // Create foreign key info
                LambdaExpression foreignKeySelector = LambdaExpressionHelper.CreateSelectorExpression(toTable.EntityType, toTableProperties);
                Type foreignKeyType = foreignKeySelector.Body.Type;
                IKeyInfo foreignKeyInfo = CreateKeyInfo(foreignKeySelector);

                // Create a IRelationContraints for defining relation mapping
                List<IRelationContraint> relationConstraints = new List<IRelationContraint>();

                for (int i = 0; i < fromTableProperties.Count(); i++)
                {
                    // Should not use the generic RelationConstraint here. 
                    relationConstraints.Add(new RelationConstraint(fromTableProperties[i], toTableProperties[i]));
                }

                Delegate primaryToForeignConverter =
                    ReflectionHelper.GetMethodInfo(() => 
                        RelationKeyConverterFactory.CreatePrimaryToForeignConverter<object, object>(null, null, null))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(primaryKeyType, foreignKeyType)
                    .Invoke(null, new object[] { primaryKeyInfo, foreignKeyInfo, relationConstraints.ToArray() }) as Delegate;

                Delegate foreignToPrimaryConverter =
                    ReflectionHelper.GetMethodInfo(() =>
                        RelationKeyConverterFactory.CreateForeignToPrimaryConverter<object, object>(null, null, null))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(primaryKeyType, foreignKeyType)
                    .Invoke(null, new object[] { primaryKeyInfo, foreignKeyInfo, relationConstraints.ToArray() }) as Delegate;

                schema.RegisterRelation(
                    new DbRelationInformation(
                        fromTableName, 
                        fromTableProperties, 
                        toTableName, 
                        toTableProperties, 
                        primaryKeyInfo, 
                        foreignKeyInfo, 
                        primaryToForeignConverter, 
                        foreignToPrimaryConverter));
            }

            return schema;
        }

        private static IKeyInfo CreateKeyInfo(LambdaExpression selector)
        {
            Type entityType = selector.Parameters[0].Type;
            Type resultType = selector.Body.Type;

            IKeyInfo result = 
                ReflectionHelper
                    .GetMethodInfo<DefaultKeyInfoFactory>(f => f.Create<object, object>(null))
                    .GetGenericMethodDefinition()
                    .MakeGenericMethod(entityType, resultType)
                    .Invoke(new DefaultKeyInfoFactory(), new object[] { selector }) as IKeyInfo;

            return result;
        }

        private static string GetTableName(RelationshipEndMember relationEndpoint, EntityContainer entityContainer)
        {
            RefType refType = relationEndpoint.TypeUsage.EdmType as RefType;
            return entityContainer.BaseEntitySets.First(m => m.ElementType.Name == refType.ElementType.Name).GetTableName();
        }

        private static PropertyInfo[] GetRelationProperties(ReadOnlyMetadataCollection<EdmProperty> properties, DbTableInformation table)
        {
            return properties.Select(edmp => table
                .Properties
                .Single(clrp => clrp.Name == edmp.GetColumnName()))
                .ToArray();
        }
    }
}
