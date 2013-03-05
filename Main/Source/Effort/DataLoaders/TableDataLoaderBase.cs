// --------------------------------------------------------------------------------------------
// <copyright file="TableDataLoaderBase.cs" company="Effort Team">
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
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    ///     Provides an abstract base class for <see cref="System.Data.IDataReader" /> based
    ///     table data loaders.
    /// </summary>
    public abstract class TableDataLoaderBase : ITableDataLoader
    {
        private TableDescription table;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TableDataLoaderBase" /> class.
        /// </summary>
        /// <param name="table"> The metadata of the table. </param>
        public TableDataLoaderBase(TableDescription table)
        {
            this.table = table;
        }

        /// <summary>
        ///     Gets the metadata of the table.
        /// </summary>
        /// <value>
        ///     The metadata of the table.
        /// </value>
        protected TableDescription Table
        {
            get { return this.table; }
        }

        /// <summary>
        ///     Creates initial data for the table.
        /// </summary>
        /// <returns>
        ///     The data created for the table.
        /// </returns>
        public virtual IEnumerable<object[]> GetData()
        {
            int columnCount = this.table.Columns.Count;
            int?[] mapper = new int?[columnCount];

            using (IDataReader reader = this.CreateDataReader())
            {
                // Setup field order mapper
                for (int i = 0; i < columnCount; i++)
                {
                    // Find the index of the field in the datareader
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        if (string.Equals(
                                this.Table.Columns[i].Name, 
                                reader.GetName(j), 
                                StringComparison.InvariantCultureIgnoreCase))
                        {
                            mapper[i] = j;
                            break;
                        }
                    }
                }

                while (reader.Read())
                {
                    object[] propertyValues = new object[columnCount];

                    for (int i = 0; i < columnCount; i++)
                    {
                        // Get the index of the field (in the DataReader)
                        int? fieldIndex = mapper[i];

                        if (!fieldIndex.HasValue)
                        {
                            continue;
                        }

                        object fieldValue = reader.GetValue(fieldIndex.Value);
                        Type fieldType = this.Table.Columns[i].Type;

                        propertyValues[i] = this.ConvertValue(fieldValue, fieldType);
                    }

                    yield return propertyValues;
                }
            }
        }

        /// <summary>
        ///     Creates a data reader that retrieves the initial data.
        /// </summary>
        /// <returns> The data reader. </returns>
        protected abstract IDataReader CreateDataReader();

        /// <summary>
        ///     Converts the value to comply with the expected type.
        /// </summary>
        /// <param name="value"> The current value. </param>
        /// <param name="type"> The expected type. </param>
        /// <returns> The expected value. </returns>
        protected virtual object ConvertValue(object value, Type type)
        {
            return value;
        }
    }
}
