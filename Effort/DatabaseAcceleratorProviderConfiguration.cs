using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;

namespace Effort
{
    public static class DatabaseAcceleratorProviderConfiguration
    {
        public static readonly string ProviderInvariantName = "EffortDatabaseAcceleratorProvider";

        private static volatile bool registered = false;
        private static object synch = new object();

        public static void RegisterProvider()
        {
            if (!registered)
            {
                lock (synch)
                {
                    if (!registered)
                    {
                        DbProviderFactoryBase.RegisterProvider("Effort Database Accelerator Provider", ProviderInvariantName, typeof(DatabaseAcceleratorProviderFactory));
                        registered = true;
                    }
                }
            }
        }
    }
}
