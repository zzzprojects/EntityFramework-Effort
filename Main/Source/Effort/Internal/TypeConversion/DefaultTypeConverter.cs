using System;
using System.Data.Metadata.Edm;

namespace Effort.Internal.TypeConversion
{
    internal class DefaultTypeConverter : ITypeConverter
    {
        public object ConvertClrObject(object obj, Type type)
        {
            if (type == typeof(NMemory.Data.Binary))
            {
                return (NMemory.Data.Binary)(byte[])obj;
            }

            if (type == typeof(NMemory.Data.Timestamp))
            {
                return (NMemory.Data.Timestamp)(byte[])obj;
            }

            if (type == typeof(byte[]))
            {
                if (obj == null)
                {
                    return null;
                }

                Type actualType = obj.GetType();

                if (actualType == typeof(NMemory.Data.Binary))
                {
                    return (byte[])(NMemory.Data.Binary)obj;
                }

                if (actualType == typeof(NMemory.Data.Timestamp))
                {
                    return (byte[])(NMemory.Data.Timestamp)obj;
                }
            }

            return obj;
        }


        public bool TryConvertEdmType(PrimitiveType primitiveType, TypeFacets facets, out Type result)
        {
            result = null;

            if (string.Equals("binary", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                result = typeof(NMemory.Data.Binary);
                return true;
            }

            if (string.Equals("rowversion", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                result = typeof(NMemory.Data.Timestamp);
                return true;
            }

            return false;
        }
    }
}
