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
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Effort.Internal.Caching;

namespace Effort.Internal.TypeGeneration
{
    internal class AnonymousTypeFactory
    {
        private static int typeCount;
        private static AssemblyBuilder assemblyBuilder;

        private static ModuleBuilder moduleBuilder;
        private static object moduleBuilderLock;

        private static ConcurrentCache<TypeCacheEntryKey, Type> typeCache;

        public static bool SkipCompiledTypeSearch { set; get; }

        private class TypeCacheEntryKey : IEquatable<TypeCacheEntryKey>
        {
            private string[] _propertyNames;

            public TypeCacheEntryKey(string[] propertyNames)
            {
                _propertyNames = propertyNames;
            }

            public override int GetHashCode()
            {
                int result = 0;

                for (int i = 0; i < _propertyNames.Length; i++)
                {
                    int hash = _propertyNames[i].GetHashCode();
                    //Rotate and mod2 addition
                    result = result ^ ((hash << i) | (hash >> (32 - i)));
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

                return Equals(key);
            }

            public bool Equals(TypeCacheEntryKey other)
            {
                if (other._propertyNames.Length != this._propertyNames.Length)
                {
                    return false;
                }

                for (int i = 0; i < this._propertyNames.Length; i++)
                {
                    if (!string.Equals(this._propertyNames[i], other._propertyNames[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }


        static AnonymousTypeFactory()
        {
            typeCount = 0;

            assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                new AssemblyName("DynamicAnonymousTypeLib"),
                    AssemblyBuilderAccess.Run);

            moduleBuilder = assemblyBuilder.DefineDynamicModule("DynamicAnonymousTypeLib");
            moduleBuilderLock = new object();

            typeCache = new ConcurrentCache<TypeCacheEntryKey, Type>();

            AnonymousTypeFactory.SkipCompiledTypeSearch = true;
        }

        public static Type Create(IDictionary<string, Type> properties)
        {
            string[] propertyNames = properties.Select(kvp => kvp.Key).ToArray();
            TypeCacheEntryKey key = new TypeCacheEntryKey(propertyNames);

            Type genericType = typeCache.Get(key, () =>
                {
                    Type result = null;

                    if (!AnonymousTypeFactory.SkipCompiledTypeSearch)
                    {
                        //Search in assemblies
                        result = SearchForAnonymousType(propertyNames);
                    }

                    if (result == null)
                    {
                        //Generate dynamic type
                        result = CreateGenericType(propertyNames);
                    }

                    return result;
                });

           
            Type[] propertyTypes = properties.Select(kvp => kvp.Value).ToArray();
            return genericType.MakeGenericType(propertyTypes);
        }

        #region Existing anonymous type search

        private static Type SearchForAnonymousType(string[] propertyNames)
        {
            var q =
                from type in
                    AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .Where(a => !a.FullName.StartsWith("mscorlib") && !a.FullName.StartsWith("System.") && !a.FullName.StartsWith("Microsoft."))
                    .SelectMany(s => s.GetTypes())
                where
                    type
                    .GetCustomAttributes(typeof(DebuggerDisplayAttribute), false)
                    .OfType<DebuggerDisplayAttribute>()
                    .Any(a => a.Type == "<Anonymous Type>")
                let
                    props = type.GetProperties()
                where
                    props.Length == propertyNames.Length &&
                    props.All(p => propertyNames.Contains(p.Name)) &&
                    IdenticalOrder(props, type.GetGenericArguments(), propertyNames)
                select
                    type;

            return q.FirstOrDefault();

        }

        private static bool IdenticalOrder(PropertyInfo[] properties, Type[] genericArguments, string[] propertyNames)
        {
            var typeList = genericArguments.ToList();
            var propertyNameList = propertyNames.ToList();

            for (int i = 0; i < properties.Length; i++)
            {
                if (typeList.IndexOf(properties[i].PropertyType) != propertyNameList.IndexOf(properties[i].Name))
                {
                    return false;
                }
            }

            return true;
        }

        #endregion

        private static Type CreateGenericType(string[] propertyNames)
        {
            TypeBuilder typeBuilder;

            lock (moduleBuilderLock)
            {
                typeCount++;
                typeBuilder =
                    moduleBuilder.DefineType(
                        string.Format("at{0}", typeCount),
                        TypeAttributes.Public,
                    //Derive from object
                        typeof(Object),
                    //No interfaces
                        Type.EmptyTypes);
            }

            //Define generics
            var genericTypes = typeBuilder.DefineGenericParameters(propertyNames.Select(prop => "T" + prop).ToArray());

            #region DebuggerDisplay attribute

            ConstructorInfo debuggerDisplayAttributeConstructor =
                    typeof(DebuggerDisplayAttribute).GetConstructor(
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { typeof(string) },
                        null);

            StringBuilder displayExpressionBuilder = new StringBuilder();
            displayExpressionBuilder.Append(@"\{ ");

            for (int i = 0; i < propertyNames.Length - 1; i++)
            {
                AppendPropertyToDisplayExpression(displayExpressionBuilder, propertyNames[i]);
                displayExpressionBuilder.Append(", ");
            }

            AppendPropertyToDisplayExpression(displayExpressionBuilder, propertyNames[propertyNames.Length - 1]);
            displayExpressionBuilder.Append(" }");

            CustomAttributeBuilder debuggerDisplayAttributeBuilder =
                new CustomAttributeBuilder(
                    debuggerDisplayAttributeConstructor,
                    new object[] { displayExpressionBuilder.ToString() },
                    new PropertyInfo[] { typeof(DebuggerDisplayAttribute).GetProperty("Type") },
                    new object[] { "<Anonymous Type>" });


            typeBuilder.SetCustomAttribute(debuggerDisplayAttributeBuilder);

            #endregion

            FieldBuilder[] fields = new FieldBuilder[propertyNames.Length];

            #region Properties and private fields


            for (int i = 0; i < propertyNames.Length; i++)
            {
                fields[i] = typeBuilder.DefineField("_" + propertyNames[i], genericTypes[i], FieldAttributes.Private);

                //Do not show the private field in the debugger
                ConstructorInfo DebuggerBrowsableAttributeConstructor =
                    typeof(DebuggerBrowsableAttribute).GetConstructor(
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new Type[] { typeof(DebuggerBrowsableState) },
                        null);

                fields[i].SetCustomAttribute(new CustomAttributeBuilder(DebuggerBrowsableAttributeConstructor, new object[] { DebuggerBrowsableState.Never }));

                PropertyBuilder propertyBuilder =
                    typeBuilder.DefineProperty(
                        propertyNames[i],
                        System.Reflection.PropertyAttributes.HasDefault,
                        genericTypes[i],
                        null);

                MethodAttributes getSetAttr = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig;


                MethodBuilder mbGetAccessor = typeBuilder.DefineMethod(
                    "get_" + propertyNames[i],
                    getSetAttr,
                    genericTypes[i],
                    Type.EmptyTypes);

                ILGenerator numberGetIL = mbGetAccessor.GetILGenerator();
                numberGetIL.Emit(OpCodes.Ldarg_0);
                numberGetIL.Emit(OpCodes.Ldfld, fields[i]);
                numberGetIL.Emit(OpCodes.Ret);

                propertyBuilder.SetGetMethod(mbGetAccessor);

            }

            #endregion

            ConstructorInfo debuggerHiddenAttributeConstructor =
                typeof(DebuggerHiddenAttribute).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    Type.EmptyTypes,
                    null);

            #region Constructor

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, genericTypes);

            constructorBuilder.SetCustomAttribute(new CustomAttributeBuilder(debuggerHiddenAttributeConstructor, new object[] { }));

            for (int i = 0; i < propertyNames.Length; i++)
            {
                constructorBuilder.DefineParameter(i, ParameterAttributes.None, propertyNames[i]);
            }

            GenerateConstructorIL(constructorBuilder.GetILGenerator(), fields);

            #endregion

            #region GetHashCode

            MethodBuilder getHashCodeBuilder =
                typeBuilder.DefineMethod(
                    "GetHashCode",
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    null,
                    Type.EmptyTypes);

            getHashCodeBuilder.SetReturnType(typeof(Int32));

            GenerateGetHashcodeIL(getHashCodeBuilder.GetILGenerator(), fields);
            getHashCodeBuilder.SetCustomAttribute(new CustomAttributeBuilder(debuggerHiddenAttributeConstructor, new object[] { }));

            #endregion

            #region ToString

            MethodBuilder toStringBuilder =
                typeBuilder.DefineMethod(
                    "ToString",
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    null,
                    Type.EmptyTypes);

            toStringBuilder.SetReturnType(typeof(string));
            GenerateToStringIL(toStringBuilder.GetILGenerator(), fields);

            toStringBuilder.SetCustomAttribute(new CustomAttributeBuilder(debuggerHiddenAttributeConstructor, new object[] { }));

            #endregion

            #region Equals

            MethodBuilder equalsBuilder =
                typeBuilder.DefineMethod(
                    "Equals",
                    MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
                    null,
                    Type.EmptyTypes);

            equalsBuilder.SetParameters(new Type[] { typeof(object) });
            equalsBuilder.SetReturnType(typeof(bool));
            GenerateEqualsIL(equalsBuilder.GetILGenerator(), fields, typeBuilder);

            equalsBuilder.SetCustomAttribute(new CustomAttributeBuilder(debuggerHiddenAttributeConstructor, new object[] { }));

            #endregion

            return typeBuilder.CreateType();
        }

        #region IL Generations

        private static void GenerateConstructorIL(ILGenerator gen, FieldBuilder[] fields)
        {
            ConstructorInfo objectConstructor =
                typeof(DebuggerHiddenAttribute).GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public,
                    null,
                    Type.EmptyTypes,
                    null);

            gen.Emit(OpCodes.Ldarg_0);
            gen.Emit(OpCodes.Call, objectConstructor);

            for (int i = 0; i < fields.Length; i++)
            {
                gen.Emit(OpCodes.Ldarg_0);
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

                gen.Emit(OpCodes.Stfld, fields[i]);
            }

            gen.Emit(OpCodes.Ret);
        }

        private static void GenerateGetHashcodeIL(ILGenerator gen, FieldBuilder[] fields)
        {
            LocalBuilder l0 = gen.DeclareLocal(typeof(Int32));
            LocalBuilder l1 = gen.DeclareLocal(typeof(Int32));
            Label label = gen.DefineLabel();

            int hashSeed = fields.Aggregate(0, (a, v) => a ^ v.Name.GetHashCode(), a => a);

            gen.Emit(OpCodes.Ldc_I4, hashSeed);


            for (int i = 0; i < fields.Length; i++)
            {
                Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(fields[i].FieldType);

                var defaultEqualityComparerFactoryMethod =
                    TypeBuilder.GetMethod(equalityComparerType, typeof(EqualityComparer<>).GetMethod("get_Default"));

                var getHashCodeMethod =
                        TypeBuilder.GetMethod(equalityComparerType, typeof(EqualityComparer<>).GetMethods().Single(mi => mi.Name == "GetHashCode" && mi.ContainsGenericParameters));

                gen.Emit(OpCodes.Stloc_0);
                gen.Emit(OpCodes.Ldc_I4, -1521134295);
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Mul);
                gen.Emit(OpCodes.Call, defaultEqualityComparerFactoryMethod);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fields[i]);
                gen.Emit(OpCodes.Callvirt, getHashCodeMethod);
                gen.Emit(OpCodes.Add);

            }

            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br_S, label);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ret);

        }

        private static void GenerateToStringIL(ILGenerator gen, FieldBuilder[] fields)
        {
            ConstructorInfo stringBuilderConstructor =
                typeof(StringBuilder)
                .GetConstructor(
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null);

            MethodInfo stringBuilderAppend =
                typeof(StringBuilder)
                .GetMethod(
                    "Append",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(string) },
                    null);

            MethodInfo stringBuilderAppend2 =
                typeof(StringBuilder)
                .GetMethod(
                    "Append",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    new Type[] { typeof(object) },
                    null);

            MethodInfo stringBuilderToString =
                typeof(StringBuilder)
                .GetMethod(
                    "ToString",
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null);

            LocalBuilder builder = gen.DeclareLocal(typeof(StringBuilder));
            LocalBuilder str = gen.DeclareLocal(typeof(String));

            Label label = gen.DefineLabel();

            gen.Emit(OpCodes.Newobj, stringBuilderConstructor);
            gen.Emit(OpCodes.Stloc_0);

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldstr, "{");
            gen.Emit(OpCodes.Callvirt, stringBuilderAppend);
            gen.Emit(OpCodes.Pop);


            for (int i = 0; i < fields.Length; i++)
            {
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Ldstr, " " + fields[i].Name.Substring(1, fields[i].Name.Length - 1) + " = ");
                gen.Emit(OpCodes.Callvirt, stringBuilderAppend);
                gen.Emit(OpCodes.Pop);
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fields[i]);
                //ez itt nem jó
                gen.Emit(OpCodes.Box, fields[i].FieldType);
                gen.Emit(OpCodes.Callvirt, stringBuilderAppend2);
                gen.Emit(OpCodes.Pop);
            }

            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Ldstr, " }");
            gen.Emit(OpCodes.Callvirt, stringBuilderAppend);
            gen.Emit(OpCodes.Pop);
            gen.Emit(OpCodes.Ldloc_0);
            gen.Emit(OpCodes.Callvirt, stringBuilderToString);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br_S, label);
            gen.MarkLabel(label);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ret);


        }

        private static void GenerateEqualsIL(ILGenerator gen, FieldBuilder[] fields, TypeBuilder typeBuilder)
        {
            Type thisType = typeBuilder.MakeGenericType(fields.Select(f => f.FieldType).ToArray());

            LocalBuilder l0 = gen.DeclareLocal(thisType);
            LocalBuilder l1 = gen.DeclareLocal(typeof(bool));
            Label label82 = gen.DefineLabel();
            Label label83 = gen.DefineLabel();
            Label label86 = gen.DefineLabel();

            gen.Emit(OpCodes.Ldarg_1);
            gen.Emit(OpCodes.Isinst, thisType);
            gen.Emit(OpCodes.Stloc_0);
            gen.Emit(OpCodes.Ldloc_0);

            for (int i = 0; i < fields.Length; i++)
            {
                Type equalityComparerType = typeof(EqualityComparer<>).MakeGenericType(fields[i].FieldType);

                var defaultEqualityComparerFactoryMethod =
                    TypeBuilder.GetMethod(equalityComparerType, typeof(EqualityComparer<>).GetMethod("get_Default"));

                var equalsMethod =
                        TypeBuilder.GetMethod(equalityComparerType, typeof(EqualityComparer<>).GetMethods().Single(mi => mi.Name == "Equals" && mi.ContainsGenericParameters));

                //gen.Emit(OpCodes.Brfalse_S, label82);    http://www.jasonbock.net/jb/GeneratingExceptionBlocks.aspx
                gen.Emit(OpCodes.Brfalse, label82);
                gen.Emit(OpCodes.Call, defaultEqualityComparerFactoryMethod);
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldfld, fields[i]);
                gen.Emit(OpCodes.Ldloc_0);
                gen.Emit(OpCodes.Ldfld, fields[i]);
                gen.Emit(OpCodes.Callvirt, equalsMethod);


            }

            gen.Emit(OpCodes.Br_S, label83);
            gen.MarkLabel(label82);
            gen.Emit(OpCodes.Ldc_I4_0);
            gen.MarkLabel(label83);
            gen.Emit(OpCodes.Stloc_1);
            gen.Emit(OpCodes.Br_S, label86);
            gen.MarkLabel(label86);
            gen.Emit(OpCodes.Ldloc_1);
            gen.Emit(OpCodes.Ret);

        }

        #endregion

        private static void AppendPropertyToDisplayExpression(StringBuilder builder, string propertyName)
        {
            builder.Append(propertyName);
            builder.Append("={");
            builder.Append(propertyName);
            builder.Append("}");
        }
    }
}
