using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Internal.TypeConversion;

namespace Effort.DataProviders
{
    public interface IDataProvider
    {
        ITypeConverter TypeConverter { get; set; }

        string Argument { get; set; }

        IDataSourceFactory CreateDataSourceFactory();
    }
}
