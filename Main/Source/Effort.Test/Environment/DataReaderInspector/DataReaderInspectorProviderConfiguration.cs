
using System;
using System.Configuration;
using System.Data;
using System.Threading;
using EFProviderWrapperToolkit;

namespace Effort.Test.Environment.DataReaderInspector
{
    internal static class DataReaderInspectorProviderConfiguration
    {
        public static readonly string ProviderInvariantName = "DataReaderInspectorProvider";

        private static bool registered = false;
        private static object sync = new object();

        public static void RegisterProvider()
        {
            if (!registered)
            {
                lock (sync)
                {
                    if (!registered)
                    {
                        DbProviderFactoryBase.RegisterProvider(
                            "Data Reader Inspector Provider", 
                            ProviderInvariantName, 
                            "Inspect DbDataReader result", 
                            typeof(DataReaderInspectorProviderFactory));

                        Thread.MemoryBarrier();
                        registered = true;
                    }
                }
            }
        }

    }
}
