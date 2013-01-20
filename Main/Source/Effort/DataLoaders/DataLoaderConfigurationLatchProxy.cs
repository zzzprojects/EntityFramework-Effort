// --------------------------------------------------------------------------------------------
// <copyright file="DataLoaderConfigurationLatchProxy.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

namespace Effort.DataLoaders
{
    using System;
    using Effort.Internal.Caching;

    /// <summary>
    ///     Represents a proxy towards the appropriate 
    ///     <see cref="T:Effort.Internal.Caching.DataLoaderConfigurationLatch"/> object.
    /// </summary>
    internal sealed class DataLoaderConfigurationLatchProxy : 
        IDataLoaderConfigurationLatch, 
        IDisposable
    {
        /// <summary>
        ///     Indicates is the latch is acquired.
        /// </summary>
        private bool aquired;

        /// <summary>
        ///     The key that identifies the latch.
        /// </summary>
        private DataLoaderConfigurationKey key;

        /// <summary>
        ///     The global configuration latch.
        /// </summary>
        private DataLoaderConfigurationLatch latch;

        /// <summary>
        ///     Initializes a new instance of the 
        ///     <see cref="DataLoaderConfigurationLatchProxy" /> class.
        /// </summary>
        /// <param name="key"> The key that identifies the global latch. </param>
        public DataLoaderConfigurationLatchProxy(DataLoaderConfigurationKey key)
        {
            this.aquired = false;
            this.key = key;
        }

        /// <summary>
        ///     Finalizes an instance of the <see cref="DataLoaderConfigurationLatchProxy" /> 
        ///     class.
        /// </summary>
        ~DataLoaderConfigurationLatchProxy()
        {
            GC.SuppressFinalize(this);
            this.Dispose(false);
        }

        /// <summary>
        ///     Acquires the configuration latch.
        /// </summary>
        public void Acquire()
        {
            if (this.aquired)
            {
                return;
            }

            if (this.latch == null)
            {
                this.latch = DataLoaderConfigurationLatchStore.GetLatch(this.key);
            }

            this.latch.Acquire();
            this.aquired = true;
        }

        /// <summary>
        ///     Releases the configuration latch.
        /// </summary>
        public void Release()
        {
            if (!this.aquired)
            {
                return;
            }

            this.latch.Release();
            this.aquired = false;

            // The latch is not removed from the cache
        }

        /// <summary>
        ///     Releases the configuration latch.
        /// </summary>
        void IDisposable.Dispose()
        {
            this.Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            this.Release();
        }
    }
}
