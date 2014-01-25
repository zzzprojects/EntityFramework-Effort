// --------------------------------------------------------------------------------------------
// <copyright file="DataReaderInspectorProviderConfiguration.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
//
//     Permission is hereby granted, free of charge, to any person obtaining a copy
//     of this software and associated documentation files (the "Software"), to deal
//     in the Software without restriction, including without limitation the rights
//     to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//     copies of the Software, and to permit persons to whom the Software is
//     furnished to do so, subject to the following conditions:
//
//     The above copyright notice and this permission notice shall be included in
//     all copies or substantial portions of the Software.
//
//     THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//     IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//     FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//     AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//     LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//     OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//     THE SOFTWARE.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.DataReaderInspector
{
#if !EFOLD
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Infrastructure.DependencyResolution;
#endif
    using System.Threading;
    using Effort.Test.Internal.WrapperProviders;

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

                        RegisterEFServices();

                        Thread.MemoryBarrier();
                        registered = true;
                    }
                }
            }
        }

        private static void RegisterEFServices()
        {
#if !EFOLD
            DbConfiguration.Loaded += OnDbConfigurationLoaded;
#endif
        }

        
#if !EFOLD
        private static void OnDbConfigurationLoaded(
            object sender, 
            DbConfigurationLoadedEventArgs e)
        {
            e.AddDependencyResolver(
                new SingletonDependencyResolver<DbProviderServices>(
                    DataReaderInspectorProviderServices.Instance,
                    ProviderInvariantName),
                false);

            e.AddDependencyResolver(
                new SingletonDependencyResolver<IProviderInvariantName>(
                        DataReaderInspectorInvariantName.Instance,
                        DataReaderInspectorProviderFactory.Instance),
                false);
        }
#endif
    }
}
