namespace Effort.Internal.Common
{
    using System;
    using System.Data;
    using System.Data.Common;

    internal class ProviderHelper
    {
        public static DbProviderManifest GetProviderManifest(string providerInvariantName, string providerManifestToken)
        {
            IServiceProvider serviceProvider = DbProviderFactories.GetFactory(providerInvariantName) as IServiceProvider;

            if (serviceProvider == null)
            {
                throw new ProviderIncompatibleException();
            }

            DbProviderServices providerServices = serviceProvider.GetService(typeof(DbProviderServices)) as DbProviderServices;

            if (providerServices == null)
            {
                throw new ProviderIncompatibleException();
            }
            
            return providerServices.GetProviderManifest(providerManifestToken);
        }

        public static DbConnection CreateConnection(string providerInvariantName)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(providerInvariantName);

            return factory.CreateConnection();
        }
    }
}
