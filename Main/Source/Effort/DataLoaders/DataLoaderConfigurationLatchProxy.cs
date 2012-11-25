// --------------------------------------------------------------------------------------------
// <copyright file="DataLoaderConfigurationLatchProxy.cs" company="Effort Team">
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

namespace Effort.DataLoaders
{
    using System;
    using Effort.Internal.Caching;

    internal sealed class DataLoaderConfigurationLatchProxy : 
        IDataLoaderConfigurationLatch, 
        IDisposable
    {
        private bool aquired;
        private DataLoaderConfigurationKey key;
        private DataLoaderConfigurationLatch latch;

        public DataLoaderConfigurationLatchProxy(DataLoaderConfigurationKey key)
        {
            this.aquired = false;
            this.key = key;
        }

        ~DataLoaderConfigurationLatchProxy()
        {
            GC.SuppressFinalize(this);
            this.Dispose(false);
        }

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
