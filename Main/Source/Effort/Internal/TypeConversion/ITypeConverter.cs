using System;
using System.Data.Metadata.Edm;

namespace Effort.Internal.TypeConversion
{
    internal interface ITypeConverter
    {
        object ConvertClrObject(object obj, Type type);

        bool TryConvertEdmType(PrimitiveType primitiveType, TypeFacets facets, out Type result);
    }
}
