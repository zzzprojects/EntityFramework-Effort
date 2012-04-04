using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;

namespace Effort.TypeConversion
{
    internal class TypeConverterBase : ITypeConverter
    {
        public virtual object ConvertClrValueToClrValue(object value, Type expectedType)
        {
            return value;
        }

        public virtual Type ConvertPrimitiveEdmTypeToClrType(Type currentType, PrimitiveType edmType, TypeFacets facets)
        {
            return currentType;
        }
    }
}
