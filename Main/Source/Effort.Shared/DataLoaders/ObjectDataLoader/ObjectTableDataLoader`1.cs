
namespace Effort.DataLoaders
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Microsoft.CSharp.RuntimeBinder;
    using Binder = Microsoft.CSharp.RuntimeBinder.Binder;

    internal class ObjectTableDataLoader<T> : ITableDataLoader
    {
        private const BindingFlags PropertyFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private readonly TableDescription description;
        private readonly ObjectDataTable<T> table;
        private readonly Lazy<Func<T, object[]>> formatter;

        public ObjectTableDataLoader(TableDescription description, ObjectDataTable<T> table)
        {
            if (description == null) throw new ArgumentNullException(nameof(description));
            if (table == null) throw new ArgumentNullException(nameof(table));
            this.description = description;
            this.table = table;
            formatter = new Lazy<Func<T, object[]>>(CreateFormatter);
        }

        protected Func<T, object[]> CreateFormatter()
        {
            var type = typeof(T);
            var param = Expression.Parameter(type, "x");
            var initialisers = description.Columns
                .Select(column => new { Property = GetProperty(type, column), Column = column })
                .Select(a => ToExpression(param, a.Property, a.Column))
                .Select(expression => CastExpression(expression));
            var newArray = Expression.NewArrayInit(typeof(object), initialisers);
            return Expression.Lambda<Func<T, object[]>>(newArray, param).Compile();
        }

        private static PropertyInfo GetProperty(Type parentType, ColumnDescription column)
        {
            return parentType.GetProperty(column.Name, PropertyFlags)
                   ?? parentType.GetProperties(PropertyFlags)
                                .SingleOrDefault(p => MatchColumnAttribute(p, column));
        }

        private string GetDiscriminator(T item)
        {
            return table.GetDiscriminator(item);
        }

        private Expression ToExpression(ParameterExpression parameter, PropertyInfo property, ColumnDescription column)
        {
            if (property == null)
            {
                if (column.Name == table.DiscriminatorColumn)
                {
                    return Expression.Call(Expression.Constant(table), typeof(ObjectDataTable<T>).GetMethod(nameof(GetDiscriminator), BindingFlags.Instance | BindingFlags.NonPublic), parameter);
                }
                var binder = Binder.GetMember(CSharpBinderFlags.None, column.Name, typeof(ObjectData),
                    new[] { CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, null) });
                var expression = Expression.Dynamic(binder, typeof(object), parameter);
                return Expression.TryCatch(expression, Expression.Catch(typeof(RuntimeBinderException), Expression.Constant(null)));
            }
            return Expression.Property(parameter, property);
        }

        private static Expression CastExpression(Expression expression)
        {
            return Expression.TypeAs(expression, typeof(object));
        }

        private static bool MatchColumnAttribute(PropertyInfo property, ColumnDescription column)
        {
            var columnAttribute = property.GetCustomAttributes(typeof(ColumnAttribute), true).FirstOrDefault();
            if (columnAttribute == null) return false;
            return ((ColumnAttribute)columnAttribute).Name == column.Name;
        }

        public IEnumerable<object[]> GetData()
        {
            var results = table.Select(formatter.Value);
            return results;
        }
    }
}
