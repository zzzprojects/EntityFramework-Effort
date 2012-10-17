﻿#region License

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
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.TypeConversion;

namespace Effort.Internal.DbCommandTreeTransformation
{
	internal class CanonicalFunctionMapper
	{
		private Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>> mappings;
		private EdmTypeConverter converter;

        /// <summary>
        /// The function is invoked on the LAST argument!
        /// </summary>
        /// <param name="converter"></param>
		public CanonicalFunctionMapper(ITypeConverter converter)
		{
			this.converter = new EdmTypeConverter(converter);
			this.mappings = new Dictionary<string, Func<EdmFunction, Expression[], MethodCallExpression>>();

			// Mappingek leirasa: http://msdn.microsoft.com/en-us/library/bb738681.aspx

            //String

            this.mappings["Edm.Concat"] = (f, args) => Expression.Call(null,
                FindMethod(typeof(string), "Concat", typeof(string), typeof(string)), args[0], args[1]);

			this.mappings["Edm.ToUpper"] = (f, args) => Expression.Call(args[0],
				FindMethod(typeof(string), "ToUpper"));
			this.mappings["Edm.ToLower"] = (f, args) => Expression.Call(args[0],
				FindMethod(typeof(string), "ToLower"));

            this.mappings["Edm.IndexOf"] = (f, args) => Expression.Call(null,
                 FindMethod(typeof(DbFunctions), "IndexOf", typeof(string), typeof(string)), args[1], args[0]);

            this.mappings["Edm.Trim"] = (f, args) => Expression.Call(args[0],
                FindMethod(typeof(string), "Trim"));

            this.mappings["Edm.Length"] = (f, args) => Expression.Call(args[0],typeof(string).GetProperties().Where(x => x.Name == "Length").First().GetGetMethod());

            this.mappings["Edm.Reverse"] = (f, args) => Expression.Call(null,
                 FindMethod(typeof(DbFunctions), "ReverseString", typeof(string)), args[0]);

            this.mappings["Edm.Substring"] = (f, args) => Expression.Call(null,
                 FindMethod(typeof(DbFunctions), "Substring", typeof(string), typeof(int?), typeof(int?)), args[0], args[1], args[2]);

            this.mappings["Edm.Replace"] = (f, args) => Expression.Call(args[0],
                FindMethod(typeof(string), "Replace", typeof(string), typeof(string)),args[1],args[2]);
            
             
            //Mathematical Function Mapping
            
            this.mappings["Edm.Abs"] = (f, args) => Expression.Call(null,
                FindMethod(typeof(DbFunctions), "Abs", typeof(decimal?)), args[0]);

            this.mappings["Edm.Round"] = (f, args) => Expression.Call(null,
               FindMethod(typeof(Math), "Round", this.converter.ConvertNotNull(f.Parameters[0].TypeUsage)),
               ExpressionHelper.ConvertToNotNull(args[0]));

		}

		public MethodCallExpression CreateMethodCall(EdmFunction function, Expression[] arguments)
		{
			try
			{
				return this.mappings[function.FullName](function, arguments);
			}
			catch (KeyNotFoundException exp)
			{
				throw new InvalidOperationException("There is no matching CLR function in MMDB for function " + function.FullName + '.', exp);
			}
		}
		private MethodInfo FindMethod(Type type, string methodName, params Type[] parameterTypes)
		{
			return type.GetMethods().Where(mi => mi.Name == methodName
				&& mi.GetParameters().Count() == parameterTypes.Length
				&& mi.GetParameters().Select(p => p.ParameterType).SequenceEqual(parameterTypes)
				).Single();
		}


	}
}