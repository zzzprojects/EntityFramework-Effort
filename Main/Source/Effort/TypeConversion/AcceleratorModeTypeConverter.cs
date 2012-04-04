using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Metadata.Edm;

namespace Effort.TypeConversion
{
    internal class AcceleratorModeTypeConverter : TypeConverterBase
    {
        public override object ConvertClrValueToClrValue(object value, Type expectedType)
        {
            // Implicit conversion does not work without explicit types
            if (expectedType == typeof(NMemory.Data.Binary) && value != null)
            {
                return (NMemory.Data.Binary)(byte[])value;
            }

            return base.ConvertClrValueToClrValue(value, expectedType);
        }

        public override Type ConvertPrimitiveEdmTypeToClrType(Type currentType, PrimitiveType edmType, TypeFacets facets)
        {
            if (edmType.BaseType != null && edmType.BaseType.Name == "Edm.Binary")
            {
                return typeof(NMemory.Data.Binary);
            }

            return base.ConvertPrimitiveEdmTypeToClrType(currentType, edmType, facets);
        }
    }
}
