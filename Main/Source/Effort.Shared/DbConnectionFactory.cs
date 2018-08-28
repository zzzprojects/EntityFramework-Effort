// --------------------------------------------------------------------------------------------
// <copyright file="DbConnectionFactory.cs" company="Effort Team">
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

namespace Effort
{
    using System;
    using System.Data.Common;
    using Effort.DataLoaders;
    using Effort.Provider;

    /// <summary>
    ///     Provides factory methods that are able to create <see cref="T:DbConnection"/> 
    ///     objects that rely on in-process and in-memory databases. All of the data operations
    ///     initiated from these connection objects are executed by the appropriate in-memory 
    ///     database, so using these connection objects does not require any external 
    ///     dependency outside of the scope of the application.
    /// </summary>
    public static class DbConnectionFactory
    {
        /// <summary>
        ///     Initializes static members of the <see cref="DbConnectionFactory" /> class.
        /// </summary>
        static DbConnectionFactory()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        public static int LargeDataRowAttribute
        {
            get { return LargeDataRowAttribute.LargePropertyCount; }
            set { LargeDataRowAttribute.LargePropertyCount = value; }
        }
       
        #region Persistent

        /// <summary>
        ///     Creates a <see cref="T:DbConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the complete application lifecycle. If the
        ///     database is accessed the first time, then its state will be initialized by the
        ///     provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <param name="instanceId">
        ///     The identifier of the in-memory database.
        /// </param>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that might initialize the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:DbConnection"/> object.
        /// </returns>
        public static DbConnection CreatePersistent(string instanceId, IDataLoader dataLoader)
        {
            EffortConnection connection = Create(instanceId, dataLoader);

            return connection;
        }

        /// <summary>
        ///     Creates a <see cref="T:DbConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the complete application lifecycle.
        /// </summary>
        /// <param name="instanceId">
        ///     The identifier of the in-memory database.</param>
        /// <returns>
        ///     The <see cref="T:DbConnection"/> object.
        /// </returns>
        public static DbConnection CreatePersistent(string instanceId)
        {
            return CreatePersistent(instanceId, null);
        }

        #endregion

        #region Transient

        /// <summary>
        ///     Creates a <see cref="T:DbConnection"/> object that rely on an in-memory 
        ///     database instance that lives during the connection object lifecycle. If the 
        ///     connection object is disposed or garbage collected, then underlying database 
        ///     will be garbage collected too. The initial state of the database is initialized
        ///     by the provided <see cref="T:IDataLoader"/> object.
        /// </summary>
        /// <param name="dataLoader">
        ///     The <see cref="T:IDataLoader"/> object that initializes the state of the 
        ///     in-memory database.
        /// </param>
        /// <returns>
        ///     The <see cref="T:DbConnection"/> object.
        /// </returns>
        public static DbConnection CreateTransient(IDataLoader dataLoader)
        {
            string instanceId = Guid.NewGuid().ToString();

            EffortConnection connection = Create(instanceId, dataLoader);
            connection.MarkAsPrimaryTransient();

            return connection;
        }

        /// <summary>
        ///     Creates a <see cref="T:DbConnection"/> object that rely on an in-memory
        ///     database instance that lives during the connection object lifecycle. If the 
        ///     connection object is disposed or garbage collected, then underlying database 
        ///     will be garbage collected too.
        /// </summary>
        /// <returns>
        ///     The <see cref="T:DbConnection"/> object.
        /// </returns>
        public static DbConnection CreateTransient()
        {
            return CreateTransient(null);
        }

        #endregion

        /// <summary>
        ///     Creates an EffortConnection object with a connection string that represents the 
        ///     specified parameter values.
        /// </summary>
        /// <param name="instanceId"> The instance id. </param>
        /// <param name="dataLoader"> The data loader. </param>
        /// <returns> The EffortConnection object. </returns>
        private static EffortConnection Create(string instanceId, IDataLoader dataLoader)
        {
            EffortProviderConfiguration.VerifyProvider();

            EffortConnectionStringBuilder connectionString = 
                new EffortConnectionStringBuilder();

            connectionString.InstanceId = instanceId;

            if (dataLoader != null)
            {
                connectionString.DataLoaderType = dataLoader.GetType();
                connectionString.DataLoaderArgument = dataLoader.Argument;
            }

            EffortConnection connection = new EffortConnection();
            connection.ConnectionString = connectionString.ConnectionString;

            return connection;
        }
    }
}
