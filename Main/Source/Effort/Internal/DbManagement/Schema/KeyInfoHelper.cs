// --------------------------------------------------------------------------------------------
// <copyright file="KeyInfoHelper.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.DbManagement.Engine.Services;
    using Effort.Internal.TypeGeneration;
    using NMemory.Indexes;

    internal static class KeyInfoHelper
    {
        public static IKeyInfo CreateKeyInfo(Type entityType, MemberInfo[] properties)
        {
            var primaryKeySelector = CreateSelectorExpression(entityType, properties);

            return CreateKeyInfo(primaryKeySelector);
        }

        public static IKeyInfo CreateKeyInfo(LambdaExpression selector)
        {
            Type entityType = selector.Parameters[0].Type;
            Type resultType = selector.Body.Type;

            MethodInfo factory = ReflectionHelper
                .GetMethodInfo<DefaultKeyInfoFactory>(f => f.Create<object, object>(null))
                .GetGenericMethodDefinition()
                .MakeGenericMethod(entityType, resultType);

            IKeyInfo result = factory.Invoke(
                    new DefaultKeyInfoFactory(),
                    new object[] { selector }) as IKeyInfo;

            return result;
        }

        private static LambdaExpression CreateSelectorExpression(
            Type sourceType, 
            MemberInfo[] selectorFields)
        {
            Expression body = null;

            ParameterExpression param = Expression.Parameter(sourceType);
            Expression[] memberSelectors = new Expression[selectorFields.Length];

            for (int i = 0; i < memberSelectors.Length; i++)
            {
                memberSelectors[i] = Expression.MakeMemberAccess(param, selectorFields[i]);
            }



            if (memberSelectors.Length == 1)
            {
                body = memberSelectors[0];
            }
            else
            {
                Type[] memberTypes = memberSelectors.Select(e => e.Type).ToArray();

                if (TypeHelper.LargeTupleSize <= memberTypes.Length)
                {
                    throw new NotSupportedException("Primary key is too large");
                }

                Type resultType = CreateTupleType(memberTypes, 0);

                // TODO: Use TupleKeyInfoHelper instead
                body = Expression.New(resultType.GetConstructors()[0], memberSelectors);
            }

            return Expression.Lambda(body, param);
        }

        private static Type CreateTupleType(Type[] memberTypes, int index)
        {
            int memberCount = Math.Min(memberTypes.Length - index, TypeHelper.LargeTupleSize);

            Type[] args = new Type[memberCount];
            bool isLarge = false;

            if (TypeHelper.LargeTupleSize <= memberCount)
            {
                isLarge = true;
                memberCount--;
            }

            for (int i = 0; i < memberCount; i++)
            {
                args[i] = memberTypes[index + i];
            }

            if (isLarge)
            {
                // Last type is a tuple
                args[memberCount] = CreateTupleType(memberTypes, index + memberCount);
            }

            return TypeHelper.GetTupleType(args);
        }
    }
}
