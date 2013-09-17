// --------------------------------------------------------------------------------------------
// <copyright file="DataRowFactory.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
// -------------------------------------------------------------------------------------------

namespace Effort.Internal.TypeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;
    using System.Threading;
    using Effort.Internal.Caching;
using Effort.Internal.Common;

    internal static class DataRowFactory
    {
        private static int typeCount;
        private static readonly AssemblyBuilder assemblyBuilder;

        private static readonly ModuleBuilder moduleBuilder;
        private static readonly object moduleBuilderLock;

        private static readonly ConcurrentCache<TypeCacheEntryKey, Type> typeCache;

        private static MethodInfo GetValueNameMethod = 
            ReflectionHelper.GetMethodInfo<DataRow>(r => r.GetValue(0));

        static DataRowFactory()
        {
            typeCount = 0;

            assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                new AssemblyName("EffortDataRowTypeLib"),
                    AssemblyBuilderAccess.Run);

            moduleBuilder = assemblyBuilder.DefineDynamicModule("EffortDataRowTypeLib");
            moduleBuilderLock = new object();

            typeCache = new ConcurrentCache<TypeCacheEntryKey, Type>();
        }

        private class TypeCacheEntryKey : IEquatable<TypeCacheEntryKey>
        {
            private readonly string[] names;
            private readonly Type[] types;

            public TypeCacheEntryKey(IDictionary<string, Type> properties)
            {
                this.names = properties.Keys.ToArray();
                this.types = properties.Values.ToArray();
            }

            public override int GetHashCode()
            {
                int result = 0;
                int hash;

                for (int i = 0; i < this.names.Length; i++)
                {
                    hash = this.names[i].GetHashCode() ^ this.types[i].GetHashCode();

                    // Rotate and mod2 addition
                    result = result ^ ((hash << (i % 32)) | (hash >> (32 - (i % 32))));
                }

                return result;
            }

            public override bool Equals(object obj)
            {
                TypeCacheEntryKey key = obj as TypeCacheEntryKey;

                if (key == null)
                {
                    return false;
                }

                return this.Equals(key);
            }

            public bool Equals(TypeCacheEntryKey other)
            {
                if (other.names.Length != this.names.Length)
                {
                    return false;
                }

                for (int i = 0; i < this.names.Length; i++)
                {
                    if (!string.Equals(this.names[i], other.names[i]) ||
                        !this.types[i].Equals(other.types[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }
        public static Type Create(IDictionary<string, Type> properties)
        {
            TypeCacheEntryKey key = new TypeCacheEntryKey(properties);

            return typeCache.Get(key, () => CreateRowType(properties));
        }

        private static Type CreateRowType(IDictionary<string, Type> properties)
        {
            string[] propertyNames = properties.Keys.ToArray();
            Type[] propertyTypes = properties.Values.ToArray();

            TypeBuilder typeBuilder;

            lock (moduleBuilderLock)
            {
                typeCount++;
                typeBuilder =
                    moduleBuilder.DefineType(
                        string.Format("DataRow{0}", typeCount),
                        TypeAttributes.Public,
                    // Derive from object
                        typeof(DataRow),
                    // No interfaces
                        Type.EmptyTypes);
            }

            bool isLarge = LargeDataRowAttribute.LargePropertyCount <= properties.Count;

            #region LargeDataRowAttribute

            if (isLarge)
            {
                ConstructorInfo largeDataRowAttrCtor =
                    typeof(LargeDataRowAttribute).GetConstructor(
                        BindingFlags.Instance | BindingFlags.Public,
                        null,
                        Type.EmptyTypes,
                        null);

                typeBuilder.SetCustomAttribute(
                    new CustomAttributeBuilder(largeDataRowAttrCtor, new object[0]));
            }

            #endregion

            #region Properties and fields

            FieldBuilder[] fields = new FieldBuilder[properties.Count];

            for (int i = 0; i < properties.Count; i++)
            {
                string name = propertyNames[i];
                Type type = propertyTypes[i];

                FieldBuilder field = CreateFieldAndProperty(typeBuilder, name, i, type);

                fields[i] = field;
            }

            #endregion

            #region Constructor

            ConstructorBuilder ctorBuilder =
                typeBuilder.DefineConstructor(
                    MethodAttributes.Public,
                    CallingConventions.Standard,
                    propertyTypes);

            if (isLarge)
            {
                ctorBuilder.DefineParameter(0, ParameterAttributes.None, "args");
            }
            else
            {
                for (int i = 0; i < propertyNames.Length; i++)
                {
                    ctorBuilder.DefineParameter(
                        i,
                        ParameterAttributes.None,
                        propertyNames[i]);
                }
            }

            GenerateConstructorIL(ctorBuilder.GetILGenerator(), fields, isLarge);

            #endregion

            #region GetHashCode

            MethodBuilder getHashCodeBuilder =
            typeBuilder.DefineMethod(
                "GetHashCode",
                MethodAttributes.Public |
                MethodAttributes.Virtual |
                MethodAttributes.HideBySig,
                null,
                Type.EmptyTypes);

            getHashCodeBuilder.SetReturnType(typeof(int));

            GenerateGetHashcodeIL(getHashCodeBuilder.GetILGenerator(), fields);

            #endregion

            #region Equals

            MethodBuilder equalsBuilder =
                typeBuilder.DefineMethod(
                    "Equals",
                    MethodAttributes.Public |
                    MethodAttributes.Virtual |
                    MethodAttributes.HideBySig,
                    null,
                    Type.EmptyTypes);

            equalsBuilder.SetParameters(new Type[] { typeof(object) });
            equalsBuilder.SetReturnType(typeof(bool));
            GenerateEqualsIL(equalsBuilder.GetILGenerator(), fields, typeBuilder);

            #endregion

            #region GetValue

            MethodBuilder getValueBuilder =
                typeBuilder.DefineMethod(
                    GetValueNameMethod.Name,
                    MethodAttributes.Public |
                    MethodAttributes.Virtual |
                    MethodAttributes.HideBySig,
                    null,
                    Type.EmptyTypes);

            getValueBuilder.SetParameters(new Type[] { typeof(int) });
            getValueBuilder.SetReturnType(typeof(object));
            GenerateGetValueIL(getValueBuilder.GetILGenerator(), fields, typeBuilder);

            #endregion

            return typeBuilder.CreateType();
        }

        private static FieldBuilder CreateFieldAndProperty(
            TypeBuilder typeBuilder,
            string name,
            int index,
            Type type)
        {
            FieldBuilder field = typeBuilder
                .DefineField("_" + name, type, FieldAttributes.Private);

            PropertyBuilder propertyBuilder =
                typeBuilder.DefineProperty(
                    name,
                    PropertyAttributes.HasDefault,
                    type,
                    null);

            ConstructorInfo dataRowPropertyAttrCtor =
                typeof(DataRowPropertyAttribute).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    new Type[] { typeof(int) },
                    null);

            propertyBuilder.SetCustomAttribute(
                new CustomAttributeBuilder(
                    dataRowPropertyAttrCtor,
                    new object[] { index }));

            MethodBuilder propertyGetAccessor = typeBuilder.DefineMethod(
                "get_" + name,
                MethodAttributes.Public |
                MethodAttributes.SpecialName |
                MethodAttributes.HideBySig,
                type,
                Type.EmptyTypes);

            ILGenerator numberGetIL = propertyGetAccessor.GetILGenerator();
            // s[0] = this
            numberGetIL.Emit(OpCodes.Ldarg_0);
            // s[0] = s[0].field
            numberGetIL.Emit(OpCodes.Ldfld, field);
            // return s[0]
            numberGetIL.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(propertyGetAccessor);
            return field;
        }


        private static void GenerateConstructorIL(
            ILGenerator gen,
            FieldBuilder[] fields,
            bool array)
        {
            ConstructorInfo objectConstructor =
                typeof(object).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    Type.EmptyTypes,
                    null);

            // s[0] = this
            gen.Emit(OpCodes.Ldarg_0);
            // s[0].base()
            gen.Emit(OpCodes.Call, objectConstructor);

            for (int i = 0; i < fields.Length; i++)
            {
                // s[0] = this
                gen.Emit(OpCodes.Ldarg_0);

                if (array)
                {
                    // s[1] = param[0] (object[] array)
                    gen.Emit(OpCodes.Ldarg_1);
                    // s[1] = s[1][i]
                    gen.Emit(OpCodes.Ldelem_Ref, i);
                    // s[1] = unbox s[1]
                    gen.Emit(OpCodes.Unbox);
                }
                else
                {
                    // s[1] = param[i]
                    switch (i)
                    {
                        case 0:
                            gen.Emit(OpCodes.Ldarg_1);
                            break;
                        case 1:
                            gen.Emit(OpCodes.Ldarg_2);
                            break;
                        case 2:
                            gen.Emit(OpCodes.Ldarg_3);
                            break;
                        default:
                            gen.Emit(OpCodes.Ldarg_S, i + 1);
                            break;
                    }
                }


                // s[0].field = s[1]
                gen.Emit(OpCodes.Stfld, fields[i]);
            }

            // Empty stack
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashcodeIL(ILGenerator gen, FieldBuilder[] fields)
        {
            int hashSeed = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];

                hashSeed =
                    hashSeed ^
                    field.Name.GetHashCode() ^
                    field.FieldType.GetHashCode();
            }

            // s[0] = seed
            gen.Emit(OpCodes.Ldc_I4, hashSeed);

            for (int i = 0; i < fields.Length; i++)
            {
                Type type = fields[i].FieldType;

                Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(type);

                MethodInfo defaultEqualityComparerGetter = equalityComparerType
                    .GetProperty("Default")
                    .GetGetMethod();

                MethodInfo getHashCodeMethod = equalityComparerType
                    .GetMethod("GetHashCode", new Type[] { type });

                // s[1] = const
                gen.Emit(OpCodes.Ldc_I4, -1521134295);
                // s[0] = s[0] * s[1]
                gen.Emit(OpCodes.Mul);

                // s[1] = EqualityComparer<T>.Default
                gen.Emit(OpCodes.Call, defaultEqualityComparerGetter);

                // s[2] = this.field
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fields[i]);

                // s[1] = s[1].GetHashCode(s[2])
                gen.Emit(OpCodes.Callvirt, getHashCodeMethod);

                // s[0] = s[0] + s[1]
                gen.Emit(OpCodes.Add);
            }

            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateEqualsIL(
            ILGenerator gen,
            FieldBuilder[] fields,
            TypeBuilder typeBuilder)
        {
            Label notIdenticalLabel = gen.DefineLabel();
            Label skip = gen.DefineLabel();

            // s[0] = param[0] (other)
            gen.Emit(OpCodes.Ldarg_1);
            // s[0] = s[0] is ThisType
            gen.Emit(OpCodes.Isinst, typeBuilder);

            for (int i = 0; i < fields.Length; i++)
            {
                // if (s[0] == null|false) goto notIdentical
                gen.Emit(OpCodes.Brfalse, notIdenticalLabel);

                Type type = fields[i].FieldType;

                Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(type);

                MethodInfo defaultEqualityComparerGetter = equalityComparerType
                    .GetProperty("Default")
                    .GetGetMethod();

                MethodInfo equalsMethod = equalityComparerType
                    .GetMethod("Equals", new Type[] { type, type });

                // s[0] = EqualityComparer<T>.Default
                gen.Emit(OpCodes.Call, defaultEqualityComparerGetter);

                // s[1] = this
                gen.Emit(OpCodes.Ldarg_0);
                // s[1] = s[1].field (this.field)
                gen.Emit(OpCodes.Ldfld, fields[i]);

                // s[2] = param[0] (other)
                gen.Emit(OpCodes.Ldarg_1);
                // s[2] = s[2].field (other field)
                gen.Emit(OpCodes.Ldfld, fields[i]);

                // s[0] = s[0].Equals(s[1], s[2])
                gen.Emit(OpCodes.Callvirt, equalsMethod);
            }

            // The result is already on the stack
            // goto skip
            gen.Emit(OpCodes.Br_S, skip);

            // notIdentical:
            gen.MarkLabel(notIdenticalLabel);
            // s[0] = false
            gen.Emit(OpCodes.Ldc_I4_0);

            // skip:
            gen.MarkLabel(skip);
            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetValueIL(
            ILGenerator gen,
            FieldBuilder[] fields,
            TypeBuilder typeBuilder)
        {
            Label[] jumpTable = new Label[fields.Length];

            for (int i = 0; i < fields.Length; i++)
            {
                jumpTable[i] = gen.DefineLabel();
            }

            Label defaultCase = gen.DefineLabel();

            //// s[0] = this
            gen.Emit(OpCodes.Ldarg_0);

            //// s[1] = param[0] (index)
            gen.Emit(OpCodes.Ldarg_1);
            //// switch(s[1])
            gen.Emit(OpCodes.Switch, jumpTable);
            gen.Emit(OpCodes.Br, defaultCase);

            for (int i = 0; i < fields.Length; i++)
            {
                gen.MarkLabel(jumpTable[i]);

                //// s[0] = s[0].field
                gen.Emit(OpCodes.Ldfld, fields[i]);

                if (fields[i].FieldType.IsValueType)
                {
                    // s[0] = box s[0]
                    gen.Emit(OpCodes.Box, fields[i].FieldType);
                }

                // return s[0]
                gen.Emit(OpCodes.Ret);
            }

            // Default case
            gen.MarkLabel(defaultCase);
            // s[0] = new ArgumentOutOfRangeException();
            ConstructorInfo exceptionCtor =
                typeof(ArgumentOutOfRangeException).GetConstructor(Type.EmptyTypes);

            gen.Emit(OpCodes.Newobj, exceptionCtor);
            // throw s[0]
            gen.Emit(OpCodes.Throw);
        }
    }
}
