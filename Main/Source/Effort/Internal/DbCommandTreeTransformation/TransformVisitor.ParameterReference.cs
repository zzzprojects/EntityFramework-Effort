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
using System.Data.Common.CommandTrees;
using System.Data.Metadata.Edm;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.DbCommandTreeTransformation.Join;
using Effort.Internal.DbCommandTreeTransformation.Variables;
using Effort.Internal.TypeConversion;
using Effort.Internal.TypeGeneration;

namespace Effort.Internal.DbCommandTreeTransformation
{
    internal partial class TransformVisitor
    {
        public override Expression Visit(DbParameterReferenceExpression expression)
        {
            //object value = this.parameterValues[expression.ParameterName].Value;

            //typeConverter.Convert(this.parameters[expression.ParameterName]);

            Type type = edmTypeConverter.Convert(expression.ResultType);

            var parameterPlaceholderConstructor = typeof(NMemory.StoredProcedures.Parameter<>)
                .MakeGenericType(type)
                .GetConstructors()
                .Single(c => c.GetParameters().Count() == 1);

            var parameter = Expression.New(parameterPlaceholderConstructor, Expression.Constant(expression.ParameterName));
            var convertedParameter = Expression.Convert(parameter, type);

            return convertedParameter;
        }
    }
}