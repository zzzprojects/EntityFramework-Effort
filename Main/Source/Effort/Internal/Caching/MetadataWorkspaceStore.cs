// --------------------------------------------------------------------------------------------
// <copyright file="MetadataWorkspaceStore.cs" company="Effort Team">
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
    using System.Data.Metadata.Edm;

    /// <summary>
    ///     Represents a cache that stores <see cref="MetadataWorkspace"/> object.
    /// </summary>
    internal class MetadataWorkspaceStore
    {
        /// <summary>
        ///     Internal collection.
        /// </summary>
        private static ConcurrentCache<string, MetadataWorkspace> store;

        /// <summary>
        ///     Initializes static members the <see cref="MetadataWorkspaceStore" /> class.
        /// </summary>
        static MetadataWorkspaceStore()
        {
            store = new ConcurrentCache<string, MetadataWorkspace>();
        }

        /// <summary>
        ///     Returns a <see cref="MetadataWorkspace"/> object that derived from the 
        ///     specified metadata in order to be compatible with the Effort provider. If no 
        ///     such element exist, the specified factory method is used to create one.
        /// </summary>
        /// <param name="metadata">
        ///     References the metadata resource.
        /// </param>
        /// <param name="workspaceFactoryMethod">
        ///     The factory method that instantiates the desired element.
        /// </param>
        /// <returns>
        ///     The MetadataWorkspace object.
        /// </returns>
        public static MetadataWorkspace GetMetadataWorkspace(
            string metadata, 
            Func<string, MetadataWorkspace> workspaceFactoryMethod)
        {
            return store.Get(metadata, () => workspaceFactoryMethod(metadata));
        }
    }
}
