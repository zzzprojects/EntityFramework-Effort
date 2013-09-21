// --------------------------------------------------------------------------------------------
// <copyright file="TypeHelper.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

        public static bool IsAnonymousType(Type type)
        {
            return CheckIfAnonymousType(type)
                ||
                (!typeof(IComparable).IsAssignableFrom(type) &&
                type.GetCustomAttributes(typeof(DebuggerDisplayAttribute), false)
                    .Cast<DebuggerDisplayAttribute>()
                    .Any(m => m.Type == "<Anonymous Type>"));
        }

        private static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
            {
                return false;
            }

            // HACK: The only way to detect anonymous types right now.
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType 
                && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>") || type.Name.StartsWith("VB$"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        public static Type GetTupleType(params Type[] memberTypes)
        {
            Type generic = null;

            switch (memberTypes.Length)
            {
                case 1:
                    generic = typeof(Tuple<>);
                    break;
                case 2:
                    generic = typeof(Tuple<,>);
                    break;
                case 3:
                    generic = typeof(Tuple<,,>);
                    break;
                case 4:
                    generic = typeof(Tuple<,,,>);
                    break;
                case 5:
                    generic = typeof(Tuple<,,,,>);
                    break;
                case 6:
                    generic = typeof(Tuple<,,,,,>);
                    break;
                case 7:
                    generic = typeof(Tuple<,,,,,,>);
                    break;
                case 8:
                    generic = typeof(Tuple<,,,,,,,>);
                    break;
                default:
                    throw new ArgumentException("memberTypes");
            }

            return generic.MakeGenericType(memberTypes);
        }
    }
}
