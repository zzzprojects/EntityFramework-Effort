using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Internal.TypeConversion;
using System.IO;

namespace Effort.DataProviders
{
    public class CsvDataProvider : IDataProvider
    {
        public CsvDataProvider()
        {

        }

        public CsvDataProvider(string path)
        {
            this.Argument = path;
        }

        public ITypeConverter TypeConverter
        {
            get;
            set;
        }

        public string Argument
        {
            get;
            set;
        }

        public IDataSourceFactory CreateDataSourceFactory()
        {
            if (string.IsNullOrEmpty(this.Argument))
            {
                return new EmptyDataSourceFactory();
            }

            return new CsvDataSourceFactory(this.TypeConverter, this.Argument);
        }
    }
}
