// --------------------------------------------------------------------------------------------
// <copyright file="DefaultTypeConverter.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif

    internal class DefaultTypeConverter : ITypeConverter
    {
        public object ConvertClrObject(object obj, Type type)
        {
            if (obj is DBNull)
            {
                return null;
            }

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

        public bool TryConvertEdmType(PrimitiveType primitiveType, FacetInfo facets, out Type result)
        {
            result = null;

            if (string.Equals("binary", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals("image", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                result = typeof(NMemory.Data.Binary);
                return true;
            }

            if (string.Equals("rowversion", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase)
                || string.Equals("timestamp", primitiveType.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                result = typeof(NMemory.Data.Timestamp);
                return true;
            }

            return false;
        }
    }
}
