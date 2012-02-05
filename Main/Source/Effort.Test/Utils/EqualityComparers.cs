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
using System.Text;
using System.Collections;

namespace Effort.Test.Utils
{
    public static class EqualityComparers
    {
        private static Dictionary<Type, IEqualityComparer> comparers = new Dictionary<Type, IEqualityComparer>();

        public static IEqualityComparer Create(Type type)
        {
            IEqualityComparer res;

            if (!comparers.TryGetValue(type, out res))
            {
                res = CreateCore(type);
                comparers.Add(type, res);
            }

            return res;
        }


        private static IEqualityComparer CreateCore(Type type)
        {
            if (type == typeof(double) ||
               type == typeof(decimal) ||
               type == typeof(float) ||
               type == typeof(double?) ||
               type == typeof(decimal?) ||
               type == typeof(float?))
            {
                return new NumberComparer();
            }

            if (type == typeof(string) ||
                type.IsValueType)
            {
                return new BasicComparer();
            }

            if (type.IsArray ||IsEnumerable(type))
            {
                return CreateCollectionComparer(type) as IEqualityComparer;
            }

            return new EntityComparer();
        }

        private static bool IsEnumerable(Type type)
        {
            return type
                .GetInterfaces()
                .Where(i =>
                    i.GetGenericTypeDefinition() ==
                    typeof(IEnumerable<>))
                .Any();
        }

        private static object CreateCollectionComparer(Type type)
        {
            Type elementType = null;

            if (type.IsArray)
            {
                elementType = type.GetElementType();
            }
            else
            {
                elementType = 
                    type
                    .GetInterfaces()
                    .Where(i => 
                        i.GetGenericTypeDefinition() ==
                        typeof(IEnumerable<>))
                    .FirstOrDefault()
                    .GetGenericArguments()[0];
            }

            Type comparer = typeof(CollectionComparer<>).MakeGenericType(elementType);

            if (elementType == typeof(byte) && type.IsArray)
            {
                // Binary array 

                return Activator.CreateInstance(comparer, new object[] { true });
            }
            

            return Activator.CreateInstance(comparer, new object[] { false });
        }


    }
}
