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
using System.ComponentModel;
using System.Data.EntityClient;
using System.Data.Objects;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Effort.Caching;
using Effort.Helpers;

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
            return CreateEmulator<T>(null, source, shared);
        }

        public static Type CreateEmulator<T>(string connectionString, string source, bool shared) where T : ObjectContext
        {
            return ObjectContextTypeStore.GetEmulator(connectionString, typeof(T), source, shared, () =>
                {
                    if (connectionString == null)
                    {
                        connectionString = GetDefaultConnectionString<T>();
                    }

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
                });
        }


        public static Type CreateAccelerator<T>() where T : ObjectContext
        {
            return CreateAccelerator<T>(null);
        }

        public static Type CreateAccelerator<T>(string connectionString) where T : ObjectContext
        {
            return ObjectContextTypeStore.GetAccelerator(connectionString, typeof(T), () =>
                {
                    if (connectionString == null)
                    {
                        connectionString = GetDefaultConnectionString<T>();
                    }

                    // Get method info
                    MethodInfo entityConnectionFactory = ReflectionHelper.GetMethodInfo<object>(a =>

                        EntityConnectionFactory.CreateAccelerator(string.Empty));

                    Action<ILGenerator> factoryArguments = gen =>
                    {
                        gen.Emit(OpCodes.Ldstr, connectionString);
                    };

                    return CreateType<T>(entityConnectionFactory, factoryArguments);

                });
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
            Type type = CreateAccelerator<T>(connectionString);

            return Activator.CreateInstance(type) as T;
        }



        public static T CreateEmulatorInstance<T>() where T : ObjectContext
        {
            return CreateEmulatorInstance<T>(string.Empty, true);
        }

        public static T CreateEmulatorInstance<T>(bool shared) where T : ObjectContext
        {
            return CreateEmulatorInstance<T>(string.Empty, shared);
        }

        public static T CreateEmulatorInstance<T>(string source) where T : ObjectContext
        {
            return CreateEmulatorInstance<T>(source, true);
        }

        public static T CreateEmulatorInstance<T>(string source, bool shared) where T : ObjectContext
        {
            string connectionString = GetDefaultConnectionString<T>();

            return CreateEmulatorInstance<T>(connectionString, source, shared);
        }

        public static T CreateEmulatorInstance<T>(string connectionString, string source, bool shared) where T : ObjectContext
        {
            Type type = CreateEmulator<T>(connectionString, source, shared);

            return Activator.CreateInstance(type) as T;
        }

        #endregion

        private static string GetDefaultConnectionString<T>() where T : ObjectContext
        {
            return Activator.CreateInstance<T>().Connection.ConnectionString;
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

            ConstructorBuilder ctor = builder.DefineConstructor(MethodAttributes.Public | MethodAttributes.HideBySig, CallingConventions.Standard, new Type[] { });

            ConstructorInfo baseCtor = typeof(T).GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(EntityConnection) },
                null
                );


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


            // protected void Dispose(bool disposing)
            MethodInfo baseDispose = typeof(T).GetMethod(
                "Dispose",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(bool) },
                null);

            // public void Dispose()
            MethodInfo connectionDispose = typeof(Component).GetMethod(
                "Dispose",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null);

            MethodInfo connectionGetter = typeof(T).GetProperty("Connection").GetGetMethod();

            MethodBuilder overridedDispose = builder.DefineMethod(
                "Dispose",
                MethodAttributes.Family | 
                MethodAttributes.Virtual | 
                MethodAttributes.HideBySig | 
                MethodAttributes.ReuseSlot);

            overridedDispose.SetReturnType(typeof(void));
            // Adding parameters
            overridedDispose.SetParameters(typeof(bool));

            gen = overridedDispose.GetILGenerator();
            LocalBuilder l0 = gen.DeclareLocal(typeof(bool));

            Label label = gen.DefineLabel();

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.Emit(OpCodes.Ceq);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Brtrue_S, label);

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, connectionGetter);
            gen.Emit(OpCodes.Callvirt, connectionDispose);

            gen.MarkLabel(label);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Call, baseDispose);

            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            return builder.CreateType();
        }
    }
}
