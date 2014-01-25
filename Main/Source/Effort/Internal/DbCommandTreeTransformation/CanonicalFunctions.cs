// --------------------------------------------------------------------------------------------
// <copyright file="CanonicalFunctions.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
    using Effort.Internal.DbCommandTreeTransformation.Functions;

    internal class CanonicalFunctionMapper
    {
        private readonly Dictionary<string, Func<EdmFunction, Expression[], Expression>> mappings;
        private EdmTypeConverter converter;

        public CanonicalFunctionMapper(ITypeConverter converter)
        {
            this.converter = new EdmTypeConverter(converter);
            this.mappings = new Dictionary<string, Func<EdmFunction, Expression[], Expression>>();

            this.AddStringMappings();
            this.AddDateTimeMappings();
            this.AddMathMappings();
            this.AddBitwiseMappings();
            this.AddMiscMappings();
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

        private void AddMathMappings()
        {
            this.Map("Edm.Power", DoubleFunctions.Pow);

            this.MapMath("Edm.Ceiling",
                DecimalFunctions.Ceiling,
                DoubleFunctions.Ceiling);

            this.MapMath("Edm.Truncate",
                DecimalFunctions.Truncate,
                DoubleFunctions.Truncate);

            this.MapMath("Edm.Floor",
                DecimalFunctions.Floor,
                DoubleFunctions.Floor);

            this.mappings["Edm.Round"] = (f, args) => MapRound(f, args);

            this.mappings["Edm.Abs"] = (f, args) => MapAbs(f, args);
        }

        private void AddStringMappings()
        {
            this.Map("Edm.Concat", StringFunctions.Concat);

            this.Map("Edm.Contains", StringFunctions.Contains);

            this.Map("Edm.EndsWith", StringFunctions.EndsWith);

            this.Map("Edm.IndexOf", StringFunctions.IndexOf);

            this.Map("Edm.Left", StringFunctions.Left);

            this.Map("Edm.Length", StringFunctions.Length);

            this.Map("Edm.LTrim", StringFunctions.LTrim);

            this.Map("Edm.Replace", StringFunctions.Replace);

            this.Map("Edm.Reverse", StringFunctions.ReverseString);

            this.Map("Edm.Right", StringFunctions.Right);

            this.Map("Edm.RTrim", StringFunctions.RTrim);

            this.Map("Edm.StartsWith", StringFunctions.StartsWith);

            this.Map("Edm.Substring", StringFunctions.Substring);

            this.Map("Edm.ToLower", StringFunctions.ToLower);

            this.Map("Edm.ToUpper", StringFunctions.ToUpper);

            this.Map("Edm.Trim", StringFunctions.Trim);
        }

        private void AddDateTimeMappings()
        {
            this.Map("Edm.CurrentDateTime", 
                DateTimeFunctions.Current);

            this.Map("Edm.CurrentUtcDateTime", 
                DateTimeFunctions.CurrentUtc);

            this.Map("Edm.CurrentDateTimeOffset",
                DateTimeOffsetFunctions.Current);

            this.Map("Edm.CreateDateTime",
                DateTimeFunctions.CreateDateTime);

            this.Map("Edm.CreateDateTimeOffset", 
                DateTimeOffsetFunctions.CreateDateTimeOffset);

            this.Map("Edm.CreateTime", 
                TimeFunctions.CreateTime);

            this.Map("Edm.GetTotalOffsetMinutes", 
                DateTimeOffsetFunctions.GetTotalOffsetMinutes);

            this.MapDate("Edm.Year",
                DateTimeFunctions.GetYear,
                DateTimeOffsetFunctions.GetYear,
                null);

            this.MapDate("Edm.Month",
                DateTimeFunctions.GetMonth,
                DateTimeOffsetFunctions.GetMonth,
                null);

            this.MapDate("Edm.Day",
                DateTimeFunctions.GetDay,
                DateTimeOffsetFunctions.GetDay,
                null);

            this.MapDate("Edm.Hour",
                DateTimeFunctions.GetHour,
                DateTimeOffsetFunctions.GetHour,
                TimeFunctions.GetHour);

            this.MapDate("Edm.Minute",
                DateTimeFunctions.GetMinute,
                DateTimeOffsetFunctions.GetMinute,
                TimeFunctions.GetMinute);

            this.MapDate("Edm.Second",
                DateTimeFunctions.GetSecond,
                DateTimeOffsetFunctions.GetSecond,
                TimeFunctions.GetSecond);

            this.MapDate("Edm.Millisecond",
                DateTimeFunctions.GetMillisecond,
                DateTimeOffsetFunctions.GetMillisecond,
                TimeFunctions.GetMillisecond);

            this.MapDate("Edm.AddYears",
                DateTimeFunctions.AddYears,
                DateTimeOffsetFunctions.AddYears,
                null);

            this.MapDate("Edm.AddMonths",
                DateTimeFunctions.AddMonths,
                DateTimeOffsetFunctions.AddMonths,
                null);

            this.MapDate("Edm.AddDays",
                DateTimeFunctions.AddDays,
                DateTimeOffsetFunctions.AddDays,
                null);

            this.MapDate("Edm.AddHours",
                DateTimeFunctions.AddHours,
                DateTimeOffsetFunctions.AddHours,
                TimeFunctions.AddHours);

            this.MapDate("Edm.AddMinutes",
                DateTimeFunctions.AddMinutes,
                DateTimeOffsetFunctions.AddMinutes,
                TimeFunctions.AddMinutes);

            this.MapDate("Edm.AddSeconds",
                DateTimeFunctions.AddSeconds,
                DateTimeOffsetFunctions.AddSeconds,
                TimeFunctions.AddSeconds);

            this.MapDate("Edm.AddMilliseconds",
                DateTimeFunctions.AddMilliseconds,
                DateTimeOffsetFunctions.AddMilliseconds,
                TimeFunctions.AddMilliseconds);

            this.MapDate("Edm.AddMicroseconds",
                DateTimeFunctions.AddMicroseconds,
                DateTimeOffsetFunctions.AddMicroseconds,
                TimeFunctions.AddMicroseconds);

            this.MapDate("Edm.AddNanoseconds",
                DateTimeFunctions.AddNanoseconds,
                DateTimeOffsetFunctions.AddNanoseconds,
                TimeFunctions.AddNanoseconds);

            this.MapDate("Edm.DiffYears",
                DateTimeFunctions.DiffYears,
                DateTimeOffsetFunctions.DiffYears,
                null);

            this.MapDate("Edm.DiffMonths",
                DateTimeFunctions.DiffMonths,
                DateTimeOffsetFunctions.DiffMonths,
                null);

            this.MapDate("Edm.DiffDays",
                DateTimeFunctions.DiffDays,
                DateTimeOffsetFunctions.DiffDays,
                null);

            this.MapDate("Edm.DiffHours",
                DateTimeFunctions.DiffHours,
                DateTimeOffsetFunctions.DiffHours,
                TimeFunctions.DiffHours);

            this.MapDate("Edm.DiffMinutes",
                DateTimeFunctions.DiffMinutes,
                DateTimeOffsetFunctions.DiffMinutes,
                TimeFunctions.DiffMinutes);

            this.MapDate("Edm.DiffSeconds",
                DateTimeFunctions.DiffSeconds,
                DateTimeOffsetFunctions.DiffSeconds,
                TimeFunctions.DiffSeconds);

            this.MapDate("Edm.DiffMilliseconds",
                DateTimeFunctions.DiffMilliseconds,
                DateTimeOffsetFunctions.DiffMilliseconds,
                TimeFunctions.DiffMilliseconds);

            this.MapDate("Edm.DiffMicroseconds",
                DateTimeFunctions.DiffMicroseconds,
                DateTimeOffsetFunctions.DiffMicroseconds,
                TimeFunctions.DiffMicroseconds);

            this.MapDate("Edm.DiffNanoseconds",
                DateTimeFunctions.DiffNanoseconds,
                DateTimeOffsetFunctions.DiffNanoseconds,
                TimeFunctions.DiffNanoseconds);

            this.MapDate("Edm.TruncateTime",
                DateTimeFunctions.TruncateTime,
                DateTimeOffsetFunctions.TruncateTime,
                null);

            this.MapDate("Edm.DayOfYear",
                DateTimeFunctions.DayOfYear,
                DateTimeOffsetFunctions.DayOfYear,
                null);
        }

        private static MethodCallExpression MapRound(EdmFunction f, Expression[] args)
        {
            MethodInfo method = null;

            switch(args.Length)
            {
                case 1:
                    method = IsDecimal(f.Parameters[0]) ?
                        DecimalFunctions.Round :
                        DoubleFunctions.Round;
                    break;
                case 2:
                    method = IsDecimal(f.Parameters[0]) ?
                        DecimalFunctions.RoundDigits :
                        DoubleFunctions.RoundDigits;
                    break;
            }

            if (method == null)
            {
                throw new NotSupportedException(
                    string.Format(
                        "'{0}' function with {1} args is not supported",
                        f.FullName,
                        args.Length));
            }

            return Expression.Call(null, method, args);
        }

        private static MethodCallExpression MapAbs(EdmFunction f, Expression[] args)
        {
            return Expression.Call(
                null,
                GetAbsMethod(f.Parameters[0]),
                args[0]);
        }

        private static MethodInfo GetAbsMethod(FunctionParameter param)
        {
            PrimitiveType primitive = param.TypeUsage.EdmType as PrimitiveType;

            if (primitive == null)
            {
                return DoubleFunctions.Abs;
            }

            Type clrType = primitive.ClrEquivalentType;

            if (clrType == typeof(decimal))
            {
                return DecimalFunctions.Abs;
            }
            else if (clrType == typeof(long))
            {
                return IntegerFunctions.Abs64;
            }
            else if (clrType == typeof(int))
            {
                return IntegerFunctions.Abs32;
            }
            else if (clrType == typeof(short))
            {
                return IntegerFunctions.Abs16;
            }
            else if (clrType == typeof(sbyte))
            {
                return IntegerFunctions.Abs8;
            }

            return DoubleFunctions.Abs;
        }

        private void Map(string name, MethodInfo method)
        {
            this.Map(
                name, 
                (f, args) => 0, 
                method);
        }

        private void MapDate(string name, 
            MethodInfo dateTime, 
            MethodInfo dateTimeOffset,
            MethodInfo time)
        {
            this.Map(
                name, 
                (f, args) => SelectDateTimeMethod(f, args), 
                dateTime, 
                dateTimeOffset,
                time);
        }

        private void MapMath(string name,
            MethodInfo decimalMethod,
            MethodInfo doubleMethod)
        {
            this.Map(
                name,
                (f, args) => SelectMathMethod(f, args),
                decimalMethod,
                doubleMethod);
        }

        private void Map(
            string name, 
            Func<EdmFunction, Expression[], int> methodSelector,
            params MethodInfo[] methods)
        {
            this.mappings[name] = (f, args) =>
            {
                int i = methodSelector(f, args);

                if ((i < 0 && methods.Length <= i))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "Invalid method selector for '{0}' edm function",
                            f.FullName));
                }

                var method = methods[i];

                if (method == null)
                {
                    throw new NotSupportedException(
                        string.Format(
                            "'{0}' function is not supported with signature ({1})",
                            f.FullName,
                            ""));
                }

                args = FixArguments(method, args);

                return Expression.Call(null, method, args);
            };
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

        private static int SelectMathMethod(EdmFunction function, Expression[] args)
        {
            if (IsDecimal(function.Parameters[0]))
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        private static int SelectDateTimeMethod(EdmFunction function, Expression[] args)
        {
            Type firstArg = TypeHelper.MakeNotNullable(args[0].Type);

            if (firstArg == typeof(DateTime))
            {
                return 0;
            }
            else if (firstArg == typeof(DateTimeOffset))
            {
                return 1;
            }
            else if (firstArg == typeof(TimeSpan))
            {
                return 2;
            }

            throw new NotSupportedException(
                string.Format("Type '{2}' is not supported for '{0}' date function ",
                    function.FullName, 
                    firstArg.Name));
        }

        private static Expression[] FixArguments(MethodInfo method, Expression[] args)
        {
            var converted = new Expression[args.Length];
            var methodParams = method.GetParameters();

            for (int i = 0; i < args.Length; i++)
            {
                Expression expr = args[i];
                Type expected = methodParams[i].ParameterType;

                if (expr.Type != expected)
                {
                    expr = Expression.Convert(expr, expected);
                }

                converted[i] = expr;
            }
            return converted;
        }


        public Expression CreateMethodCall(EdmFunction function, Expression[] arguments)
        {
            Func<EdmFunction, Expression[], Expression> mapper = null;

            if (!this.mappings.TryGetValue(function.FullName, out mapper))
            {
                throw new NotSupportedException(
                    string.Format(
                        "Missing mapping for '{0}' function",
                        function.FullName));
            }

            return mapper(function, arguments);
        }
    }
}