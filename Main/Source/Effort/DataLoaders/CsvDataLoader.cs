// ----------------------------------------------------------------------------------
// <copyright file="CsvDataLoader.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.DataLoaders
{
    /// <summary>
    /// Represents a data loader that reads data from CSV files.
    /// </summary>
    public class CsvDataLoader : IDataLoader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataLoader" /> class.
        /// </summary>
        public CsvDataLoader()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvDataLoader" /> class.
        /// </summary>
        /// <param name="path">The path of the folder that contains the CSV files.</param>
        public CsvDataLoader(string path)
        {
            this.Argument = path;
        }

        /// <summary>
        /// Gets or sets the argument that contains the path of the folder where the CSV
        /// files are located.
        /// </summary>
        /// <value>
        /// The argument.
        /// </value>
        public string Argument
        {
            get;
            set;
        }

        /// <summary>
        /// Creates a <see cref="CsvTableDataLoaderFactory" /> instance.
        /// </summary>
        /// <returns>
        /// A <see cref="CsvTableDataLoaderFactory" /> instance.
        /// </returns>
        public ITableDataLoaderFactory CreateTableDataLoaderFactory()
        {
            return new CsvTableDataLoaderFactory(this.Argument);
        }
    }
}
