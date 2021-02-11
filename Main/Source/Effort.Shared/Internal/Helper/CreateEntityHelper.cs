using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using NMemory.Tables;

namespace Effort.Shared.Internal
{
    public static class CreateEntityHelper
    {
        public static ConcurrentDictionary<string, Func<object[], object>> DictCreateAndInsertEntityDelegate = new ConcurrentDictionary<string, Func<object[], object>>();
        
        public static object Create(ITable table, IList<MemberBinding> memberBindings)
        {
            if (memberBindings.All(x => x is MemberAssignment memberAssignment
                                        && (memberAssignment.Expression is ConstantExpression
                                            || (memberAssignment.Expression is UnaryExpression unaryExpression && unaryExpression.Operand is ConstantExpression))))
            {


                // CREATE key by combining the table and all member name
                var key = table.GetHashCode() + ";zzz;" + string.Join(";", memberBindings.Select(x => x.Member.Name));

                // CHECK if already compiled, otherwise we compile it
                if (!DictCreateAndInsertEntityDelegate.TryGetValue(key, out var factory))
                {
                    // PARAMETER
                    var parameterValues = Expression.Parameter(typeof(object[]));

                    // CREATE new entity // code: var entity = new [EntityType]()
                    var variableEntity = Expression.Variable(table.EntityType);
                    Expression expressionNewEntity = Expression.New(table.EntityType);
                    expressionNewEntity = Expression.Assign(variableEntity, expressionNewEntity);

                    // CREATE the code block
                    var expressionBlock = new List<Expression>();
                    expressionBlock.Add(variableEntity);
                    expressionBlock.Add(expressionNewEntity);

                    // FOREACH property, assign the value
                    for (int i = 0; i < memberBindings.Count; i++)
                    {
                        var property = (PropertyInfo)memberBindings[i].Member;
                        var value = Expression.ArrayIndex(parameterValues, Expression.Constant(i));
                        var assign = Expression.Assign(Expression.Property(variableEntity, property), Expression.Convert(value, property.PropertyType));

                        expressionBlock.Add(assign);
                    }

                    // RETURN the entity
                    var returnTarget = Expression.Label(typeof(object));
                    expressionBlock.Add(Expression.Return(returnTarget, variableEntity));
                    expressionBlock.Add(Expression.Label(returnTarget, Expression.Constant(null)));

                    // CREATE the block
                    var block = Expression.Block(new List<ParameterExpression>() { variableEntity }, expressionBlock);

                    // COMPILE the lambda
                    factory = Expression.Lambda<Func<object[], object>>(block, parameterValues).Compile();
                    DictCreateAndInsertEntityDelegate[key] = factory;
                }

                // SELECT values
                var values = memberBindings.Cast<MemberAssignment>().Select(x =>
                {
                    object value = null;

                    if (x.Expression is ConstantExpression constantExpression1)
                    {
                        value = constantExpression1.Value;
                    }
                    else if (x.Expression is UnaryExpression unaryExpression && unaryExpression.Operand is ConstantExpression constantExpression2)
                    {
                        value = constantExpression2.Value;
                    }

                    return value;
                }).ToArray();

                // CREATE the new entity
                object newEntity = factory(values);

                return newEntity;
            }
            else
            {
                // KEEP old logic and compile every time
                LambdaExpression expression =
                    Expression.Lambda(
                        Expression.MemberInit(
                            Expression.New(table.EntityType),
                            memberBindings));

                var factory = expression.Compile();

                object newEntity = factory.DynamicInvoke();

                return newEntity;
            }
        }
    }
}
