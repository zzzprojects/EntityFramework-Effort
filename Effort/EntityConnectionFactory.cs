using System;
using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using Effort.Caching;
using EFProviderWrapperToolkit;

namespace Effort
{
    public static class EntityConnectionFactory
    {
        static EntityConnectionFactory()
        {
            DatabaseEmulatorProviderConfiguration.RegisterProvider();
            DatabaseAcceleratorProviderConfiguration.RegisterProvider();
        }

        public static EntityConnection CreateEmulator(string entityConnectionString, string source, bool shared)
        {
            entityConnectionString = GetFullEntityConnectionString(entityConnectionString);

            EntityConnectionStringBuilder ecsb = new EntityConnectionStringBuilder(entityConnectionString);

            // Change the provider connection string
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            builder["Data Source"] = source;
            builder["Shared Data"] = shared;

            ecsb.ProviderConnectionString = builder.ConnectionString;

            return Create(DatabaseEmulatorProviderConfiguration.ProviderInvariantName, ecsb.ConnectionString);
        }

        ////public static EntityConnection CreateEmulator(string entityConnectionString)
        ////{
        ////    return Create(DatabaseEmulatorProviderConfiguration.ProviderInvariantName, entityConnectionString);
        ////}

        public static EntityConnection CreateAccelerator(string entityConnectionString)
        {
            return Create(DatabaseAcceleratorProviderConfiguration.ProviderInvariantName, entityConnectionString);
        }

        private static EntityConnection Create(string providerInvariantName, string entityConnectionString)
        {
            entityConnectionString = GetFullEntityConnectionString(entityConnectionString);

            ConnectionStringStore.TryAdd(entityConnectionString);

            EntityConnection connection = EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(entityConnectionString, providerInvariantName);

            return connection;
        }

        private static string GetFullEntityConnectionString(string entityConnectionString)
        {
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(entityConnectionString);

            if (!string.IsNullOrWhiteSpace(builder.Name))
            {
                string connectionStringName = builder.Name;

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connectionStringName];

                if (setting == null)
                {
                    throw new ArgumentException("Connectionstring was not found", "entityConnectionString");
                }

                entityConnectionString = setting.ConnectionString;
            }

            return entityConnectionString;
        }
    }
}
