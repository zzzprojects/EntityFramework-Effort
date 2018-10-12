// --------------------------------------------------------------------------------------------
// <copyright file="DbTableInfo.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Exceptions;
    using Effort.Internal.Common;
    using NMemory.Indexes;

    public class DbTableInfo
    {
        private FastLazy<Func<object[], object>> initializer;

        public DbTableInfo(
            TableName tableName, 
            Type entityType, 
            MemberInfo identityField,
            PropertyInfo[] properties,
            IKeyInfo primaryKeyInfo,
            IKeyInfo[] uniqueKeys,
            IKeyInfo[] foreignKeys,
            object[] constraintFactories)
        {
            this.TableName = tableName;
            this.EntityType = entityType;
            this.IdentityField = identityField;
            this.Properties = properties;
            this.ConstraintFactories = constraintFactories;
            this.PrimaryKeyInfo = primaryKeyInfo;
            this.UniqueKeys = uniqueKeys;
            this.ForeignKeys = foreignKeys;

            this.initializer = new FastLazy<Func<object[], object>>(CreateEntityInitializer);
        }

        public TableName TableName { get; private set; }

        public Type EntityType { get; private set; }

        public MemberInfo IdentityField { get; private set; }

        public PropertyInfo[] Properties { get; private set; }

        // NMemory.Constraints.IConstraint<TEntity> array
        public object[] ConstraintFactories { get; private set; }

        public IKeyInfo PrimaryKeyInfo { get; private set; }

        public IKeyInfo[] ForeignKeys { get; private set; }

        public IKeyInfo[] UniqueKeys { get; private set; }


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

            Expression<Action<Exception, PropertyInfo, object>> handleException =
                (exception, property, value) => 
                    HandleConvertException(exception, property, value);

            ParameterExpression caught = Expression.Parameter(typeof(Exception));
            var valueExpression = Expression.Variable(typeof(object), "value");
            
            for (int i = 0; i < this.Properties.Length; i++)
            {
                blockElements.Add(
                    Expression.TryCatch(
                        Expression.Block(typeof(void),
                            Expression.Assign(
                                valueExpression, 
                                Expression.ArrayIndex(parameter, Expression.Constant(i))),
                            Expression.Assign(
                                Expression.Property(
                                    result, 
                                    this.Properties[i]),
                                Expression.Convert(
                                    valueExpression, 
                                    this.Properties[i].PropertyType))),
                        Expression.Catch(
                            caught, 
                            Expression.Invoke(
                                handleException, 
                                caught, 
                                Expression.Constant(Properties[i]), valueExpression))));
            }

            blockElements.Add(result);

            Expression body =
                Expression.Block(
                    this.EntityType,
                    new ParameterExpression[] { result, valueExpression },
                    blockElements.ToArray());

            return Expression.Lambda<Func<object[], object>>(body, parameter).Compile();
        }

        private void HandleConvertException(Exception exception, PropertyInfo property, object value)
        {
            string message = 
                string.Format(
                    ExceptionMessages.EntityPropertyAssignFailed,
                    value ?? "[null]",
                    property.Name,
                    property.PropertyType,
                    TableName);

            throw new EffortException(message, exception);
        }
    }
}
