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
