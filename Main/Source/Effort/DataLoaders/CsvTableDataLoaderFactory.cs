// --------------------------------------------------------------------------------------------
// <copyright file="CsvTableDataLoaderFactory.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
    using System.IO;

    /// <summary>
    ///     Represents a table data loader factory that creates 
    ///     <see cref="CsvTableDataLoader" /> instances for tables.
    /// </summary>
    internal class CsvTableDataLoaderFactory : ITableDataLoaderFactory
    {
        private readonly FileSource source;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvTableDataLoaderFactory" /> 
        ///     class.
        /// </summary>
        /// <param name="source"> The source of CSV files. </param>
        /// <exception cref="System.ArgumentException"> The path does not exists. </exception>
        public CsvTableDataLoaderFactory(FileSource source)
        {
            this.source = source;

            if (!this.source.IsValid)
            {
                throw new ArgumentException(
                    string.Format("Path \"{0}\" does not exists", source.Path), 
                    "path");
            }
        }

        /// <summary>
        ///     Creates a <see cref="CsvTableDataLoader" /> instance for the specified table.
        /// </summary>
        /// <param name="table">
        ///     The metadata of the table. 
        /// </param>
        /// <returns>
        ///     The <see cref="CsvTableDataLoader" /> instance for the table.
        /// </returns>
        public ITableDataLoader CreateTableDataLoader(TableDescription table)
        {
            string name = string.Format("{0}.csv", table.Name);
            IFileReference file = this.source.GetFile(name);

            if (file == null || !file.Exists)
            {
                return new EmptyTableDataLoader();
            }

            return new CsvTableDataLoader(file, table);
        }

        /// <summary>
        ///     Does nothing.
        /// </summary>
        public void Dispose()
        {
        }
    }
}
