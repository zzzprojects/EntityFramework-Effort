// --------------------------------------------------------------------------------------------
// <copyright file="ObjectLoader.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.DataLoaders
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Reflection;
    using Effort.Internal.Common;

    internal static class ObjectLoader
    {
        public static IEnumerable<object> Load(
            ITableDataLoaderFactory loaderFactory, 
            string tableName, 
            Type entityType)
        {
            List<ColumnDescription> columns = new List<ColumnDescription>();
            PropertyInfo[] properties = entityType.GetProperties();

            foreach (PropertyInfo property in properties)
            {
                Type type = property.PropertyType;

                // TODO: external 
                if (type == typeof(NMemory.Data.Timestamp) || 
                    type == typeof(NMemory.Data.Binary))
                {
                    type = typeof(byte[]);
                }

                columns.Add(new ColumnDescription(property.Name, property.PropertyType));
            }

            TableDescription tableDescription = new TableDescription(tableName, columns);

            ITableDataLoader loader = loaderFactory.CreateTableDataLoader(tableDescription);

            LambdaExpression initializerExpression =
                LambdaExpressionHelper.CreateInitializerExpression(entityType, properties);
           
            Delegate initializer = initializerExpression.Compile();
          

            foreach (object[] data in loader.GetData())
            {
                for (int i = 0; i < columns.Count; i++)
                {
                    // TODO: external
                    if (properties[i].PropertyType == typeof(NMemory.Data.Timestamp))
                    {
                        data[i] = (NMemory.Data.Timestamp)(byte[])data[i];
                    }
                    else if (properties[i].PropertyType == typeof(NMemory.Data.Binary))
                    {
                        data[i] = (NMemory.Data.Binary)(byte[])data[i];
                    }
                }

                object entity = initializer.DynamicInvoke(data);

                yield return entity;
            }
        }
    }
}
