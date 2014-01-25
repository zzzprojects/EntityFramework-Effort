// --------------------------------------------------------------------------------------------
// <copyright file="ObjectLoader.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
    using System.Linq;
    using System.Collections.Generic;
    using System.Reflection;
    using Effort.Internal.DbManagement.Schema;

    /// <summary>
    ///     Loads data from a table data loader and materializes it.
    /// </summary>
    internal static class ObjectLoader
    {
        /// <summary>
        ///     Loads the table data from the specified table data loader and materializes it
        ///     bases on the specified metadata.
        /// </summary>
        /// <param name="loaderFactory"> The loader factory. </param>
        /// <param name="table"> The table metadata. </param>
        /// <returns> The materialized data. </returns>
        public static IEnumerable<object> Load(
            ITableDataLoaderFactory loaderFactory, 
            DbTableInfo table)
        {
            List<ColumnDescription> columns = new List<ColumnDescription>();
            PropertyInfo[] properties = table.EntityType.GetProperties();
            Func<object, object>[] converters = new Func<object,object>[properties.Length];

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                Type type = property.PropertyType;

                // TODO: external 
                if (type == typeof(NMemory.Data.Timestamp))
                {
                    converters[i] = ConvertTimestamp;
                    type = typeof(byte[]);
                }
                else if (type == typeof(NMemory.Data.Binary))
                {
                    converters[i] = ConvertBinary;
                    type = typeof(byte[]);
                }

                ColumnDescription column = new ColumnDescription(property.Name, type);
                columns.Add(column);
            }

            TableDescription tableDescription = 
                new TableDescription(table.TableName, columns);

            ITableDataLoader loader = loaderFactory.CreateTableDataLoader(tableDescription);

            // Prefetch require info/object to increase performance
            Func<object[], object> initializer = table.EntityInitializer;
            int columnCount = columns.Count;

            // Single array to spare GC
            object[] entityProperties = null;

            foreach (object[] data in loader.GetData())
            {
                if (entityProperties == null)
                {
                    // Initialize at the first element
                    entityProperties = new object[data.Length];
                }

                for (int i = 0; i < columnCount; i++)
                {
                    object propertyValue = data[i];

                    // Use converter if required
                    Func<object, object> converter = converters[i];
                    if (converter != null)
                    {
                        propertyValue = converter.Invoke(propertyValue);
                    }

                    entityProperties[i] = propertyValue;
                }

                yield return initializer.Invoke(entityProperties);
            }
        }

        private static object ConvertTimestamp(object obj)
        {
            return (NMemory.Data.Timestamp)(byte[])obj;
        }

        private static object ConvertBinary(object obj)
        {
            return (NMemory.Data.Binary)(byte[])obj;
        }
    }
}
