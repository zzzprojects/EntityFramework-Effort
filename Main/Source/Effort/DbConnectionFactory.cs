using System;
using System.Data.Common;
using Effort.DataLoaders;
using Effort.Provider;

namespace Effort
{
//Értelmes komment
    public static class DbConnectionFactory
    {
        static DbConnectionFactory()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        #region Persistent

        public static DbConnection CreatePersistent(string instanceId, IDataLoader dataLoader)
        {
            EffortConnection connection = Create(instanceId, dataLoader);

            return connection;
        }

        public static DbConnection CreatePersistent(string instanceId)
        {
            return CreatePersistent(instanceId, null);
        }

        #endregion

        #region Transient

        public static DbConnection CreateTransient(IDataLoader dataLoader)
        {
            string instanceId = Guid.NewGuid().ToString();

            EffortConnection connection = Create(instanceId, dataLoader);
            connection.MarkAsTransient();

            return connection;
        }

        public static DbConnection CreateTransient()
        {
            return CreateTransient(null);
        }

        #endregion

        private static EffortConnection Create(string instanceId, IDataLoader dataLoader)
        {
            EffortConnectionStringBuilder connectionString = new EffortConnectionStringBuilder();

            connectionString.InstanceId = instanceId;

            if (dataLoader != null)
            {
                connectionString.DataLoaderType = dataLoader.GetType();
                connectionString.DataLoaderArgument = dataLoader.Argument;
            }

            EffortConnection connection = new EffortConnection();
            connection.ConnectionString = connectionString.ConnectionString;

            return connection;
        }
    }
}
