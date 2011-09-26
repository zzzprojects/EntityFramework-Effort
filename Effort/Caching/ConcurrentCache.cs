using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Collections.Concurrent;

namespace MMDB.EntityFrameworkProvider.Caching
{
    public class ConcurrentCache<TKey, TElement> 
    {
        private Func<TKey, TElement> defaultFactory;

        private ConcurrentDictionary<TKey, Lazy<TElement>> store;

        public ConcurrentCache() : this(null)
        {

        }

        public ConcurrentCache(Func<TKey, TElement> defaultFactory)
        {
            this.store = new ConcurrentDictionary<TKey, Lazy<TElement>>();

            this.defaultFactory = defaultFactory;
        }

        public TElement Get(TKey key)
        {
            if (defaultFactory == null)
            {
                throw new InvalidOperationException("Default factory is not set during the initialization");
            }

            return this.Get(key, () => defaultFactory(key));
        }
        
        public TElement Get(TKey key, Func<TElement> factory)
        {
            Lazy<TElement> element = 
                this.store.GetOrAdd(
                    key, 
                    k => new Lazy<TElement>(factory, true));

            // Return the value (maybe it will initialize now)
            return element.Value;
        }


    }
}
