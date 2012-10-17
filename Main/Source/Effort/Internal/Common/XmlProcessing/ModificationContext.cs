namespace Effort.Internal.Common.XmlProcessing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    internal class ModificationContext : IModificationContext
    {
        private IDictionary<string, object> elements;

        public ModificationContext()
        {
            this.elements = new Dictionary<string, object>();
        }

        public void Set<T>(string key, T element)
            where T : class
        {
            this.elements[key] = element;
        }

        public T Get<T>(string key, T failback)
            where T : class
        {
            object value;

            if (!this.elements.TryGetValue(key, out value))
            {
                return failback;
            }

            return value as T;
        }
    }
}
