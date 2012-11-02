// ----------------------------------------------------------------------------------
// <copyright file="CsvTableDataLoader.cs" company="Effort Team">
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
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.IO;
    using Effort.Internal.Common;
    using LumenWorks.Framework.IO.Csv;

    public class CsvTableDataLoader : TableDataLoaderBase
    {
        private FileInfo file;

        public CsvTableDataLoader(string path, TableDescription table) : base(table)
        {
            this.file = new FileInfo(path);
        }

        public override IEnumerable<object[]> GetData()
        {
            if (!file.Exists)
            {
                yield break;
            }

            using (new CultureScope(CultureInfo.InvariantCulture))
            {
                foreach (var item in base.GetData())
                {
                    yield return item;
                }
            }
        }

        protected override IDataReader CreateDataReader()
        {
            return new CsvReader(new StreamReader(file.OpenRead()), true);
        }

        protected override object ConvertValue(object value, Type type)
        {
            string val = value as string;

            if (string.IsNullOrEmpty(val))
            {
                return null;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                type = type.GetGenericArguments()[0];
            }

            if (type == typeof(string))
            {
                if (!val.StartsWith("'", StringComparison.InvariantCulture))
                {
                    throw new FormatException(string.Format("\"{0}\" is an invalid string, strings fields must start with \"'\"", val));
                }

                value = ResolveEscapeCharacters(val.Substring(1));

            }
            if (type == typeof(byte[]) || type == typeof(NMemory.Data.Binary) || type == typeof(NMemory.Data.Timestamp))
            {
                value = Convert.FromBase64String(val);
            }
            else
            {
                value = Convert.ChangeType(value, type);
            }

            return base.ConvertValue(value, type);
        }

        private static string ResolveEscapeCharacters(string value)
        {
            char[] chars = value.ToCharArray();

            StringWriter writer = new StringWriter();

            bool escaped = false;

            for (int i = 0; i < chars.Length; i++)
            {
                char c = chars[i];

                if (escaped)
                {
                    escaped = false;
                    switch (c)
                    {
                        case '\\':
                            writer.Write('\\');
                            break;
                        case 'n':
                            writer.Write('\n');
                            break;
                        case 'r':
                            writer.Write('\r');
                            break;
                        default:
                            throw new FormatException(string.Format("\"{0}\" is an invalid string, it contains an invalid escaped character", value));
                    }
                }
                else if (c == '\\')
                {
                    escaped = true;
                }
                else
                {
                    writer.Write(c);
                }
            }

            if (escaped)
            {
                throw new FormatException(string.Format("\"{0}\" is an invalid string, it contains an invalid escaped character", value));
            }

            return writer.ToString();
        }
    }
}
