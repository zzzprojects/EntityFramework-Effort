#if !EFOLD
namespace Effort.DataLoaders
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a collection of object data entities.
    /// </summary>
    /// <typeparam name="T">The type of entity that this table stores.</typeparam>
    /// <seealso cref="System.Collections.Generic.IList{T}" />
    public class ObjectDataTable<T> : IList<T>
    {
        private readonly IList<T> list = new List<T>();
        private readonly IDictionary<Type, string> discriminators = new Dictionary<Type, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectDataTable{T}"/> class.
        /// </summary>
        internal ObjectDataTable() { }

        /// <summary>
        /// Gets or sets the discriminator column name.
        /// </summary>
        /// <value>
        /// The discriminator column name.
        /// </value>
        public string DiscriminatorColumn { get; set; } = "Discriminator";

        /// <summary>
        /// Adds a discriminator value for the given type.
        /// </summary>
        /// <typeparam name="TType">The type of entity.</typeparam>
        /// <param name="discriminator">The discriminator value.</param>
        public void AddDiscriminator<TType>(string discriminator) where TType : T
        {
            if (!discriminators.ContainsKey(typeof(TType)))
            {
                discriminators.Add(typeof(TType), discriminator);
            }
            discriminators[typeof(TType)] = discriminator;
        }

        /// <summary>
        /// Gets the discriminator value for the given type.
        /// </summary>
        /// <typeparam name="T">The type of entity.</typeparam>
        /// <returns>The discriminator value.</returns>
        internal string GetDiscriminator(T item)
        {
            if (item == null) throw new ArgumentNullException(nameof(item));
            var type = item.GetType();
            string discriminator;
            if (!discriminators.TryGetValue(type, out discriminator))
            {
                discriminator = type.Name;
                discriminators.Add(type, discriminator);
            }
            return discriminator;
        }

        #region IList<T>

        public T this[int index]
        {
            get { return list[index]; }
            set { list[index] = value; }
        }

        public int Count => list.Count;

        public bool IsReadOnly => list.IsReadOnly;

        public IEnumerator<T> GetEnumerator()
        {
            return list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)list).GetEnumerator();
        }

        public void Add(T item)
        {
            list.Add(item);
        }

        public void Clear()
        {
            list.Clear();
        }

        public bool Contains(T item)
        {
            return list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            return list.Remove(item);
        }

        public int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        #endregion
    }
}
#endif