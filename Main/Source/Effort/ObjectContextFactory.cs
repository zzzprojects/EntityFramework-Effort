// --------------------------------------------------------------------------------------------
// <copyright file="ObjectContextFactory.cs" company="Effort Team">
//     Copyright (C) Effort Team
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

namespace Effort
{
    using System;
    using System.ComponentModel;
    using System.Configuration;
#if !EFOLD
    using System.Data.Entity.Core.Objects;
    using System.Data.Entity.Core.EntityClient;
#else
    using System.Data.Objects;
    using System.Data.EntityClient;
#endif
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.DataLoaders;
    using Effort.Internal.Caching;
    using Effort.Internal.Common;
    using Effort.Provider;

    /// <summary>
    ///     Provides factory methods that are able to create <see cref="T:ObjectContext"/>
    ///     objects that rely on in-process and in-memory databases. All of the data operations
    ///     initiated from these context objects are executed by the appropriate in-memory 
    ///     database, so using these context objects does not require any external dependency
    ///     outside of the scope of the application.
    /// </summary>
    public static class ObjectContextFactory
    {
        /// <summary>
        ///     The dynamic CLI module that contains the dynamically created ObjectContext 
        ///     classes.
        /// </summary>
        private static ModuleBuilder objectContextContainer;

        /// <summary>
        ///     The count of the dynamically created ObjectContext classes.
        /// </summary>
        private static int objectContextCount;

        /// <summary>
        ///     Initializes static members of the <see cref="ObjectContextFactory" /> class.
        /// </summary>
        static ObjectContextFactory()
        {
            EffortProviderConfiguration.RegisterProvider();

            // Dynamic Library for Effort
            AssemblyBuilder assembly =
                Thread.GetDomain().DefineDynamicAssembly(
                    new AssemblyName(string.Format("DynamicObjectContextLib")),
                    AssemblyBuilderAccess.Run);

            // Module for the entity types
            objectContextContainer = assembly.DefineDynamicModule("ObjectContexts");
            objectContextCount = 0;
        }

        #region Persistent

        /// <summary>
        ///     Returns a new type that derives from the <see cref="T:ObjectContext"/> based
        ///     class specified by the <typeparamref name="T"/> generic argument. This class
        ///     relies on an in-memory database instance that lives during the complete 
        ///     application lifecycle. If the database is accessed the first time, then it will
        ///     be constructed based on the metadata referenced by the provided entity 
        ///     connection string and its state is initialized by the provided
        ///     <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreatePersistentType<T>(
            string entityConnectionString, 
            IDataLoader dataLoader) 
            where T : ObjectContext
        {
            return CreateType<T>(entityConnectionString, true, dataLoader);
        }

        /// <summary>
        ///     Returns a new type that derives from the <see cref="T:ObjectContext"/> based
        ///     class specified by the <typeparamref name="T"/> generic argument. This class 
        ///     relies on an in-memory database instance that lives during the complete 
        ///     application lifecycle. If the database is accessed the first time, then it will
        ///     be constructed based on the metadata referenced by the provided entity 
        ///     connection string.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and 
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreatePersistentType<T>(
            string entityConnectionString) 
            where T : ObjectContext
        {
            return CreateType<T>(entityConnectionString, true, null);
        }

        /// <summary>
        ///     Returns a new type that derives from the <see cref="T:ObjectContext"/> based
        ///     class specified by the <typeparamref name="T"/> generic argument. This class 
        ///     relies on an in-memory database instance that lives during the complete 
        ///     application lifecycle. If the database is accessed the first time, then it will
        ///     be constructed based on the metadata referenced by the default entity 
        ///     connection string of the provided <see cref="T:ObjectContext"/> type.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreatePersistentType<T>() 
            where T : ObjectContext
        {
            return CreateType<T>(null, true, null);
        }

        /// <summary>
        ///     Returns a new type that derives from the <see cref="T:ObjectContext"/> based
        ///     class specified by the <typeparamref name="T"/> generic argument. This class
        ///     relies on an in-memory database instance that lives during the complete 
        ///     application lifecycle. If the database is accessed the first time, then it will
        ///     be constructed based on the metadata referenced by the default entity 
        ///     connection string of the provided <see cref="T:ObjectContext"/> type and its
        ///     state is initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreatePersistentType<T>(IDataLoader dataLoader)
            where T : ObjectContext
        {
            return CreateType<T>(null, true, dataLoader);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the complete application
        ///     lifecycle. If the database is accessed the first time, then it will be 
        ///     constructed based on the metadata referenced by the provided entity connection 
        ///     string.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and 
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <returns>The <see cref="T:ObjectContext"/> object.</returns>
        public static T CreatePersistent<T>(string entityConnectionString) 
            where T : ObjectContext
        {
            return Activator.CreateInstance(
                CreatePersistentType<T>(entityConnectionString)) as T;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the complete application
        ///     lifecycle. If the database is accessed the first time, then it will be 
        ///     constructed based on the metadata referenced by the provided entity connection
        ///     string and its state is initialized by the provided <see cref="T:IDataLoader"/>
        ///     object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreatePersistent<T>(
            string entityConnectionString, 
            IDataLoader dataLoader) 
            where T : ObjectContext
        {
            return Activator.CreateInstance(
                CreatePersistentType<T>(entityConnectionString, dataLoader)) as T;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the complete application
        ///     lifecycle. If the database is accessed the first time, then it will be
        ///     constructed based on the metadata referenced by the default entity connection
        ///     string of the provided <see cref="T:ObjectContext"/> type.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreatePersistent<T>() 
            where T : ObjectContext
        {
            return Activator.CreateInstance(CreatePersistentType<T>()) as T;
        }

        /// <summary>
        ///     Creates a instance of the <see cref="T:ObjectContext"/> based class specified 
        ///     by the <typeparamref name="T"/> generic argument. This class relies on an 
        ///     in-memory database instance that lives during the complete application 
        ///     lifecycle. If the database is accessed the first time, then it will be 
        ///     constructed based on the metadata referenced by the default entity connection
        ///     string of the provided <see cref="T:ObjectContext"/> type and its state is
        ///     initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreatePersistent<T>(
            IDataLoader dataLoader) 
            where T : ObjectContext
        {
            return Activator.CreateInstance(CreatePersistentType<T>(dataLoader)) as T;
        }

        #endregion

        #region Transient

        /// <summary>
        ///     Returns a type that derives from the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the provided entity connection
        ///     string and its state is initialized by the provided <see cref="T:IDataLoader"/>
        ///     object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and 
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreateTransientType<T>(
            string entityConnectionString, 
            IDataLoader dataLoader)
            where T : ObjectContext
        {
            return CreateType<T>(entityConnectionString, false, dataLoader);
        }

        /// <summary>
        ///     Returns a type that derives from the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the provided entity connection
        ///     string.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and 
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreateTransientType<T>(
            string entityConnectionString) 
            where T : ObjectContext
        {
            return CreateType<T>(entityConnectionString, false, null);
        }

        /// <summary>
        ///     Returns a type that derives from the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies 
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the default entity connection
        ///     string of the provided <see cref="T:ObjectContext"/> type.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreateTransientType<T>() where T : ObjectContext
        {
            return CreateType<T>(null, false, null);
        }

        /// <summary>
        ///     Returns a type that derives from the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context object is disposed or garbage collected, then 
        ///     the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the default entity connection 
        ///     string of the provided <see cref="T:ObjectContext"/> type and its state is 
        ///     initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:Type"/> object.
        /// </returns>
        public static Type CreateTransientType<T>(
            IDataLoader dataLoader)
            where T : ObjectContext
        {
            return CreateType<T>(null, false, dataLoader);
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the provided entity connection
        ///     string and its state is initialized by the provided <see cref="T:IDataLoader"/>
        ///     object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and 
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreateTransient<T>(
            string entityConnectionString, 
            IDataLoader dataLoader)
            where T : ObjectContext
        {
            return Activator.CreateInstance(
                CreateTransientType<T>(entityConnectionString, dataLoader)) as T;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the provided entity connection
        ///     string.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that identifies the in-memory database and
        ///     references the metadata that is required for constructing the schema.
        /// </param>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreateTransient<T>(
            string entityConnectionString)
            where T : ObjectContext
        {
            return Activator.CreateInstance(
                CreateTransientType<T>(entityConnectionString)) as T;
        }

        /// <summary>
        ///     Creates a new instance of the <see cref="T:ObjectContext"/> based class 
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies 
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context instance is disposed or garbage collected, 
        ///     then the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the default entity connection
        ///     string of the provided <see cref="T:ObjectContext"/> type and its state is 
        ///     initialized by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreateTransient<T>(
            IDataLoader dataLoader)
            where T : ObjectContext
        {
            return Activator.CreateInstance(
                CreateTransientType<T>(dataLoader)) as T;
        }

        /// <summary>
        ///     Creates of new instance of the <see cref="T:ObjectContext"/> based class
        ///     specified by the <typeparamref name="T"/> generic argument. This class relies
        ///     on an in-memory database instance that lives during the context object 
        ///     lifecycle. If the object context object is disposed or garbage collected, then
        ///     the underlying database will be garbage collected too. The database is 
        ///     constructed based on the metadata referenced by the default entity connection
        ///     string of the provided <see cref="T:ObjectContext"/> type.
        /// </summary>
        /// <typeparam name="T">
        ///     The concrete <see cref="T:ObjectContext"/> based class.
        /// </typeparam>
        /// <returns>
        ///     The <see cref="T:ObjectContext"/> object.
        /// </returns>
        public static T CreateTransient<T>()
            where T : ObjectContext
        {
             return Activator.CreateInstance(
                 CreateTransientType<T>()) as T;
        }

        #endregion

        /// <summary>
        ///     Returns the appropriate dynamic ObjectContext type.
        /// </summary>
        /// <typeparam name="T">
        ///     The ObjectContext type that the result type should derive from.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the metadata and identifies the 
        ///     persistent database.
        /// </param>
        /// <param name="persistent">
        ///     if set to <c>true</c> the ObjectContext uses a persistent database, otherwise 
        ///     transient.
        /// </param>
        /// <param name="dataLoader">
        ///     The data loader that initializes the state of the database.
        /// </param>
        /// <returns>
        ///     The ObjectContext type.
        /// </returns>
        private static Type CreateType<T>(
            string entityConnectionString, 
            bool persistent, 
            IDataLoader dataLoader) 
            where T : ObjectContext
        {
            EffortConnectionStringBuilder ecsb = new EffortConnectionStringBuilder();

            if (dataLoader != null)
            {
                ecsb.DataLoaderType = dataLoader.GetType();
                ecsb.DataLoaderArgument = dataLoader.Argument;
            }

            string effortConnectionString = ecsb.ConnectionString;

            return ObjectContextTypeStore.GetObjectContextType(
                entityConnectionString, 
                effortConnectionString, 
                typeof(T), 
                () =>
                {
                    if (string.IsNullOrEmpty(entityConnectionString))
                    {
                        entityConnectionString = GetDefaultConnectionString<T>();
                    }

                    return CreateType<T>(
                        entityConnectionString, 
                        effortConnectionString, 
                        persistent);
                });
        }

        /// <summary>
        ///     Returns the default entity connection string of the specified ObjectContext
        ///     type.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the ObjectContext.
        /// </typeparam>
        /// <returns>
        ///     The entity connection string.
        /// </returns>
        private static string GetDefaultConnectionString<T>() where T : ObjectContext
        {
            bool hasDefaultConstructor = 
                typeof(T).GetConstructor(new Type[] { }) != null;

            if (hasDefaultConstructor)
            {
                return Activator.CreateInstance<T>().Connection.ConnectionString;
            }
            else
            {
                return FindDefaultConnectionStringByConvention<T>();
            }
        }

        /// <summary>
        ///     Creates a ObjectContext type during dynamically.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the ObjectContext.
        /// </typeparam>
        /// <param name="entityConnectionString">
        ///     The entity connection string that references the metadata and identifies the 
        ///     persistent database.
        /// </param>
        /// <param name="effortConnectionString">
        ///     The effort connection string that is passed to the EffortConnection object.
        /// </param>
        /// <param name="persistent">
        ///     if set to <c>true</c> the ObjectContext uses a persistent database, otherwise 
        ///     transient.
        /// </param>
        /// <returns>The ObjectContext type.</returns>
        private static Type CreateType<T>(
            string entityConnectionString, 
            string effortConnectionString, 
            bool persistent)
        {
            TypeBuilder builder = null;

            lock (objectContextContainer)
            {
                objectContextCount++;
                builder = objectContextContainer.DefineType(
                    string.Format("DynamicObjectContext{0}", objectContextCount),
                    TypeAttributes.Public,
                    typeof(T));
            }

            //// public DynamicObjectContext() : base(EntityConnectionFactory.Create(...))
            ConstructorBuilder ctor = 
                builder.DefineConstructor(
                    MethodAttributes.Public | MethodAttributes.HideBySig, 
                    CallingConventions.Standard, 
                    new Type[] { });

            ConstructorInfo baseCtor = 
                typeof(T).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null,
                    new Type[] { typeof(EntityConnection) },
                    null);

            ILGenerator gen = ctor.GetILGenerator();

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Ldstr, entityConnectionString);
            gen.Emit(OpCodes.Ldstr, effortConnectionString);
            gen.Emit(persistent ? OpCodes.Ldc_I4_1 : OpCodes.Ldc_I4_0);

            MethodInfo entityConnectionFactory = ReflectionHelper.GetMethodInfo<object>(a =>
                       EntityConnectionFactory.Create(string.Empty, string.Empty, false));

            gen.Emit(OpCodes.Call, entityConnectionFactory);
            gen.Emit(OpCodes.Call, baseCtor);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Nop);
            gen.Emit(OpCodes.Ret);

            //// protected void Dispose(bool disposing)
            MethodInfo baseDispose = typeof(T).GetMethod(
                "Dispose",
                BindingFlags.Instance | BindingFlags.NonPublic,
                null,
                new Type[] { typeof(bool) },
                null);

            //// public void Dispose()
            MethodInfo connectionDispose = typeof(Component).GetMethod(
                "Dispose",
                BindingFlags.Instance | BindingFlags.Public,
                null,
                Type.EmptyTypes,
                null);

            MethodInfo connectionGetter = typeof(T).GetProperty("Connection").GetGetMethod();

            MethodBuilder overridedDispose = 
                builder.DefineMethod(
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

        /// <summary>
        ///     Returns the default connection string by convention.
        /// </summary>
        /// <typeparam name="T">
        ///     The type of the ObjectContext.
        /// </typeparam>
        /// <returns>
        ///     The default connection string based on the name of the ObjectContext
        /// </returns>
        private static string FindDefaultConnectionStringByConvention<T>() 
            where T : ObjectContext
        {
            string requestedName = typeof(T).Name;

            foreach (ConnectionStringSettings connectionString in 
                ConfigurationManager.ConnectionStrings)
            {
                if (string.Equals(
                        connectionString.ProviderName, 
                        "System.Data.EntityClient", 
                        StringComparison.InvariantCulture) &&
                    string.Equals(
                        connectionString.Name, 
                        requestedName, 
                        StringComparison.InvariantCulture))
                {
                    return connectionString.ConnectionString;
                }
            }

            throw new InvalidOperationException(
                "ObjectContext/DbContext does not have a default connection string");
        }
    }
}
