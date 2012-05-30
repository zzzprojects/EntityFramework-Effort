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
using System.Linq;
using System.Reflection;
using Effort.Internal.Common;
using Effort.Internal.TypeConversion;

namespace Effort.DataProviders
{
    internal abstract class DataSourceBase : IDataSource
    {
        private ITypeConverter converter;
        
        private string[] propertyNames;
        private Type[] propertyTypes;
        
        private Delegate initializer;


        public DataSourceBase(Type entityType, ITypeConverter converter)
        {
            this.converter = converter;

            PropertyInfo[] properties = entityType.GetProperties();

            this.propertyNames = properties.Select(p => p.Name).ToArray();
            this.propertyTypes = properties.Select(p => p.PropertyType).ToArray();

            this.initializer = LambdaExpressionHelper.CreateInitializerExpression(entityType, properties).Compile();
        }

        public virtual IEnumerable<object> GetInitialRecords()
        {
            int?[] mapper = new int?[propertyNames.Length];
            object[] propertyValues = new object[propertyNames.Length];

            using (IDataReader reader = this.CreateDataReader())
            {
                // Setup field order mapper
                for (int i = 0; i < this.propertyNames.Length; i++)
                {
                    // Find the index of the field in the datareader
                    for (int j = 0; j < reader.FieldCount; j++)
                    {
                        if (string.Equals(this.propertyNames[i], reader.GetName(j), StringComparison.InvariantCultureIgnoreCase))
                        {
                            mapper[i] = j;
                            break;
                        }
                    }
                }
                while (reader.Read())
                {
                    for (int i = 0; i < this.propertyNames.Length; i++)
                    {
                        // Get the index of the field (in the DataReader)
                        int? fieldIndex = mapper[i];

                        if (!fieldIndex.HasValue)
                        {
                            continue;
                        }

                        object fieldValue = reader.GetValue(fieldIndex.Value);

                        propertyValues[i] = this.ConvertValue(fieldValue, this.propertyTypes[i]);
                    }

                    object entity = this.initializer.DynamicInvoke(propertyValues);

                    yield return entity;
                }
            }
        }


        protected abstract IDataReader CreateDataReader();

        protected virtual object ConvertValue(object value, Type type)
        {
            return this.converter.ConvertClrValueToClrValue(value, type);
        }

    }
}
