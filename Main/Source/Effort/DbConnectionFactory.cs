using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using Effort.Provider;
using Effort.DataProviders;

namespace Effort
{
    public static class DbConnectionFactory
    {
        static DbConnectionFactory()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        #region Persistent

        public static DbConnection CreatePersistent(string instanceId, IDataProvider dataProvider)
        {
            EffortConnection connection = Create(instanceId, dataProvider);

            return connection;
        }

        public static DbConnection CreatePersistent(string instanceId)
        {
            return CreatePersistent(instanceId, null);
        }

        #endregion

        #region Transient

        public static DbConnection CreateTransient(IDataProvider dataProvider)
        {
            string instanceId = Guid.NewGuid().ToString();

            EffortConnection connection = Create(instanceId, dataProvider);
            connection.MarkAsTransient();

            return connection;
        }

        public static DbConnection CreateTransient()
        {
            return CreateTransient(null);
        }

        #endregion

        private static EffortConnection Create(string instanceId, IDataProvider dataProvider)
        {
            EffortConnectionStringBuilder connectionString = new EffortConnectionStringBuilder();

            connectionString.InstanceId = instanceId;

            if (dataProvider != null)
            {
                connectionString.DataProviderType = dataProvider.GetType();
                connectionString.DataProviderArg = dataProvider.Argument;
            }

            EffortConnection connection = new EffortConnection();
            connection.ConnectionString = connectionString.ConnectionString;

            return connection;
        }
    }
}
