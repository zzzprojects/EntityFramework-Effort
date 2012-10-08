#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Data;

#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion



namespace Effort.DataLoaders
{
    public abstract class   TableDataLoaderBase : ITableDataLoader
    {
        private TableDescription table;
       
        public TableDataLoaderBase(TableDescription table)
        {
            this.table = table;
        }

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
                        if (string.Equals(this.Table.Columns[i].Name, reader.GetName(j), StringComparison.InvariantCultureIgnoreCase))
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

                        propertyValues[i] = this.ConvertValue(fieldValue, this.Table.Columns[i].Type);
                    }

                    yield return propertyValues;
                }
            }
        }

        protected TableDescription Table
        {
            get { return this.table; }
        }

        protected abstract IDataReader CreateDataReader();

        protected virtual object ConvertValue(object value, Type type)
        {
            return value;
        }


    }
}
