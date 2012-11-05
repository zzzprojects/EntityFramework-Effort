// ----------------------------------------------------------------------------------
// <copyright file="EmitHelper.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.Common
{
    using System;
    using System.Reflection;
    using System.Reflection.Emit;

    internal static class EmitHelper
    {
        public static PropertyBuilder AddProperty<T>(TypeBuilder tb, string name)
        {
            return AddProperty(tb, name, typeof(T));
        }

        public static PropertyBuilder AddProperty(TypeBuilder tb, string name, Type type)
        {
            string memberName = "_" + name;
            string setMethodName = "set_" + name;
            string getMethodName = "get_" + name;
            string propName = name;

            FieldBuilder field = tb.DefineField(memberName, type, FieldAttributes.Private);

            // Define a property named Number that gets and sets the private 
            // field.
            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use the built-in array with no elements: Type.EmptyTypes)
            PropertyBuilder property = 
                tb.DefineProperty(
                    propName,
                    System.Reflection.PropertyAttributes.HasDefault,
                    type,
                    null);

            // The property "set" and property "get" methods require a special
            // set of attributes.
            MethodAttributes getSetAttr = MethodAttributes.Public |
                MethodAttributes.SpecialName | MethodAttributes.HideBySig;

            // Define the "get" accessor method for Number. The method returns
            // an integer and has no arguments. (Note that null could be 
            // used instead of Types.EmptyTypes)
            MethodBuilder getAccessor = 
                tb.DefineMethod(
                    getMethodName,
                    getSetAttr,
                    type,
                    Type.EmptyTypes);

            ILGenerator numberGetIL = getAccessor.GetILGenerator();

            // For an instance property, argument zero is the instance. Load the 
            // instance, then load the private field and return, leaving the
            // field value on the stack.
            numberGetIL.Emit(OpCodes.Ldarg_0);
            numberGetIL.Emit(OpCodes.Ldfld, field);
            numberGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for Number, which has no return
            // type and takes one argument of type int (Int32).
            MethodBuilder setAccessor = tb.DefineMethod(
                setMethodName,
                getSetAttr,
                null,
                new Type[] { type });

            ILGenerator numberSetIL = setAccessor.GetILGenerator();

            // Load the instance and then the numeric argument, then store the
            // argument in the field.
            numberSetIL.Emit(OpCodes.Ldarg_0);
            numberSetIL.Emit(OpCodes.Ldarg_1);
            numberSetIL.Emit(OpCodes.Stfld, field);
            numberSetIL.Emit(OpCodes.Ret);

            // Last, map the "get" and "set" accessor methods to the 
            // PropertyBuilder. The property is now complete. 
            property.SetGetMethod(getAccessor);
            property.SetSetMethod(setAccessor);

            return property;
        }
    }
}
