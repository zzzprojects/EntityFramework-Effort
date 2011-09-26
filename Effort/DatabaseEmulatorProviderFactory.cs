using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using EFProviderWrapperToolkit;
using MMDB.EntityFrameworkProvider.Components;

namespace MMDB.EntityFrameworkProvider
{
    public class DatabaseEmulatorProviderFactory : DbProviderFactoryBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Factory class is immutable.")]
        public static readonly DatabaseEmulatorProviderFactory Instance = new DatabaseEmulatorProviderFactory();

        private DatabaseEmulatorProviderFactory() 
            : base(MMDBWrapperProviderServices.Instance)
        {
        }

        public override DbConnection CreateConnection()
        {
            return new MMDBWrapperConnection(ProviderModes.DatabaseEmulator);
        }
    }
}
