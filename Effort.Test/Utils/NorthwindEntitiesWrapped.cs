using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MMDB.EntityFrameworkProvider.UnitTests.Data;
using System.Data.EntityClient;
using EFProviderWrapperToolkit;
using MMDB.EntityFrameworkProvider.Components;

namespace MMDB.EntityFrameworkProvider.UnitTests.Utils
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
