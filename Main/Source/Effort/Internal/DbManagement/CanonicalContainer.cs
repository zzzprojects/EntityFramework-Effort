// --------------------------------------------------------------------------------------------
// <copyright file="MergedEntityContainer.cs" company="Effort Team">
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
    using System.Data.Entity.Core.Metadata.Edm;
    using System.Linq;
    using System.Text;
    using Effort.Internal.Common;
    using Effort.Internal.DbManagement.Schema.Configuration;
    using Effort.Internal.TypeConversion;

    internal class CanonicalContainer
    {
        private readonly IEnumerable<EntityContainer> containers;

        public CanonicalContainer(ItemCollection source)
        {
            this.containers = source.GetItems<EntityContainer>();
        }

        public IEnumerable<EntityInfo> GetEntities(EdmTypeConverter converter)
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

                // TODO: verify conflict

                yield return new EntityPropertyInfo(name, clrType, facets);
            }
        }

        public IEnumerable<AssociationInfo> GetAssociations()
        {
            var groups = this.containers
                .SelectMany(x => x.BaseEntitySets.OfType<AssociationSet>())
                .GroupBy(x => x.Name);

            foreach (var group in groups)
            {
                var association = group.First();

                AssociationInfo associationInfo = null;

                if (AssociationInfo.Create(association, out associationInfo))
                {
                    yield return associationInfo;
                }
            }
        }
    }
}
