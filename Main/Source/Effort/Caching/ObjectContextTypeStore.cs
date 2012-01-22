using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Caching
{
    internal class ObjectContextTypeStore
    {
        private static ConcurrentCache<ObjectContextTypeKey, Type> accelerators;
        private static ConcurrentCache<ObjectContextTypeKey, Type> emulators;

        static ObjectContextTypeStore()
        {
            accelerators = new ConcurrentCache<ObjectContextTypeKey, Type>();
            emulators = new ConcurrentCache<ObjectContextTypeKey, Type>();
        }

        public static Type GetAccelerator(string connectionString, Type contextType, Func<Type> typeFactoryMethod)
        {
            return accelerators.Get(
                new ObjectContextTypeKey(connectionString, contextType, string.Empty, false), 
                typeFactoryMethod);
        }

        public static Type GetEmulator(string connectionString, Type contextType, string source, bool shared, Func<Type> typeFactoryMethod)
        {
            return emulators.Get(
                new ObjectContextTypeKey(connectionString, contextType, source, shared), 
                typeFactoryMethod);
        }
    }
}
