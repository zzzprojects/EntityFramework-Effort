// --------------------------------------------------------------------------------------------
// <copyright file="DbContainerStore.cs" company="Effort Team">
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
    using System;
    using Effort.Internal.DbManagement;

    /// <summary>
    /// Represents a cache that stores <see cref="DbContainer"/> objects.
    /// </summary>
    internal class DbContainerStore
    {
        /// <summary>
        /// Internal collection.
        /// </summary>
        private static ConcurrentCache<string, DbContainer> store;

        /// <summary>
        /// Initializes static members of the <see cref="DbContainerStore" /> class.
        /// </summary>
        static DbContainerStore()
        {
            store = new ConcurrentCache<string, DbContainer>();
        }

        /// <summary>
        /// Returns a <see cref="DbContainer"/> object identified by the specified instance
        /// identifier. If no such element exist, the specified factory method is used to
        /// create one.
        /// </summary>
        /// <param name="instanceId">The instance id.</param>
        /// <param name="databaseFactoryMethod">The database factory method.</param>
        /// <returns>The  <see cref="DbContainer"/> object.</returns>
        public static DbContainer GetDbContainer(
            string instanceId, 
            Func<DbContainer> databaseFactoryMethod)
        {
            return store.Get(instanceId, databaseFactoryMethod);
        }

        /// <summary>
        /// Removes the DbContainer associated to the specified identifier from the cache.
        /// </summary>
        /// <param name="instanceId">The instance identifier.</param>
        public static void RemoveDbContainer(string instanceId)
        {
            store.Remove(instanceId);
        }
    }
}
