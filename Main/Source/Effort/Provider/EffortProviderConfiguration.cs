// --------------------------------------------------------------------------------------------
// <copyright file="EffortProviderConfiguration.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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

namespace Effort.Provider
{
    using System;
    using System.Configuration;
    using System.Data;
#if !EFOLD
    using System.Data.Entity.Config;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure;
#endif
    using System.Threading;

    /// <summary>
    ///     Configuration module for the Effort provider.
    /// </summary>
    public static class EffortProviderConfiguration
    {
        /// <summary>
        ///     The provider invariant name of the Effort provider.
        /// </summary>
        public static readonly string ProviderInvariantName = "Effort.Provider";

        /// <summary>
        ///     Indicates if the Effort provider is registered.
        /// </summary>
        private static bool isRegistered = false;

        /// <summary>
        ///     Latch object that is used to avoid double registration.
        /// </summary>
        private static object latch = new object();

        /// <summary>
        ///     Registers the provider factory.
        /// </summary>
        public static void RegisterProvider()
        {
            if (!isRegistered)
            {
                lock (latch)
                {
                    if (!isRegistered)
                    {
                        RegisterProvider(
                            "Effort Provider", 
                            ProviderInvariantName, 
                            typeof(EffortProviderFactory));

#if !EFOLD
                        RegisterDbConfigurationEventHandler();
#endif

                        Thread.MemoryBarrier();
                        isRegistered = true;
                    }
                }
            }
        }

        private static void RegisterProvider(
            string name, 
            string invariantName, 
            Type factoryType)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException("name");
            }

            if (string.IsNullOrEmpty(invariantName))
            {
                throw new ArgumentNullException("invariantName");
            }

            if (factoryType == null)
            {
                throw new ArgumentNullException("factoryType");
            }

            string assemblyName = factoryType.AssemblyQualifiedName;

            DataSet data = (DataSet)ConfigurationManager.GetSection("system.data");
            DataTable providerFactories = data.Tables["DbProviderFactories"];

            providerFactories.Rows.Add(name, name, invariantName, assemblyName);
        }

#if !EFOLD
        private static void RegisterDbConfigurationEventHandler()
        {
            DbConfiguration.OnLockingConfiguration += OnLockingConfiguration;
        }

        private static void OnLockingConfiguration(object sender, DbConfigurationEventArgs e)
        {
            e.AddDependencyResolver(
                new SingletonDependencyResolver<DbProviderServices>(
                        EffortProviderServices.Instance,
                        ProviderInvariantName),
                false);

            e.AddDependencyResolver(
                new SingletonDependencyResolver<IProviderInvariantName>(
                        EffortProviderInvariantName.Instance,
                        EffortProviderFactory.Instance),
                false);
        }
#endif
    }
}
