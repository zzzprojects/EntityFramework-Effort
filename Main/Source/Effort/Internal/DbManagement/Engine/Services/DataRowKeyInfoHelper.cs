// --------------------------------------------------------------------------------------------
// <copyright file="DataRowKeyInfoHelper.cs" company="Effort Team">
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

namespace Effort.Internal.DbManagement.Engine.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using Effort.Internal.TypeGeneration;
    using NMemory.Indexes;

    internal class DataRowKeyInfoHelper : IKeyInfoHelper
    {
        private readonly Type type;
        private readonly PropertyInfo[] properties;
        private readonly bool isLarge;
        private readonly ConstructorInfo ctor;

        public DataRowKeyInfoHelper(Type type)
        {
            this.type = type;

            this.properties = type
                .GetProperties()
                .Select(p => new {
                    Property = p,
                    Attribute = p.GetCustomAttributes(false)
                        .OfType<DataRowPropertyAttribute>()
                        .SingleOrDefault()
                })
                .Where(x => x.Attribute != null)
                .OrderBy(x => x.Attribute.Index)
                .Select(x => x.Property)
                .ToArray();

            this.ctor = type
                .GetConstructors()
                .Single();

            this.isLarge = this.type
                .GetCustomAttributes(false)
                .OfType<LargeDataRowAttribute>()
                .Any();
        }

        public Expression CreateKeyFactoryExpression(params Expression[] arguments)
        {
            Expression[] args = new Expression[this.properties.Length];

            if (args.Length != arguments.Length)
            {
                throw new ArgumentException("", "arguments");	 
            }

            for (int i = 0; i < args.Length; i++)
            {
                args[i] = arguments[i];

                Type propertyType = this.properties[i].PropertyType;

                if (propertyType != args[i].Type)
                {
                    args[i] = Expression.Convert(args[i], propertyType);
                }

                if (this.isLarge)
                {
                    args[i] = Expression.Convert(args[i], typeof(object));
                }
            }

            if (this.isLarge)
            {
                Expression array = Expression.NewArrayInit(typeof(object), args);

                return Expression.New(this.ctor, array);
            }
            else
            {
                return Expression.New(this.ctor, args);
            }
        }

        public Expression CreateKeyMemberSelectorExpression(Expression source, int index)
        {
            return Expression.MakeMemberAccess(source, this.properties[index]);
        }

        public int GetMemberCount()
        {
            return this.properties.Length;
        }

        public bool TryParseKeySelectorExpression(
            Expression keySelector, 
            bool strict, 
            out MemberInfo[] result)
        {
            if (keySelector == null)
            {
                throw new ArgumentNullException("keySelector");
            }

            if (keySelector.Type != this.type)
            {
                result = null;
                return false;
            }

            NewExpression resultCreator = keySelector as NewExpression;

            if (resultCreator == null)
            {
                result = null;
                return false;
            }

            Expression[] args = resultCreator.Arguments.ToArray();

            if (this.isLarge)
            {
                if (resultCreator.Arguments.Count != 1)
                {
                    result = null;
                    return false;
                }

                NewArrayExpression array = resultCreator.Arguments[0] as NewArrayExpression;

                if (array == null)
                {
                    result = null;
                    return false;
                }

                args = array.Expressions.ToArray();
            }

            List<MemberInfo> resultList = new List<MemberInfo>();

            foreach (Expression arg in args)
            {
                Expression expr = arg;

                if (!strict || this.isLarge)
                {
                    expr = ExpressionHelper.SkipConversionNodes(expr);
                }

                MemberExpression member = expr as MemberExpression;

                if (member == null)
                {
                    result = null;
                    return false;
                }

                resultList.Add(member.Member);
            }

            result = resultList.ToArray();
            return true;
        }
    }
}
