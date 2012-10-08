namespace Effort.Test.Environment.DataReaderInspector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EFProviderWrapperToolkit;
    using System.Data.Common;

    public class DataReaderInspectorProviderFactory : DbProviderFactoryBase
    {
        public static readonly DataReaderInspectorProviderFactory Instance = new DataReaderInspectorProviderFactory();

        public DataReaderInspectorProviderFactory() : base(new DataReaderInspectorProviderServices())
        {

        }

        public override DbConnection CreateConnection()
        {
            return new DataReaderInspectorConnection(null);
        }
    }
}
