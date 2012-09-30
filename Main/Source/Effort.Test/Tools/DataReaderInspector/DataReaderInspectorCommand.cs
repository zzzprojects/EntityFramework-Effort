namespace Effort.Test.Tools.DataReaderInspector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EFProviderWrapperToolkit;
    using System.Data.Common;
    using System.Data;

    internal class DataReaderInspectorCommand : DbCommandWrapper
    {
        public DataReaderInspectorCommand(
            DbCommand wrappedCommand,
            DbCommandDefinitionWrapper definition)

            : base(wrappedCommand, definition)
        {
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            IResultSetComposer composer = null;
            DataReaderInspectorConnection connection = this.Connection as DataReaderInspectorConnection;

            if (connection != null)
            {
                 composer = connection.Composer;
            }

            if (composer == null)
            {
                throw new InvalidOperationException();
            }

            return new DataReaderInspectorDataReader(base.ExecuteDbDataReader(behavior), composer);
        }
    }
}
