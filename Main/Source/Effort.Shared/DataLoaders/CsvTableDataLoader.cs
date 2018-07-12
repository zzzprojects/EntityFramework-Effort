// --------------------------------------------------------------------------------------------
// <copyright file="CsvTableDataLoader.cs" company="Effort Team">
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
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using Effort.Internal.Common;
    using Effort.Internal.Csv;

    /// <summary>
    ///     Represent a table data loader that retrieves data from a CSV file.
    /// </summary>
    public class CsvTableDataLoader : TableDataLoaderBase
    {
        private readonly IFileReference file;
        private readonly IValueConverter valueConverter;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvTableDataLoader" /> class.
        /// </summary>
        /// <param name="file"> The file reference to the CSV file. </param>
        /// <param name="table"> The metadata of the requested table. </param>
        public CsvTableDataLoader(IFileReference file, TableDescription table) 
            : base(table)
        {
            // TODO: Constructor injection
            this.valueConverter = new CsvValueConverter();
            this.file = file;
        }

        /// <summary>
        ///     Creates initial data for the table.
        /// </summary>
        /// <returns>
        ///     The data created for the table.
        /// </returns>
        public override IEnumerable<object[]> GetData()
        {
            if (!this.file.Exists)
            {
                yield break;
            }

            foreach (object[] record in base.GetData())
            {
                yield return record;
            }
        }

        /// <summary>
        ///     Creates a CSV data reader that retrieves the initial data from the appropriate
        ///     CSV file.
        /// </summary>
        /// <returns>
        ///     The CSV data reader.
        /// </returns>
        protected override IDataReader CreateDataReader()
        {
            Stream file = this.file.Open();

            TextReader reader = new StreamReader(file);

            return new CsvReader(reader, true);
        }

        /// <summary>
        ///     Converts the string value to the appropriate type.
        /// </summary>
        /// <param name="value">
        ///     The current string value.
        /// </param>
        /// <param name="type"> 
        ///     The expected type. 
        /// </param>
        /// <returns>
        ///     The expected value.
        /// </returns>
        /// <exception cref="System.FormatException">
        ///     The string value is in wrong format.
        /// </exception>
        protected override object ConvertValue(object value, Type type)
        {
            value = this.valueConverter.ConvertValue(value, type);
            
            return base.ConvertValue(value, type);
        }
    }
}
