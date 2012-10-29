#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Concurrent;

namespace Effort.Internal.Caching
{
    internal class ConcurrentCache<TKey, TElement> 
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
            return this.Get(key, () => { throw new InvalidOperationException(); });
        }

        public void Remove(TKey key)
        {
            Lazy<TElement> value;
            this.store.TryRemove(key, out value);
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
