// --------------------------------------------------------------------------------------------
// <copyright file="DbTableInformation.cs" company="Effort Team">
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
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;
    using NMemory.Indexes;
    using System.Collections.Generic;

    internal class DbTableInformation
    {
        private FastLazy<Func<object[], object>> initializer;

        public DbTableInformation(
            string tableName, 
            Type entityType, 
            PropertyInfo[] primaryKeys, 
            PropertyInfo identityField, 
            PropertyInfo[] properties,
            object[] constraints, 
            IKeyInfo primaryKeyInfo)
        {
            this.TableName = tableName;
            this.EntityType = entityType;
            this.PrimaryKeyFields = primaryKeys;
            this.IdentityField = identityField;
            this.Properties = properties;
            this.Constraints = constraints;
            this.PrimaryKeyInfo = primaryKeyInfo;

            this.initializer = new FastLazy<Func<object[], object>>(CreateEntityInitializer);
        }

        public string TableName { get; set; }

        public Type EntityType { get; private set; }

        public PropertyInfo[] PrimaryKeyFields { get; private set; }

        public PropertyInfo IdentityField { get; private set; }

        public PropertyInfo[] Properties { get; private set; }

        // NMemory.Constraints.IConstraint<TEntity> array
        public object[] Constraints { get; private set; }

        public IKeyInfo PrimaryKeyInfo { get; private set; }

        public Func<object[], object> EntityInitializer
        {
            get { return this.initializer.Value; }
        }

        private Func<object[], object> CreateEntityInitializer()
        {
            ParameterExpression parameter = Expression.Parameter(typeof(object[]));

            ParameterExpression result = Expression.Variable(this.EntityType);
            List<Expression> blockElements = new List<Expression>();

            blockElements.Add(Expression.Assign(result, Expression.New(this.EntityType)));

            for (int i = 0; i < this.Properties.Length; i++)
            {
                blockElements.Add(
                    Expression.Assign(
                        Expression.Property(result, this.Properties[i]),
                        Expression.Convert(
                            Expression.ArrayIndex(parameter, Expression.Constant(i)),
                            this.Properties[i].PropertyType)));
            }

            blockElements.Add(result);

            Expression body =
                Expression.Block(
                    this.EntityType,
                    new ParameterExpression[] { result },
                    blockElements.ToArray());

            return Expression.Lambda<Func<object[], object>>(body, parameter).Compile();
        }
    }
}
