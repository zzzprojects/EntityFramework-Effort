using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Effort.Test.Utils
{
    internal static class TypeHelper
    {
        public static Type GetCollectionInterfaceTypeDefinition(Type type)
        {
            if (type.IsInterface)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == typeof(ICollection<>))
                {
                    return type;
                }
            }

            return type
                .GetInterfaces()
                .FirstOrDefault(t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof(ICollection<>));
        }
    }
}
