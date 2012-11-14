// --------------------------------------------------------------------------------------------
// <copyright file="CanonicalFunctions.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.TypeConversion;

    internal class CanonicalFunctionMapper
    {
        private Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>> mappings;
        private EdmTypeConverter converter;

        public CanonicalFunctionMapper(ITypeConverter converter)
        {
            //// Still missing:
            //// 3 System.Decimal. functions
            //// System.TimeSpan
            //// System.DateTimeOffset
            //// Math.Round with digits

            this.converter = new EdmTypeConverter(converter);
            this.mappings = new Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>>();

            //// Description of canonical functions: http://msdn.microsoft.com/en-us/library/bb738681.aspx

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

            this.mappings["Edm.Year"] = (f, args) =>
                CreateDateExpression(args, "Year");

            this.mappings["Edm.Month"] = (f, args) =>
                CreateDateExpression(args, "Month");

            this.mappings["Edm.Day"] = (f, args) =>
                CreateDateExpression(args, "Day");

            this.mappings["Edm.Hour"] = (f, args) =>
                CreateDateExpression(args, "Hour");

            this.mappings["Edm.Minute"] = (f, args) =>
                CreateDateExpression(args, "Minute");

            this.mappings["Edm.Second"] = (f, args) =>
                CreateDateExpression(args, "Second");

            this.mappings["Edm.Millisecond"] = (f, args) =>
                CreateDateExpression(args, "Millisecond");

            //// Arithmetic
            this.mappings["Edm.Ceiling"] = (f, args) => 
                Expression.Call(
                    null,
                    FindMethod(typeof(Math), "Ceiling", this.converter.ConvertNotNull(f.Parameters[0].TypeUsage)),
                    ExpressionHelper.ConvertToNotNull(args[0]));

            this.mappings["Edm.Floor"] = (f, args) => 
                Expression.Call(
                    null,
                    FindMethod(typeof(Math), "Floor", this.converter.ConvertNotNull(f.Parameters[0].TypeUsage)),
                    ExpressionHelper.ConvertToNotNull(args[0]));

            this.mappings["Edm.Round"] = (f, args) => 
                Expression.Call(
                    null,
                    FindMethod(
                        typeof(Math), 
                        "Round", 
                        this.converter.ConvertNotNull(f.Parameters[0].TypeUsage),
                        args.Count() > 1 ? 
                            this.converter.ConvertNotNull(f.Parameters[1].TypeUsage) : 
                            typeof(int)),
                    ExpressionHelper.ConvertToNotNull(args[0]), 
                        args.Count() > 1 ? 
                            ExpressionHelper.ConvertToNotNull(args[1]) : 
                            Expression.Constant(0));

            this.mappings["Edm.Abs"] = (f, args) => 
                Expression.Call(
                    null,
                    FindMethod(typeof(Math), "Abs", this.converter.ConvertNotNull(f.Parameters[0].TypeUsage)),
                    ExpressionHelper.ConvertToNotNull(args[0]));

            this.mappings["Edm.Power"] = (f, args) => 
                Expression.Call(
                    null,
                    FindMethod(
                        typeof(Math), 
                        "Pow",
                        this.converter.ConvertNotNull(f.Parameters[0].TypeUsage),
                        this.converter.ConvertNotNull(f.Parameters[1].TypeUsage)),
                    ExpressionHelper.ConvertToNotNull(args[0]), 
                    ExpressionHelper.ConvertToNotNull(args[1]));
        }

        public MethodCallExpression CreateMethodCall(EdmFunction function, Expression[] arguments)
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