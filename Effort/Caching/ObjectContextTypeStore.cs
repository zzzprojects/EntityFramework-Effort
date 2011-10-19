using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Effort.Caching
{
    public class ObjectContextTypeStore
    {
        private static ConcurrentCache<ConnectionStringKey, Type> accelerators;
        private static ConcurrentCache<EmulatorTypeKey, Type> emulators;

        static ObjectContextTypeStore()
        {
            accelerators = new ConcurrentCache<ConnectionStringKey, Type>();
            emulators = new ConcurrentCache<EmulatorTypeKey, Type>();
        }

        public static Type GetAccelerator(string connectionString, Func<Type> typeFactoryMethod)
        {
            return accelerators.Get(new ConnectionStringKey(connectionString), typeFactoryMethod);
        }

        public static Type GetEmulator(string connectionString, string source, bool shared, Func<Type> typeFactoryMethod)
        {
            return emulators.Get(new EmulatorTypeKey(connectionString, source, shared), typeFactoryMethod);
        }
    }
}
