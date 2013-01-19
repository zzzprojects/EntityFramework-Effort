// --------------------------------------------------------------------------------------------
// <copyright file="FakeCachingTableDataLoaderStoreProxy.cs" company="Effort Team">
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

namespace Effort.Test.Internal.Fakes
{
    using System;
    using System.Collections.Generic;
    using Effort.DataLoaders;
    using Effort.Internal.Caching;

    internal class FakeCachingTableDataLoaderStoreProxy : ICachingTableDataLoaderStore
    {
        private Dictionary<CachingTableDataLoaderKey, CachingTableDataLoader> simpleCache;

        public FakeCachingTableDataLoaderStoreProxy()
        {
            this.simpleCache = 
                new Dictionary<CachingTableDataLoaderKey, CachingTableDataLoader>();

            this.CachedItemReturnCount = 0;
        }

        public int CachedItemReturnCount 
        { 
            get; 
            set; 
        }

        public CachingTableDataLoader GetCachedData(
            CachingTableDataLoaderKey key,
            Func<CachingTableDataLoader> factoryMethod)
        {
            CachingTableDataLoader result = null;

            if (!this.simpleCache.TryGetValue(key, out result))
            {
                result = factoryMethod.Invoke();
                this.simpleCache.Add(key, result);
            }
            else
            {
                this.CachedItemReturnCount++;
            }

            return result;
        }


        public bool Contains(CachingTableDataLoaderKey key)
        {
            return this.simpleCache.ContainsKey(key);
        }
    }
}
