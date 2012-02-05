#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

using System;
using System.Configuration;
using System.Data.Common;
using System.Data.EntityClient;
using Effort.Caching;
using EFProviderWrapperToolkit;

namespace Effort
{
    public static class EntityConnectionFactory
    {
        static EntityConnectionFactory()
        {
            DatabaseEmulatorProviderConfiguration.RegisterProvider();
            DatabaseAcceleratorProviderConfiguration.RegisterProvider();
        }

        public static EntityConnection CreateEmulator(string entityConnectionString, string source, bool shared)
        {
            entityConnectionString = GetFullEntityConnectionString(entityConnectionString);

            EntityConnectionStringBuilder ecsb = new EntityConnectionStringBuilder(entityConnectionString);

            // Change the provider connection string
            DbConnectionStringBuilder builder = new DbConnectionStringBuilder();

            builder["Data Source"] = source;
            builder["Shared Data"] = shared;

            ecsb.ProviderConnectionString = builder.ConnectionString;

            return Create(DatabaseEmulatorProviderConfiguration.ProviderInvariantName, ecsb.ConnectionString);
        }

        ////public static EntityConnection CreateEmulator(string entityConnectionString)
        ////{
        ////    return Create(DatabaseEmulatorProviderConfiguration.ProviderInvariantName, entityConnectionString);
        ////}

        public static EntityConnection CreateAccelerator(string entityConnectionString)
        {
            return Create(DatabaseAcceleratorProviderConfiguration.ProviderInvariantName, entityConnectionString);
        }

        private static EntityConnection Create(string providerInvariantName, string entityConnectionString)
        {
            entityConnectionString = GetFullEntityConnectionString(entityConnectionString);

            ConnectionStringStore.TryAdd(entityConnectionString);

            EntityConnection connection = EntityConnectionWrapperUtils.CreateEntityConnectionWithWrappers(entityConnectionString, providerInvariantName);

            return connection;
        }

        private static string GetFullEntityConnectionString(string entityConnectionString)
        {
            EntityConnectionStringBuilder builder = new EntityConnectionStringBuilder(entityConnectionString);

            if (!string.IsNullOrWhiteSpace(builder.Name))
            {
                string connectionStringName = builder.Name;

                ConnectionStringSettings setting = ConfigurationManager.ConnectionStrings[connectionStringName];

                if (setting == null)
                {
                    throw new ArgumentException("Connectionstring was not found", "entityConnectionString");
                }

                entityConnectionString = setting.ConnectionString;
            }

            return entityConnectionString;
        }
    }
}
