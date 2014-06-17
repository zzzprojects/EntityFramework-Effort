// --------------------------------------------------------------------------------------------
// <copyright file="CanonicalContainer.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.DbManagement
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Text;
    using Effort.Internal.Common;
    using Effort.Internal.DbManagement.Schema.Configuration;
    using Effort.Internal.TypeConversion;

    internal class CanonicalContainer
    {
        private readonly EdmTypeConverter converter;
        private readonly List<EntityContainer> containers;
        private readonly Lazy<ReadOnlyCollection<EntityInfo>> entities;
        private readonly Lazy<ReadOnlyCollection<AssociationInfo>> associations;

        public CanonicalContainer(ItemCollection source, EdmTypeConverter converter)
        {
            this.converter = converter;
            this.containers = source.GetItems<EntityContainer>().ToList();

            this.entities = new Lazy<ReadOnlyCollection<EntityInfo>>(() =>
                this.GetEntities()
                    .ToList()
                    .AsReadOnly());

            this.associations = new Lazy<ReadOnlyCollection<AssociationInfo>>(() =>
                this.GetAssociations()
                    .ToList()
                    .AsReadOnly());
        }

        public ReadOnlyCollection<EntityInfo> Entities
        {
            get { return this.entities.Value; }
        }

        public ReadOnlyCollection<AssociationInfo> Associations
        {
            get { return this.associations.Value; }
        }

        private IEnumerable<EntityInfo> GetEntities()
        {
            var groups = this.containers
                .SelectMany(x => x.BaseEntitySets.OfType<EntitySet>())
                .GroupBy(x => x.GetTableName());

            foreach (var group in groups)
            {
                var tableName = group.First().GetTableName();

                var properties = group.SelectMany(x => x.ElementType.Properties);

                var keyProperties = group.First()
                    .ElementType
                    .KeyMembers
                    .Select(x => x.GetColumnName())
                    .ToArray();

                yield return new EntityInfo(
                    tableName,
                    this.GetProperties(properties, converter),
                    keyProperties);
            }
        }

        private IEnumerable<EntityPropertyInfo> GetProperties(
            IEnumerable<EdmProperty> props,
            EdmTypeConverter converter)
        {
            var groups = props.GroupBy(x => x.GetColumnName());

            foreach (var group in groups)
            {
                var prop = group.First();

                var name = prop.GetColumnName();
                var facets = converter.GetTypeFacets(prop.TypeUsage);
                var clrType = converter.Convert(prop.TypeUsage);
                var indexes = GetIndexes(prop).ToList();

                // TODO: verify conflict

                yield return new EntityPropertyInfo(name, clrType, facets, indexes);
            }
        }

        private static IEnumerable<IndexInfo> GetIndexes(EdmProperty prop)
        {
            var indexMetadata = prop.MetadataProperties
                .FirstOrDefault(x => x.Name == "http://schemas.microsoft.com/ado/2013/11/edm/customannotation:Index");

            if (indexMetadata == null)
            {
                yield break;
            }

            // Use dynamic in order to not force the need of EF6.1
            dynamic indexAnnotation = indexMetadata.Value;

            foreach (dynamic index in indexAnnotation.Indexes)
            {
                yield return GetIndexInfo(index);
            }
        }

        private static IndexInfo GetIndexInfo(dynamic index)
        {
            return new IndexInfo(index.Name, index.Order, index.IsUnique);
        }

        private IEnumerable<AssociationInfo> GetAssociations()
        {
            var groups = this.containers
                .SelectMany(x => x.BaseEntitySets.OfType<AssociationSet>())
                .GroupBy(x => x.Name);

            foreach (var group in groups)
            {
                var association = group.First();

                // TODO: verify conflict

                if (association.ElementType.ReferentialConstraints.Count != 1)
                {
                    continue;
                }

                var constraint = association.ElementType.ReferentialConstraints[0];

                var primaryTable = this.GetTable(association, constraint.FromRole);
                var foreignTable = this.GetTable(association, constraint.ToRole);

                var primaryProps = this.GetPropertyNames(constraint.FromProperties);
                var foreignProps = this.GetPropertyNames(constraint.ToProperties);

                bool cascadedDelete = association
                    .AssociationSetEnds[constraint.FromRole.Name]
                    .CorrespondingAssociationEndMember
                    .DeleteBehavior == OperationAction.Cascade;

                yield return new AssociationInfo(
                    new AssociationTableInfo(primaryTable, primaryProps),
                    new AssociationTableInfo(foreignTable, foreignProps),
                    cascadedDelete);
            }
        }

        private string[] GetPropertyNames(IEnumerable<EdmProperty> properties)
        {
            return properties
                .Select(x => x.GetColumnName())
                .ToArray();
        }

        private string GetTable(
            AssociationSet association,
            RelationshipEndMember relationEndpoint)
        {
  
            RefType refType = relationEndpoint.TypeUsage.EdmType as RefType;

            return association
                .AssociationSetEnds
                .Select(x => x.EntitySet)
                .First(x => x.ElementType == refType.ElementType)
                .GetTableName();
        }
    }
}
