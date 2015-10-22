// --------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Effort Team">
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

namespace Effort.Internal.Common
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TypeHelper
    {
        public static Type GetEnumerableInterfaceTypeDefinition(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    return type;
                }
            }

            return type
                .GetInterfaces()
                .FirstOrDefault(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static Type GetElementType(Type type)
        {
            if (type.IsArray)
            {
                return type.GetElementType();
            }

            Type enumerableInterface = GetEnumerableInterfaceTypeDefinition(type);

            if (enumerableInterface == null)
            {
                return null;
            }

            return enumerableInterface.GetGenericArguments()[0];
        }

        public static bool IsNullable(Type type)
        {
            return 
                !type.IsValueType || 
                (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
        }

        public static Type MakeNullable(Type type)
        {
            if (!IsNullable(type))
            {
                return typeof(Nullable<>).MakeGenericType(type);
            }
            else
            {
                return type;
            }
        }

        public static Type MakeNotNullable(Type type)
        {
            if (!IsNullable(type))
            {
                return type;
            }
            else
            {
                return type.GetGenericArguments()[0];
            }
        }

        public static bool IsCastableTo(Type from, Type to)
        {
            if (to.IsAssignableFrom(from))
            {
                return true;
            }

            var methods = from
                .GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m =>
                    m.ReturnType == to &&
                    (m.Name == "op_Implicit" || m.Name == "op_Explicit"));

            return methods.Count() > 0;
        }

        public static Type GetDelegateReturnType(Type type)
        {
            if (!typeof(Delegate).IsAssignableFrom(type))
            {
                return null;
            }

            return type.GetMethod("Invoke").ReturnType;
        }

        public static string NormalizeForCliTypeName(string name)
        {
            return name.Replace("_", "__").Replace(".", "_");
        }

        public static Type GetMemberType(MemberInfo member)
        {
            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Event:
                    return ((EventInfo)member).EventHandlerType;
                default:
                    throw new ArgumentException("MemberInfo must be if type FieldInfo, PropertyInfo or EventInfo", "member");
            }
        }

        public static bool IsNumeric(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }
    }
}
