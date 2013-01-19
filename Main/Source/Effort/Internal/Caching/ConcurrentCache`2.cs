// --------------------------------------------------------------------------------------------
// <copyright file="ConcurrentCache`2.cs" company="Effort Team">
//     Copyright (C) 2012-2013 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.Caching
{
    using System;
    using System.Collections.Concurrent;

    /// <summary>
    ///     Represents a thread-safe generic dictionary-like cache.
    /// </summary>
    /// <typeparam name="TKey"> The type of the key. </typeparam>
    /// <typeparam name="TElement"> The type of the elements. </typeparam>
    internal class ConcurrentCache<TKey, TElement> 
    {
        /// <summary>
        ///     The internal store.
        /// </summary>
        private ConcurrentDictionary<TKey, Lazy<TElement>> store;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConcurrentCache{TKey,TElement}" /> 
        ///     class.
        /// </summary>
        public ConcurrentCache() 
        {
            this.store = new ConcurrentDictionary<TKey, Lazy<TElement>>();
        }

        /// <summary>
        ///     Gets the element associated with the specified key.
        /// </summary>
        /// <param name="key"> The key that identifies the cached element. </param>
        /// <returns> The cached element. </returns>
        public TElement Get(TKey key)
        {
            return this.Get(key, () => { throw new InvalidOperationException(); });
        }

        /// <summary>
        ///     Gets the element associated with the specified key. If no such element exists,
        ///     it is initialized by the supplied factory method.
        /// </summary>
        /// <param name="key"> The key that identifies the cached element. </param>
        /// <param name="factory"> The element factory method. </param>
        /// <returns> The queried element. </returns>
        public TElement Get(TKey key, Func<TElement> factory)
        {
            Lazy<TElement> element = 
                this.store.GetOrAdd(
                    key, 
                    k => new Lazy<TElement>(factory, true));

            // Evaluate the value (maybe it will initialize now)
            return element.Value;
        }

        /// <summary>
        ///     Determines whether the store containes an element associated to the specified
        ///     key.
        /// </summary>
        /// <param name="key"> 
        ///     The key that identifies the cached element. 
        /// </param>
        /// <returns>
        ///     <c>true</c> if it contains the appropriate element otherwise, <c>false</c>.
        /// </returns>
        public bool Contains(TKey key)
        {
            return this.store.ContainsKey(key);
        }

        /// <summary>
        ///     Removes the element associate to the specified key.
        /// </summary>
        /// <param name="key"> The key that identifies the cached element. </param>
        public void Remove(TKey key)
        {
            Lazy<TElement> value;
            this.store.TryRemove(key, out value);
        }
    }
}
