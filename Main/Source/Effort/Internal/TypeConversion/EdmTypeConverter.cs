// --------------------------------------------------------------------------------------------
// <copyright file="EdmTypeConverter.cs" company="Effort Team">
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

namespace Effort.Internal.TypeConversion
{
    using System;
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using Effort.Internal.Common;
    using Effort.Internal.TypeGeneration;

    internal class EdmTypeConverter
    {
        private ITypeConverter converter;

        public EdmTypeConverter(ITypeConverter converter)
        {
            this.converter = converter;
        }

        public Type Convert(TypeUsage type)
        {
            FacetInfo facets = this.GetTypeFacets(type);
            return this.ConvertWithFacets(type, facets);
        }

        public Type ConvertNotNull(TypeUsage type)
        {
            FacetInfo facets = new FacetInfo();

            return this.ConvertWithFacets(type, facets);
        }

        public Type GetElementType(TypeUsage type)
        {
            CollectionType collectionType = type.EdmType as CollectionType;

            if (collectionType == null)
            {
                throw new ArgumentException("type");
            }

            return this.Convert(collectionType.TypeUsage);
        }

        public FacetInfo GetTypeFacets(TypeUsage type)
        {
            FacetInfo facets = new FacetInfo();
            Facet facet = null;
            
            if (type.Facets.TryGetValue("Nullable", false, out facet))
            {
                facets.Nullable = (bool)facet.Value == true;
            }

            if (type.Facets.TryGetValue("FixedLength", false, out facet))
            {
                if (!facet.IsUnbounded && facet.Value != null)
                {
                    facets.FixedLength = (bool)facet.Value == true;
                }
            }

            if (type.Facets.TryGetValue("StoreGeneratedPattern", false, out facet))
            {
                switch ((StoreGeneratedPattern)facet.Value)
                {
                    case StoreGeneratedPattern.Computed:
                        facets.Computed = true;
                        break;
                    case StoreGeneratedPattern.Identity:
                        facets.Identity = true;
                        break;
                }
            }

            if (type.Facets.TryGetValue("MaxLength", false, out facet))
            {
                if (facet.IsUnbounded)
                {
                    facets.LimitedLength = false;
                }
                else if (facet.Value != null)
                {
                    facets.MaxLenght = (int)facet.Value;
                    facets.LimitedLength = true;
                }
            }

            return facets;
        }

        private Type ConvertWithFacets(TypeUsage type, FacetInfo facets)
        {
            if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType)
            {
                return this.CreatePrimitiveType(type.EdmType as PrimitiveType, facets);
            }
            else if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.RowType)
            {
                return this.CreateRowType(type.EdmType as RowType, facets);
            }
            else if (type.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType)
            {
                return this.CreateCollectionType(type.EdmType as CollectionType, facets);
            }

            throw new NotSupportedException();
        }

        private Type CreatePrimitiveType(PrimitiveType primitiveType, FacetInfo facets)
        {
            Type result = null;
            if (this.converter.TryConvertEdmType(primitiveType, facets, out result))
            {
                return result;
            }
            
            result = primitiveType.ClrEquivalentType;

            if (facets.Nullable && result.IsValueType)
            {
                result = typeof(Nullable<>).MakeGenericType(result);
            }

            return result;
        }

        private Type CreateRowType(RowType rowType, FacetInfo facets)
        {
            Dictionary<string, Type> members = new Dictionary<string, Type>();

            foreach (EdmMember member in rowType.Members)
            {
                members.Add(member.GetColumnName(), this.Convert(member.TypeUsage));
            }

            Type result = DataRowFactory.Create(members);

            return result;
        }

        private Type CreateCollectionType(CollectionType collectionType, FacetInfo facets)
        {
            Type elementType = this.ConvertWithFacets(collectionType.TypeUsage, facets);

            return elementType.MakeArrayType();
        }
    }
}