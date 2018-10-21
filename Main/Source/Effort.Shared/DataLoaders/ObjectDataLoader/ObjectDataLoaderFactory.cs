// All credits for ObjectDataLoader (Effort.Extra): Chris Rodgers
// GitHub: https://github.com/christophano

namespace Effort.DataLoaders
{
    using System;
    using Effort.DataLoaders;

    /// <summary>
    /// Implementation of <see cref="ITableDataLoaderFactory"/> for <see cref="ObjectData"/>.
    /// </summary>
    internal class ObjectDataLoaderFactory : ITableDataLoaderFactory
    {
        private static readonly Type LoaderType = typeof(ObjectTableDataLoader<>);
        private readonly ObjectData data;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataLoaderFactory"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ObjectDataLoaderFactory(ObjectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            this.data = data;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose() { }

        /// <summary>
        /// Creates a data loader for the specified table.
        /// </summary>
        /// <param name="table">The metadata of the table.</param>
        /// <returns>
        /// The data loader for the table.
        /// </returns>
        public ITableDataLoader CreateTableDataLoader(TableDescription table)
        {
            if (table == null) throw new ArgumentNullException(nameof(table));
            if (data.HasTable(table.Name))
            {
                var entityType = data.TableType(table.Name);
                var type = LoaderType.MakeGenericType(entityType);
                var constructor = type.GetConstructor(new[]
                {
                    typeof (TableDescription),
                    typeof (ObjectDataTable<>).MakeGenericType(entityType)
                });
                return (ITableDataLoader)constructor?.Invoke(new[] { table, data.GetTable(table.Name) });
            }

            string name = data.FindWithEntitySet(table.TableInfo.EntitySet);
            if (name != null && data.HasTable(name))
            {
                var entityType = data.TableType(name);
                var type = LoaderType.MakeGenericType(entityType);
                var constructor = type.GetConstructor(new[]
                {
                    typeof (TableDescription),
                    typeof (ObjectDataTable<>).MakeGenericType(entityType)
                });
                return (ITableDataLoader)constructor?.Invoke(new[] { table, data.GetTable(name) });
            }

            return new EmptyTableDataLoader();
        }
    }
}
