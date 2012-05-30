using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Internal.TypeConversion;
using System.Data.EntityClient;

namespace Effort.DataProviders
{
    public class EntityDataProvider : IDataProvider
    {
        public EntityDataProvider()
        {

        }

        public EntityDataProvider(string entityConnectionString)
        {
            this.Argument = entityConnectionString;
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
            return new EntityDataSourceFactory(this.TypeConverter, () => new EntityConnection(this.Argument));
        }
    }
}
