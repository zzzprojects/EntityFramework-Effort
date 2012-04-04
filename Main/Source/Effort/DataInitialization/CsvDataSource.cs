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
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using LumenWorks.Framework.IO.Csv;
using Effort.Helpers;
using System.Globalization;
using Effort.TypeConversion;

namespace Effort.DataInitialization
{
    internal class CsvDataSource : DataSourceBase
    {
        private FileInfo file;

        public CsvDataSource(Type entityType, ITypeConverter typeConverter, string path) : base(entityType, typeConverter)
        {
            this.file = new FileInfo(path);
        }

        public override IEnumerable<object> GetInitialRecords()
        {
            if (!file.Exists)
            {
                yield break;
            }

            using(new CultureScope(CultureInfo.InvariantCulture))
            foreach (var item in base.GetInitialRecords())
            {
                yield return item;
            }

        }

        protected override IDataReader CreateDataReader()
        {
            return new CsvReader(new StreamReader(file.OpenRead()), true);
        }

        protected override object ConvertValue(object value, Type type)
        {
            string val = value as string;

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                if (string.IsNullOrWhiteSpace(val))
                {
                    return base.ConvertValue(null, type);
                }
                else
                {
                    type = type.GetGenericArguments()[0];
                }
            }

            if (type == typeof(byte[]) || type == typeof(NMemory.Data.Binary))
            {
                value = Convert.FromBase64String(val);
            }
            else
            {
                value = Convert.ChangeType(value, type);
            }

            return base.ConvertValue(value, type);
        }
    }
}
