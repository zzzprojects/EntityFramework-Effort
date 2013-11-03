// --------------------------------------------------------------------------------------------
// <copyright file="CanonicalFunctions.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbCommandTreeTransformation
{
    using System;
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.TypeConversion;

    internal class CanonicalFunctionMapper
    {
        private Dictionary<string, Func<EdmFunction, Expression[], Expression>> mappings;
        private EdmTypeConverter converter;

        public CanonicalFunctionMapper(ITypeConverter converter)
        {
            //// Still missing:
            //// 3 System.Decimal. functions
            //// System.TimeSpan
            //// System.DateTimeOffset
            //// Math.Round with digits

            this.converter = new EdmTypeConverter(converter);
            this.mappings = new Dictionary<string, Func<EdmFunction, Expression[], Expression>>();

            //// Description of canonical functions: http://msdn.microsoft.com/en-us/library/bb738681.aspx

            AddStringMappings();
            AddDateTimeMappings();
            AddArithmeticMappings();
            AddBitwiseMappings();
            AddMiscMappings();
        }

        private void AddMiscMappings()
        {
            this.mappings["Edm.NewGuid"] = (f, args) =>
                Expression.Call(null, ReflectionHelper.GetMethodInfo(() => Guid.NewGuid()));
        }

        private void AddBitwiseMappings()
        {
            this.mappings["Edm.BitwiseOr"] = (f, args) =>
                Expression.Or(args[0], args[1]);

            this.mappings["Edm.BitwiseAnd"] = (f, args) =>
                Expression.And(args[0], args[1]);

            this.mappings["Edm.BitwiseXor"] = (f, args) =>
                Expression.ExclusiveOr(args[0], args[1]);

            this.mappings["Edm.BitwiseNot"] = (f, args) =>
                Expression.Not(args[0]);
        }

        private void AddArithmeticMappings()
        {
            this.mappings["Edm.Ceiling"] = (f, args) =>
                Expression.Call(
                    null, 
                    IsDecimal(f.Parameters[0]) ? 
                        DbFunctions.CeilingDecMethod :
                        DbFunctions.CeilingMethod, 
                    args[0]);

            this.mappings["Edm.Truncate"] = (f, args) =>
                Expression.Call(
                    null,
                    IsDecimal(f.Parameters[0]) ?
                        DbFunctions.TruncateDecMethod :
                        DbFunctions.TruncateMethod,
                    args[0]);

            this.mappings["Edm.Floor"] = (f, args) =>
                Expression.Call(
                    null,
                    IsDecimal(f.Parameters[0]) ?
                        DbFunctions.FloorDecMethod :
                        DbFunctions.FloorMethod,
                    args[0]);

            this.mappings["Edm.Round"] = (f, args) =>
                args.Length > 1 ?
                    Expression.Call(
                        IsDecimal(f.Parameters[0]) ?
                            DbFunctions.RoundDigitsDecMethod :
                            DbFunctions.RoundDigitsMethod,
                        args[0],
                        args[1]) :
                    Expression.Call(
                        IsDecimal(f.Parameters[0]) ?
                            DbFunctions.RoundDecMethod :
                            DbFunctions.RoundMethod,
                        args[0]);

            this.mappings["Edm.Abs"] = (f, args) =>
                Expression.Call(
                    null,
                    GetAbsMethod(f.Parameters[0]),
                    args[0]);

            this.mappings["Edm.Power"] = (f, args) =>
                Expression.Call(
                    null,
                    DbFunctions.PowMethod,
                    args[0],
                    args[1]);
        }

        private void AddStringMappings()
        {
            //// String
            this.mappings["Edm.Concat"] = (f, args) =>
                Expression.Call(
                    null,
                    FindMethod(typeof(string), "Concat", typeof(string), typeof(string)),
                    args[0],
                    args[1]);

            this.mappings["Edm.ToUpper"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "ToUpper"));

            this.mappings["Edm.ToLower"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "ToLower"));

            this.mappings["Edm.IndexOf"] = (f, args) =>
                Expression.Call(
                    null,
                    FindMethod(typeof(DbFunctions), "IndexOf", typeof(string), typeof(string)),
                    args[1],
                    args[0]);

            this.mappings["Edm.Trim"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "Trim"));

            this.mappings["Edm.RTrim"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "TrimEnd", typeof(char[])),
                    Expression.Constant(new char[0]));

            this.mappings["Edm.LTrim"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "TrimStart", typeof(char[])),
                    Expression.Constant(new char[0]));

            this.mappings["Edm.Length"] = (f, args) =>
                Expression.Call(
                    args[0],
                    typeof(string).GetProperties().Where(x => x.Name == "Length").First().GetGetMethod());

            this.mappings["Edm.Reverse"] = (f, args) =>
                Expression.Call(
                    null,
                    FindMethod(typeof(DbFunctions), "ReverseString", typeof(string)),
                    args[0]);

            this.mappings["Edm.Substring"] = (f, args) =>
                Expression.Call(
                    null,
                    FindMethod(typeof(DbFunctions), "Substring", typeof(string), typeof(int?), typeof(int?)),
                    args[0],
                    args[1],
                    args[2]);

            this.mappings["Edm.Replace"] = (f, args) =>
                Expression.Call(
                    args[0],
                    FindMethod(typeof(string), "Replace", typeof(string), typeof(string)),
                    args[1],
                    args[2]);
        }

        private void AddDateTimeMappings()
        {
            //// DateTime
            this.mappings["Edm.CurrentDateTime"] = (f, args) =>
                Expression.Call(
                    null,
                    typeof(DateTime).GetProperties().Where(x => x.Name == "Now").First().GetGetMethod());

            this.mappings["Edm.CurrentUtcDateTime"] = (f, args) =>
                Expression.Call(
                    null,
                    typeof(DateTime).GetProperties().Where(x => x.Name == "UtcNow").First().GetGetMethod());

            this.mappings["Edm.CurrentDateTimeOffset"] = (f, args) =>
                Expression.Call(
                    null,
                    typeof(DateTimeOffset).GetProperties().Where(x => x.Name == "Now").First().GetGetMethod());

            this.mappings["Edm.AddYears"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddYears",
                        typeof(Int32)),
                         ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(Int32))));

            this.mappings["Edm.Year"] = (f, args) =>
                CreateDateExpression(args, "Year");

            this.mappings["Edm.AddMonths"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddMonths",
                        typeof(Int32)),
                         ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(Int32))));

            this.mappings["Edm.Month"] = (f, args) =>
                CreateDateExpression(args, "Month");

            this.mappings["Edm.AddDays"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddDays",
                        typeof(double)),
                         ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(double))));

            this.mappings["Edm.Day"] = (f, args) =>
                CreateDateExpression(args, "Day");

            this.mappings["Edm.AddHours"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddHours",
                        typeof(double)),
                         ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(double))));

            this.mappings["Edm.Hour"] = (f, args) =>
                CreateDateExpression(args, "Hour");

            this.mappings["Edm.AddMinutes"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddMinutes",
                        typeof(double)),
                        ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(double))));

            this.mappings["Edm.Minute"] = (f, args) =>
                CreateDateExpression(args, "Minute");

            this.mappings["Edm.AddSeconds"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddSeconds",
                        typeof(double)),
                        ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(double))));

            this.mappings["Edm.Second"] = (f, args) =>
                CreateDateExpression(args, "Second");

            this.mappings["Edm.AddMilliseconds"] = (f, args) =>
                Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "AddMilliseconds",
                        typeof(double)),
                        ExpressionHelper.ConvertToNotNull(Expression.Convert(args[1], typeof(double))));

            this.mappings["Edm.Millisecond"] = (f, args) =>
                CreateDateExpression(args, "Millisecond");

            this.mappings["Edm.TruncateTime"] = (f, args) =>
                    Expression.Call(ExpressionHelper.ConvertToNotNull(args[0]), FindMethod(typeof(DateTime), "get_Date"));
        }

        private static MethodInfo GetAbsMethod(FunctionParameter param)
        {
            PrimitiveType primitive = param.TypeUsage.EdmType as PrimitiveType;

            if (primitive == null)
            {
                return DbFunctions.AbsMethod;
            }

            Type clrType = primitive.ClrEquivalentType;

            if (clrType == typeof(decimal))
            {
                return DbFunctions.AbsMethodDec;
            }
            else if (clrType == typeof(long))
            {
                return DbFunctions.AbsMethod64;
            }
            else if (clrType == typeof(int))
            {
                return DbFunctions.AbsMethod32;
            }
            else if (clrType == typeof(short))
            {
                return DbFunctions.AbsMethod16;
            }
            else if (clrType == typeof(sbyte))
            {
                return DbFunctions.AbsMethod8;
            }

            return DbFunctions.AbsMethod;
        }

        private static bool IsDecimal(FunctionParameter param)
        {
            PrimitiveType primitive = param.TypeUsage.EdmType as PrimitiveType;

            if (primitive == null)
            {
                return false;
            }

            return primitive.ClrEquivalentType == typeof(decimal);
        }

        public Expression CreateMethodCall(EdmFunction function, Expression[] arguments)
        {
            try
            {
                return this.mappings[function.FullName](function, arguments);
            }
            catch (KeyNotFoundException exp)
            {
                throw new InvalidOperationException("Missing function mapping for " + function.FullName + '.', exp);
            }
        }

        private static MethodCallExpression CreateDateExpression(Expression[] args, string propertyName)
        {
            Expression source = ExpressionHelper.ConvertToNotNull(args[0]);
            Type sourceType = source.Type;

            PropertyInfo property = sourceType.GetProperty(propertyName);

            return Expression.Call(source, property.GetGetMethod());
        }

        private static MethodInfo FindMethod(Type type, string methodName, params Type[] parameterTypes)
        {
            return type
                .GetMethods()
                .Single(mi =>
                    mi.Name == methodName &&
                    mi.GetParameters().Count() == parameterTypes.Length &&
                    mi.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes));
        }
    }
}