using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MMDB.EntityFrameworkProvider.Helpers
{
    internal static class TypeHelper
    {
        public static Type GetEnumerableInterfaceTypeDefinition( Type type )
        {
            // zsvarnai: ez meg hianyzik
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
                .FirstOrDefault( t =>
                    t.IsGenericType &&
                    t.GetGenericTypeDefinition() == typeof( IEnumerable<> ) );
        }

        public static Type GetEncapsulatedType( Type type )
        {
            Type enumerableInterface = GetEnumerableInterfaceTypeDefinition( type );

            if( enumerableInterface == null )
            {
                return null;
            }

            return enumerableInterface.GetGenericArguments()[0];
        }

        public static bool IsNullable( Type type )
        {
            return !type.IsValueType || ( type.IsGenericType && type.GetGenericTypeDefinition() == typeof( Nullable<> ) );
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

    }
}
