// --------------------------------------------------------------------------------------------
// <copyright file="RelationConfiguration.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

namespace Effort.Internal.DbManagement.Schema.Configuration
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.Common;
    using NMemory.Indexes;
    using NMemory.Tables;

    internal class RelationConfiguration : IRelationConfiguration
    {
        private static readonly MethodInfo PrimaryToForeignConverterMethod =
            ReflectionHelper.GetMethodInfo(() =>
                RelationKeyConverterFactory
                    .CreatePrimaryToForeignConverter<object, object>(
                        null, 
                        null, 
                        null));

        private static readonly MethodInfo ForeignToPrimaryConverterMethod =
            ReflectionHelper.GetMethodInfo(() =>
                RelationKeyConverterFactory
                    .CreateForeignToPrimaryConverter<object, object>(
                        null,
                        null,
                        null));

        public void Configure(AssociationInfo associationInfo, DbSchemaBuilder builder)
        {
            string primaryTableName = associationInfo.PrimaryTable.GetTableName();
            string foreignTableName = associationInfo.ForeignTable.GetTableName();

            DbTableInfoBuilder primaryTable = builder.Find(primaryTableName);
            DbTableInfoBuilder foreignTable = builder.Find(foreignTableName);

            MemberInfo[] primaryKeyMembers = 
                GetMembers(
                    primaryTable.EntityType, 
                    associationInfo.PrimaryKeyMembers);

            MemberInfo[] foreignKeyMembers =
                GetMembers(
                    foreignTable.EntityType,
                    associationInfo.ForeignKeyMembers);

            IKeyInfo primaryKeyInfo = 
                EnsureKey(
                    primaryKeyMembers,
                    true,
                    primaryTable);

            IKeyInfo foreignKeyInfo = 
                EnsureKey(
                    foreignKeyMembers,
                    false,
                    foreignTable);

            IRelationContraint[] relationConstraints =
                new IRelationContraint[primaryKeyMembers.Length];

            for (int i = 0; i < relationConstraints.Length; i++)
            {
                relationConstraints[i] =
                    new RelationConstraint(primaryKeyMembers[i], foreignKeyMembers[i]);
            }

            Delegate primaryToForeignConverter =
                CreateConverter(
                    PrimaryToForeignConverterMethod,
                    primaryKeyInfo,
                    foreignKeyInfo,
                    relationConstraints);

            Delegate foreignToPrimaryConverter =
                CreateConverter(
                    ForeignToPrimaryConverterMethod,
                    primaryKeyInfo,
                    foreignKeyInfo,
                    relationConstraints);

            builder.Register(
                new DbRelationInfo(
                    primaryTable:               primaryTableName,
                    primaryKeyInfo:             primaryKeyInfo,
                    primaryToForeignConverter:  primaryToForeignConverter,
                    foreignTable:               foreignTableName,
                    foreignKeyInfo:             foreignKeyInfo,
                    foreignToPrimaryConverter:  foreignToPrimaryConverter)); 
        }

        public static IKeyInfo EnsureKey(
            MemberInfo[] members, 
            bool unique,
            DbTableInfoBuilder tableBuilder)
        {
            IKeyInfo keyInfo = tableBuilder.FindKey(members, false, unique);

            if (keyInfo == null)
            {
                keyInfo = KeyInfoHelper.CreateKeyInfo(tableBuilder.EntityType, members);
                tableBuilder.AddKey(keyInfo, unique);
            }

            return keyInfo;
        }

        private static MemberInfo[] GetMembers(
            Type entityType,
            ICollection<EdmProperty> properties)
        {
            return properties.Select(edmp => entityType
                .GetProperties()
                .Single(clrp => clrp.Name == edmp.GetColumnName()))
                .ToArray();
        }

        private static Delegate CreateConverter(
            MethodInfo converterMethod,
            IKeyInfo primaryKeyInfo,
            IKeyInfo foreignKeyInfo,
            IRelationContraint[] relationConstraints)
        {
            MethodInfo factory = converterMethod
                .GetGenericMethodDefinition()
                .MakeGenericMethod(primaryKeyInfo.KeyType, foreignKeyInfo.KeyType);

            Delegate result = factory.Invoke(
                null,
                new object[] 
                { 
                    primaryKeyInfo, 
                    foreignKeyInfo, 
                    relationConstraints.ToArray() 
                }) as Delegate;

            return result;
        }
    }
}
