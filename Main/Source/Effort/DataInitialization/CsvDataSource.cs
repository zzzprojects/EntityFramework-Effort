using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;
using LumenWorks.Framework.IO.Csv;
using Effort.Helpers;
using System.Globalization;

namespace Effort.DataInitialization
{
    internal class CsvDataSource : DataSourceBase
    {
        private FileInfo file;

        public CsvDataSource(string path, Type entityType) : base(entityType)
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
                    return null;
                }
                else
                {
                    type = type.GetGenericArguments()[0];
                }
            }

            if (type == typeof(byte[]))
            {
                return Convert.FromBase64String(val);
            }

            return Convert.ChangeType(value, type);
        }
    }
}
