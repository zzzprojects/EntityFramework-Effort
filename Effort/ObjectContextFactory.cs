using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.EntityClient;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Effort.Helpers;
using System.Linq.Expressions;

namespace Effort
{
    public static class ObjectContextFactory
    {
        private static ModuleBuilder objectContextContainer;
        private static int objectContextCounter;

        static ObjectContextFactory()
        {
            DatabaseEmulatorProviderConfiguration.RegisterProvider();
            DatabaseAcceleratorProviderConfiguration.RegisterProvider();

            // Dynamic Library for MMDB
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("DynamicObjectContextLib")),
                    AssemblyBuilderAccess.Run);

            // Module for the entity types
            objectContextContainer = assembly.DefineDynamicModule("ObjectContexts");
            objectContextCounter = 0;
        }

        #region Type factory methods

        public static Type CreateEmulator<T>(string source, bool shared) where T : ObjectContext
        {
            string connectionString = GetDefaultConnectionString<T>();

            return CreateEmulator<T>(connectionString, source, shared);
        }

        public static Type CreateEmulator<T>(string connectionString, string source, bool shared) where T : ObjectContext
        {
            // Get method info
            MethodInfo entityConnectionFactory = ReflectionHelper.GetMethodInfo<object>(a => 

                EntityConnectionFactory.CreateEmulator(string.Empty, string.Empty, false));

            Action<ILGenerator> factoryArguments = gen =>
                {
                    gen.Emit(OpCodes.Ldstr, connectionString);
                    gen.Emit(OpCodes.Ldstr, source);
                    gen.Emit(shared ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);
                };

            return CreateType<T>(entityConnectionFactory, factoryArguments);

        }


        private static Type CreateType<T>(MethodInfo entityConnectionFactoryMethod, Action<ILGenerator> methodArgs)
        {
            TypeBuilder builder = null;

            lock (objectContextContainer)
            {
                objectContextCounter++;
                builder = objectContextContainer.DefineType(
                    string.Format("DynamicObjectContext{0}", objectContextCounter), 
                    TypeAttributes.Public,
                    typeof(T));
            }

            ConstructorBuilder ctor = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] {});

            ConstructorInfo baseCtor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(EntityConnection) },
                null
                );

            // Adding parameters
            ILGenerator gen = ctor.GetILGenerator();
            // Writing body
            gen.Emit(OpCodes.Ldarg_0);

            methodArgs.Invoke(gen);

            gen.Emit(OpCodes.Call, entityConnectionFactoryMethod);
            gen.Emit(OpCodes.Call, baseCtor);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            return builder.CreateType();
        }

        #endregion

        #region Instance factory methods

        public static T CreateAcceleratorInstance<T>() where T : ObjectContext
        {
            string connectionString = GetDefaultConnectionString<T>();

            return CreateAcceleratorInstance<T>(connectionString);
        }

        public static T CreateAcceleratorInstance<T>(string connectionString) where T : ObjectContext
        {
            EntityConnection conn = EntityConnectionFactory.CreateAccelerator(connectionString);

            return Activator.CreateInstance(typeof(T), new object[] { conn }) as T;
        }


        public static T CreateEmulatorInstance<T>(string source, bool shared) where T : ObjectContext
        {
            string connectionString = GetDefaultConnectionString<T>();

            return CreateEmulatorInstance<T>(connectionString, source, shared);
        }

        public static T CreateEmulatorInstance<T>(string connectionString, string source, bool shared) where T : ObjectContext
        {
            EntityConnection conn = EntityConnectionFactory.CreateEmulator(connectionString, source, shared);

            return Activator.CreateInstance(typeof(T), new object[] { conn }) as T;
        }

        #endregion

        private static string GetDefaultConnectionString<T>() where T : ObjectContext
        {
            return Activator.CreateInstance<T>().Connection.ConnectionString;
        }
    }
}
