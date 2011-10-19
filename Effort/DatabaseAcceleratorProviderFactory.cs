using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using EFProviderWrapperToolkit;
using Effort.Components;

namespace Effort
{
    public class DatabaseAcceleratorProviderFactory : DbProviderFactoryBase
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Factory class is immutable.")]
        public static readonly DatabaseAcceleratorProviderFactory Instance = new DatabaseAcceleratorProviderFactory();

        private DatabaseAcceleratorProviderFactory() 
            : base(EffortWrapperProviderServices.AcceleratorInstance)
        {
        }

        public override DbConnection CreateConnection()
        {
            return new EffortWrapperConnection(ProviderModes.DatabaseAccelerator);
        }
    }
}
