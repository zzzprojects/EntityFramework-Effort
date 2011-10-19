using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;
using Effort.TypeGeneration;

namespace Effort.DbCommandTreeTransform
{

    internal class EdmTypeConverter
    {

        public Type Convert(TypeUsage type)
        {
            TypeFacets facets = this.GetTypeFacets(type);
            return ConvertWithFacets(type, facets);
        }

        public Type ConvertNotNull(TypeUsage type)
        {
            TypeFacets facets = new TypeFacets();

            return ConvertWithFacets(type, facets);

        }
        public Type GetEncapsulatedType(TypeUsage type)
        {
            CollectionType collectionType = type.EdmType as CollectionType;

            if (collectionType == null)
            {
                throw new ArgumentException("type");
            }

            return this.Convert(collectionType.TypeUsage);
        }

        public TypeFacets GetTypeFacets(TypeUsage type)
        {
            TypeFacets facets = new TypeFacets();

            facets.Nullable = type.Facets.Any(f => f.Name == "Nullable" && (bool)f.Value == true);

            return facets;
        }





        private Type ConvertWithFacets( TypeUsage type, TypeFacets facets )
        {

            if( type.EdmType.BuiltInTypeKind == BuiltInTypeKind.PrimitiveType )
            {
                return this.CreatePrimitiveType( type.EdmType as PrimitiveType, facets );

            }
            else if( type.EdmType.BuiltInTypeKind == BuiltInTypeKind.RowType )
            {
                return this.CreateRowType( type.EdmType as RowType, facets );
            }
            else if( type.EdmType.BuiltInTypeKind == BuiltInTypeKind.CollectionType )
            {
                return this.CreateCollectionType( type.EdmType as CollectionType, facets );
            }


            throw new NotSupportedException();
        }



        private Type CreatePrimitiveType(PrimitiveType primitiveType, TypeFacets facets)
        {
            Type result = primitiveType.ClrEquivalentType;

            if (facets.Nullable && result.IsValueType)
            {
                result = typeof(Nullable<>).MakeGenericType(result);
            }

            return result;
        }

        private Type CreateRowType(RowType rowType, TypeFacets facets)
        {
            Dictionary<string, Type> members = new Dictionary<string, Type>();

            foreach (var item in rowType.Members)
            {
                members.Add(item.Name, this.Convert(item.TypeUsage));
            }

            Type result = AnonymousTypeFactory.Create(members);

            return result;
        }

        private Type CreateCollectionType( CollectionType cType, TypeFacets facets )
        {
            Type elementType = this.ConvertWithFacets( cType.TypeUsage, facets );

            return elementType.MakeArrayType();
        }
    }

    public struct TypeFacets
    {
        public bool Nullable { set; get; }
    }
}
