// All credits for ObjectDataLoader (Effort.Extra): Chris Rodgers
// GitHub: https://github.com/christophano

namespace Effort.DataLoaders
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using Effort.DataLoaders;

    /// <summary>
    /// An implementation of <c>IDataLoader</c> for <c>ObjectData</c>.
    /// </summary>
    public class ObjectDataLoader : IDataLoader
    {
        private static readonly ConcurrentDictionary<Guid, ObjectData> DataCollection = new ConcurrentDictionary<Guid, ObjectData>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataLoader"/> class.
        /// </summary>
        public ObjectDataLoader() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataLoader"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ObjectDataLoader(ObjectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));
            Argument = data.Identifier.ToString();
            DataCollection.AddOrUpdate(data.Identifier, data, (key, value) => data);
        }

        /// <summary>
        /// Gets or sets the argument that describes the complete state of the data loader.
        /// </summary>
        /// <value>
        /// The argument.
        /// </value>
        public string Argument { get; set; }

        /// <summary>
        /// Creates a table data loader factory.
        /// </summary>
        /// <returns>
        /// A table data loader factory.
        /// </returns>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">
        /// Thrown if no object data with a key matching the <see cref="Argument"/> is held in the <see cref="DataCollection"/>.
        /// </exception>
        /// <exception cref="System.InvalidOperationException">
        /// Thrown if the <see cref="Argument"/> is not a valid <see cref="Guid"/>.
        /// </exception>
        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            if (Guid.TryParse(Argument, out var id))
            {
                if (DataCollection.TryGetValue(id, out var data))
                {
                    return new ObjectDataLoaderFactory(data);
                }
                throw new KeyNotFoundException($"The key '{id}' was not found in the data collection.");
            }
            throw new InvalidOperationException($"Unable to parse '{Argument}' as a guid.");
        }
    }
}