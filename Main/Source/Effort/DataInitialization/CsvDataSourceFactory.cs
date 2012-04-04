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
using System.Data.Common;
using System.IO;
using Effort.TypeConversion;

namespace Effort.DataInitialization
{
    internal class CsvDataSourceFactory : IDataSourceFactory
    {
        private string connectionString;
        private DirectoryInfo source;
        private ITypeConverter converter;

        public CsvDataSourceFactory(ITypeConverter converter, string connectionString)
        {
            this.connectionString = connectionString;
            this.converter = converter;

            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();
            builder.ConnectionString = connectionString;

            if (!builder.ContainsKey("Data Source"))
            {
                throw new InvalidOperationException("Connectionstring does not contain 'Data Source' attribute");
            }

            this.source = new DirectoryInfo(builder["Data Source"] as string);

            if (!source.Exists)
            {
                throw new InvalidOperationException("The directory specified in 'Data Source' attribute does not exist");
            }
        }

        public IDataSource Create(string tableName, Type entityType)
        {
            string csvPath = Path.Combine(source.FullName, string.Format("{0}.csv", tableName));

            return new CachedCsvDataSource(entityType, this.converter, this.connectionString, tableName, csvPath);
        }

        public void Dispose()
        {
            
        }
    }
}
