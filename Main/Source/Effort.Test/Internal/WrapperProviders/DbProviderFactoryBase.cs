// --------------------------------------------------------------------------------------------
// <copyright file="DbProviderFactoryBase.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.
//     This source is subject to the Microsoft Public License (Ms-PL).
//     Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//     All other rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.WrapperProviders
{
    using System;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.Common;
#endif
    using System.Diagnostics.Contracts;
    using System.Linq;

    /// <summary>
    /// Common implementation of the <see cref="DbProviderFactory"/> methods.
    /// </summary>
    internal abstract class DbProviderFactoryBase : DbProviderFactory, IServiceProvider
    {
        private DbProviderServices services;

        /// <summary>
        /// Initializes a new instance of the DbProviderFactoryBase class.
        /// </summary>
        /// <param name="providerServices">The provider services.</param>
        protected DbProviderFactoryBase(DbProviderServices providerServices)
        {
            this.services = providerServices;
        }

        /// <summary>
        /// Specifies whether the specific <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
        /// </summary>
        /// <value></value>
        /// <returns>true if the instance of the <see cref="T:System.Data.Common.DbProviderFactory"/> supports the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class; otherwise false.
        /// </returns>
        public override bool CanCreateDataSourceEnumerator
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Registers the specified <see cref="DbProviderFactory"/> type, overwriting any existing registration.
        /// </summary>
        /// <param name="name">The friendly name of the provider factory.</param>
        /// <param name="invariantName">The invariant name of the provider factory. This must be unique.</param>
        /// <param name="description">The description of the provider factory.</param>
        /// <param name="factoryType">The type of the provider factory. This must be derived from <see cref="DbProviderFactory"/>.</param>
        /// <returns><c>true</c> if the registration succeeded; <c>false</c> if the registration failed.</returns>
        public static bool RegisterProvider(string name, string invariantName, string description, Type factoryType)
        {
            Contract.Requires(name != null);
            Contract.Requires(!string.IsNullOrEmpty(invariantName));
            Contract.Requires(description != null);
            Contract.Requires(factoryType != null);

            var systemData = ConfigurationManager.GetSection("system.data") as DataSet;
            if (systemData == null)
                return false;
            if (!systemData.Tables.Contains("DbProviderFactories"))
                return false;
            var dbProviders = systemData.Tables["DbProviderFactories"];
            if (dbProviders == null)
                return false;
            var existing = dbProviders.Rows.Cast<DataRow>().FirstOrDefault(x => Convert.ToString(x["InvariantName"]) == invariantName);
            if (existing != null)
                dbProviders.Rows.Remove(existing);
            var row = dbProviders.NewRow();
            row["Name"] = name;
            row["InvariantName"] = invariantName;
            row["Description"] = description;
            row["AssemblyQualifiedName"] = factoryType.AssemblyQualifiedName;
            dbProviders.Rows.Add(row);
            return true;
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbCommandBuilder"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbCommandBuilder"/>.
        /// </returns>
        public override DbCommandBuilder CreateCommandBuilder()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnectionStringBuilder"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbConnectionStringBuilder"/>.
        /// </returns>
        public override DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataAdapter"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbDataAdapter"/>.
        /// </returns>
        public override DbDataAdapter CreateDataAdapter()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbDataSourceEnumerator"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbDataSourceEnumerator"/>.
        /// </returns>
        public override DbDataSourceEnumerator CreateDataSourceEnumerator()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbParameter"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbParameter"/>.
        /// </returns>
        public override DbParameter CreateParameter()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Returns a new instance of the provider's class that implements the <see cref="T:System.Data.Common.DbConnection"/> class.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="T:System.Data.Common.DbConnection"/>.
        /// </returns>
        public abstract override DbConnection CreateConnection();

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="serviceType">An object that specifies the type of service object to get.</param>
        /// <returns>
        /// A service object of type <paramref name="serviceType"/>.
        /// -or-
        /// null if there is no service object of type <paramref name="serviceType"/>.
        /// </returns>
        public object GetService(Type serviceType)
        {
            if (serviceType == typeof(DbProviderServices))
            {
                return this.services;
            }
            else
            {
                return null;
            }
        }
    }
}
