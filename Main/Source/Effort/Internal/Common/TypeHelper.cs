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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Effort.Internal.Common
{
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
            return !type.IsValueType || (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>));
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
                    m.Name == "op_Implicit" ||
                    m.Name == "op_Explicit");

            return methods.Count() > 0;
        }

    }
}
