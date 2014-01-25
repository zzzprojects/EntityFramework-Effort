// --------------------------------------------------------------------------------------------
// <copyright file="FileSource.cs" company="Effort Team">
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
    using Effort.DataLoaders.Internal;

    /// <summary>
    ///     Represents a source of files.
    /// </summary>
    public class FileSource
    {
        private readonly string path;
        private readonly IFileProvider provider;

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileSource"/> class.
        /// </summary>
        /// <param name="path"> The path representing the source. </param>
        public FileSource(string path)
        {
            this.provider = GetProvider(path);
            this.path = path;           
        }

        /// <summary>
        ///     Gets a value indicating whether the source is valid and containing CSV files.
        /// </summary>
        /// <value>
        ///   <c>true</c> if valid; otherwise, <c>false</c>.
        /// </value>
        public bool IsValid
        {
            get 
            { 
                return this.provider.IsValid; 
            }
        }

        /// <summary>
        ///     The path that represents the source.
        /// </summary>
        /// <value>
        ///     The path.
        /// </value>
        public string Path
        {
            get 
            { 
                return this.path; 
            }
        }

        /// <summary>
        ///     Returns the specified file contained by this soruce.
        /// </summary>
        /// <param name="name">The name of the file.</param>
        /// <returns> Reference for the requested file. </returns>
        public IFileReference GetFile(string name)
        {
            return this.provider.GetFile(name);
        }

        private IFileProvider GetProvider(string path)
        {
            Uri uri = null;

            Uri.TryCreate(path, UriKind.Absolute, out uri);

            if (uri == null)
            {
                return new InvalidFileProvider();
            }

            switch (uri.Scheme)
            {
                case "res":
                    return new ResourceFileProvider(uri);
                case "file":
                    return new FileSystemFileProvider(uri);
                default:
                    return new InvalidFileProvider();
            }
        }
    }
}
