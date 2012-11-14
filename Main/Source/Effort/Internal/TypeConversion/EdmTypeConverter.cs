// --------------------------------------------------------------------------------------------
// <copyright file="EdmTypeConverter.cs" company="Effort Team">
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

namespace Effort.Internal.TypeConversion
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
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
            FacetInformation facets = this.GetTypeFacets(type);
            return this.ConvertWithFacets(type, facets);
        }

        public Type ConvertNotNull(TypeUsage type)
        {
            FacetInformation facets = new FacetInformation();

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

        public FacetInformation GetTypeFacets(TypeUsage type)
        {
            FacetInformation facets = new FacetInformation();
            Facet facet = null;
            
            if (type.Facets.TryGetValue("Nullable", false, out facet))
            {
                facets.Nullable = (bool)facet.Value == true;
            }

            if (type.Facets.TryGetValue("FixedLength", false, out facet))
            {
                if (((bool?)facet.Value).HasValue)
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
                if (((int?)facet.Value).HasValue)
                {
                    facets.MaxLenght = (int)facet.Value;
                    facets.LimitedLength = true;
                }
            }

            return facets;
        }

        private Type ConvertWithFacets(TypeUsage type, FacetInformation facets)
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

        private Type CreatePrimitiveType(PrimitiveType primitiveType, FacetInformation facets)
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

        private Type CreateRowType(RowType rowType, FacetInformation facets)
        {
            Dictionary<string, Type> members = new Dictionary<string, Type>();

            foreach (var item in rowType.Members)
            {
                members.Add(item.Name, this.Convert(item.TypeUsage));
            }

            Type result = AnonymousTypeFactory.Create(members);

            return result;
        }

        private Type CreateCollectionType(CollectionType collectionType, FacetInformation facets)
        {
            Type elementType = this.ConvertWithFacets(collectionType.TypeUsage, facets);

            return elementType.MakeArrayType();
        }
    }
}