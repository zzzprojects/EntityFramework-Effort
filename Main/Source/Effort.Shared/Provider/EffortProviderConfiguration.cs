// --------------------------------------------------------------------------------------------
// <copyright file="EffortProviderConfiguration.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Infrastructure;
    using System.Data.Entity.Infrastructure.DependencyResolution;
#endif
    using System.Threading;
    using Effort.Exceptions;

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

        internal static void VerifyProvider()
        {
            try
            {
#if NETSTANDARD && !EF6
                DbProviderFactoriesCore.GetFactory(ProviderInvariantName);
#else
                DbProviderFactories.GetFactory(ProviderInvariantName);
#endif
            }
            catch (Exception ex)
            {
                throw new EffortException(
                    ExceptionMessages.AutomaticRegistrationFailed,
                    ex);
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

#if NETSTANDARD && !EF6
            System.Data.Common.DbProviderFactoriesCore.RegisterFactory(invariantName, factoryType);
#else
            string assemblyName = factoryType.AssemblyQualifiedName;

            DataSet data = (DataSet)ConfigurationManager.GetSection("system.data");

            if (data != null)
            {
                DataTable providerFactories = data.Tables["DbProviderFactories"];

                foreach (DataRow providerFactory in providerFactories.Rows)
                {
                    string providerFactoryInvariantName =
                        providerFactory["InvariantName"] as string;

                    if (invariantName.Equals(
                        providerFactoryInvariantName,
                        StringComparison.InvariantCulture))
                    {
                        // Provider is already registered
                        return;
                    }
                }

                providerFactories.Rows.Add(name, name, invariantName, assemblyName);
            }
#endif
        }

#if !EFOLD
        internal static void RegisterDbConfigurationEventHandler()
        {
            try
            {
                DbConfiguration.Loaded += OnDbConfigurationLoaded;
            }
            catch (Exception ex)
            {
                throw new EffortException(ExceptionMessages.AutomaticRegistrationFailed, ex);
            }
        }

        private static void OnDbConfigurationLoaded(
            object sender,
            DbConfigurationLoadedEventArgs e)
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

        internal static void RegisterDbConfiguration()
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


                        try
                        {
                            new SingletonDependencyResolver<DbProviderServices>(
                                EffortProviderServices.Instance,
                                ProviderInvariantName);

                            new SingletonDependencyResolver<IProviderInvariantName>(
                                EffortProviderInvariantName.Instance,
                                EffortProviderFactory.Instance);

                        }
                        catch (Exception ex)
                        {
                            throw new EffortException(ExceptionMessages.AutomaticRegistrationFailed, ex);
                        }

                        Thread.MemoryBarrier();
                        isRegistered = true; 
                    }
                }
            }
        } 
#endif
    }
}
