using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading;

namespace MMDB.EntityFrameworkProvider.Helpers
{
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

            FieldBuilder fb = tb.DefineField(memberName,
            type,
            FieldAttributes.Private);
            // Define a property named Number that gets and sets the private 
            // field.
            //
            // The last argument of DefineProperty is null, because the
            // property has no parameters. (If you don't specify null, you must
            // specify an array of Type objects. For a parameterless property,
            // use the built-in array with no elements: Type.EmptyTypes)
            PropertyBuilder pb = tb.DefineProperty(
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
            MethodBuilder mbGetAccessor = tb.DefineMethod(
                getMethodName,
                getSetAttr,
                type,
                Type.EmptyTypes);

            ILGenerator numberGetIL = mbGetAccessor.GetILGenerator();
            // For an instance property, argument zero is the instance. Load the 
            // instance, then load the private field and return, leaving the
            // field value on the stack.
            numberGetIL.Emit(OpCodes.Ldarg_0);
            numberGetIL.Emit(OpCodes.Ldfld, fb);
            numberGetIL.Emit(OpCodes.Ret);

            // Define the "set" accessor method for Number, which has no return
            // type and takes one argument of type int (Int32).
            MethodBuilder mbSetAccessor = tb.DefineMethod(
                setMethodName,
                getSetAttr,
                null,
                new Type[] { type });

            ILGenerator numberSetIL = mbSetAccessor.GetILGenerator();
            // Load the instance and then the numeric argument, then store the
            // argument in the field.
            numberSetIL.Emit(OpCodes.Ldarg_0);
            numberSetIL.Emit(OpCodes.Ldarg_1);
            numberSetIL.Emit(OpCodes.Stfld, fb);
            numberSetIL.Emit(OpCodes.Ret);

            // Last, map the "get" and "set" accessor methods to the 
            // PropertyBuilder. The property is now complete. 
            pb.SetGetMethod(mbGetAccessor);
            pb.SetSetMethod(mbSetAccessor);

            return pb;
        }
    }
}
