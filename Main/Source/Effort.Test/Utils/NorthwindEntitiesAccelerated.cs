using System.Data.EntityClient;
using Effort.Test.Data;
using EFProviderWrapperToolkit;

namespace Effort.Test.Utils
{
    public class NorthwindEntitiesAccelerated : NorthwindEntities
    {
        public NorthwindEntitiesAccelerated()
            : this("name=NorthwindEntities")
        {
 
        }

        public NorthwindEntitiesAccelerated(string connectionString)
            : base(CreateEntityConnection(connectionString))
        {

        }

        private static EntityConnection CreateEntityConnection(string connectionString)
        {
            return EntityConnectionFactory.CreateAccelerator(connectionString); 
        }

    }
}
