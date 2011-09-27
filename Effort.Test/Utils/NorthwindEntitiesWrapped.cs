using System.Data.EntityClient;
using Effort.Test.Data;
using EFProviderWrapperToolkit;

namespace Effort.Test.Utils
{
    public class NorthwindEntitiesWrapped : NorthwindEntities
    {
        public NorthwindEntitiesWrapped(string connectionString)
            : base(CreateEntityConnection(connectionString))
        {

        }

        static NorthwindEntitiesWrapped()
        {
            DatabaseAcceleratorProviderConfiguration.RegisterProvider();
        }

        private static EntityConnection CreateEntityConnection(string connectionString)
        {
            return EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(connectionString, DatabaseAcceleratorProviderConfiguration.ProviderInvariantName);
        }

    }
}
