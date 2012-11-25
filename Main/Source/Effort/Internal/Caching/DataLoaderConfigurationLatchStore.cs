// --------------------------------------------------------------------------------------------
// <copyright file="DataLoaderConfigurationLatchStore.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    /// <summary>
    /// Represents a cache that stores <see cref="T:DataLoaderConfigurationLatch"/> objects.
    /// </summary>
    internal static class DataLoaderConfigurationLatchStore
    {
        /// <summary>
        /// Internal collection.
        /// </summary>
        private static ConcurrentCache<
            DataLoaderConfigurationKey,
            DataLoaderConfigurationLatch> store;

        /// <summary>
        /// Initializes static members of the the 
        /// <see cref="DataLoaderConfigurationLatchStore" /> 
        /// class.
        /// </summary>
        static DataLoaderConfigurationLatchStore()
        {
            store = new ConcurrentCache<
                DataLoaderConfigurationKey,
                DataLoaderConfigurationLatch>();
        }

        /// <summary>
        /// Return the latch associated to specified data loader configuration
        /// </summary>
        /// <param name="key">Identifies the data loader configuration.</param>
        /// <returns>The configuration latch.</returns>
        public static DataLoaderConfigurationLatch GetLatch(
            DataLoaderConfigurationKey key)
        {
            return store.Get(key, () => new DataLoaderConfigurationLatch());
        }
    }
}
