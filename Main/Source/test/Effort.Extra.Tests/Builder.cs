
namespace Effort.Extra.Tests
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Reflection;
    using Effort.DataLoaders;

    internal static class Builder
    {
        public static TableDescription CreateTableDescription(string name, Type type)
        {
            var ctor = typeof(TableDescription).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)
                .First(c => c.GetParameters().Length == 2);
            var columns = type.GetProperties().Select(CreateColumnDescription).ToArray();

            return (TableDescription)ctor.Invoke(new object[] { name, columns });
        }

        private static ColumnDescription CreateColumnDescription(PropertyInfo property)
        {
            var ctor = typeof(ColumnDescription).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic).First();
            return (ColumnDescription)ctor.Invoke(new object[] { GetPropertyName(property), property.PropertyType });
        }

        private static string GetPropertyName(PropertyInfo property)
        {
            var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
            return columnAttribute != null ? columnAttribute.Name : property.Name;
        }
    }
}