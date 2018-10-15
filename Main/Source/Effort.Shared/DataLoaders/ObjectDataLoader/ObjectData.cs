#if !EFOLD

namespace Effort.DataLoaders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Data.Entity.Core.Metadata.Edm;

    /// <summary>
    /// An object used to create and access collections of entities.
    /// </summary>
    public class ObjectData
    {
        private readonly IDictionary<string, IEnumerable> tables = new Dictionary<string, IEnumerable>();
        private readonly Func<string, string> generateTableName;

        /// <summary>
        /// Initialises a new instance of <c>ObjectData</c>.
        /// </summary>
        public ObjectData()
        {
        }

        internal virtual Guid Identifier { get; } = Guid.NewGuid();

        /// <summary>
        /// Returns the table specified by name. If a table with the specified name does not already exist, it will be created.
        /// </summary>
        /// <typeparam name="T">The type of entity that the table should contain.</typeparam>
        /// <param name="tableName">
        /// Name of the table.
        /// <remarks>
        /// If this value is null then the name of the entity will be used.
        /// </remarks>
        /// </param>
        /// <returns>The existing table with the specified name, if it exists. Otherwise, a new table will be created.</returns>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the table exists, but the element type specified is incorrect.
        /// </exception>
        /// <example>
        /// <code language="c#">
        /// public class Person
        /// {
        ///     public string Name { get; set; }
        /// }
        /// ...
        /// var data = new ObjectData();
        /// var table = data.Table&lt;Person>();
        /// table.Add(new Person { Name = "Fred" });
        /// table.Add(new Person { Name = "Jeff" });
        /// foreach (var person in data.Table&lt;Person>())
        /// {
        ///     Debug.Print(person.Name);
        /// }
        /// // prints:
        /// // Fred
        /// // Jeff
        /// </code>
        /// </example>
        public ObjectDataTable<T> Table<T>(string tableName = null)
        {
            tableName = tableName ?? typeof(T).Name;
            IEnumerable table;
            if (!tables.TryGetValue(tableName, out table) || table == null)
            {
                table = new ObjectDataTable<T>();
                tables[tableName] = table;
            }
            if (table is ObjectDataTable<T>)
            {
                return (ObjectDataTable<T>)table;
            }
            throw new InvalidOperationException($"A table with the name '{tableName}' already exists, but the element type is incorrect.\r\nExpected type: '{typeof(T).Name}'\r\nActual type: '{table.GetType().GetGenericArguments()[0].Name}'");
        }

        internal bool HasTable(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            return tables.ContainsKey(tableName);
        }

        internal string FindWithentitySet(EntitySet entitySet)
        {
            EntityContainer entityContainer = entitySet.EntityContainer;
            string name = null;
            foreach (var table in tables)
            {
                try
                {
                    if (entitySet == entityContainer.GetEntitySetByName(table.Key, true))
                    {
                        name = table.Key;
                    }
                }
                catch (ArgumentException e)
                { 
                } 
            }

            return name;
        }

        internal Type TableType(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            IEnumerable table;
            if (tables.TryGetValue(tableName, out table))
            {
                return table.GetType().GetGenericArguments()[0];
            }
            throw new InvalidOperationException($"No table with the name '{tableName}' defined.");
        }

        internal object GetTable(string tableName)
        {
            if (tableName == null) throw new ArgumentNullException(nameof(tableName));
            if (String.IsNullOrWhiteSpace(tableName)) throw new ArgumentException(nameof(tableName));
            IEnumerable table;
            tables.TryGetValue(tableName, out table);
            return table;
        }
    }
}
#endif