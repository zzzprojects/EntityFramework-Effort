#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Data.Metadata.Edm;

namespace Effort.TypeConversion
{
    internal class EmulatorModeTypeConverter : TypeConverterBase
    {
        public override object ConvertClrValueToClrValue(object value, Type expectedType)
        {
            if (value != null && expectedType == typeof(NMemory.Data.Timestamp))
            {
                return NMemory.Data.Timestamp.FromBytes((byte[])value);
            }

            // Implicit conversion does not work without explicit types
            if (value != null && expectedType == typeof(NMemory.Data.Binary))
            {
                return (NMemory.Data.Binary)(byte[])value;
            }

            return base.ConvertClrValueToClrValue(value, expectedType);
        }

        public override object ConvertClrValueFromClrValue(object value)
        {
            if (value != null && value.GetType() == typeof(NMemory.Data.Timestamp))
            {
                return NMemory.Data.Timestamp.GetBytes((NMemory.Data.Timestamp)value);
            }

            if (value != null && value.GetType() == typeof(NMemory.Data.Binary))
            {
                return (byte[])(NMemory.Data.Binary)value;
            }

            return base.ConvertClrValueFromClrValue(value);
        }

        public override Type ConvertPrimitiveEdmTypeToClrType(Type currentType, PrimitiveType edmType, TypeFacets facets)
        {
            if (EdmTypeHelper.IsTimestamp(edmType))
            {
                return typeof(NMemory.Data.Timestamp);
            }

            if (EdmTypeHelper.IsBinary(edmType))
            {
                return typeof(NMemory.Data.Binary);
            }

            return base.ConvertPrimitiveEdmTypeToClrType(currentType, edmType, facets);
        }
    }
}
