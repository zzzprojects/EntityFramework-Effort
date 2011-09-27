using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using EFProviderWrapperToolkit;
using Effort.Components;

namespace Effort
{
    public class DatabaseEmulatorProviderFactory : DbProviderFactoryBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Factory class is immutable.")]
        public static readonly DatabaseEmulatorProviderFactory Instance = new DatabaseEmulatorProviderFactory();

        private DatabaseEmulatorProviderFactory() 
            : base(EffortWrapperProviderServices.Instance)
        {
        }

        public override DbConnection CreateConnection()
        {
            return new EffortWrapperConnection(ProviderModes.DatabaseEmulator);
        }
    }
}
