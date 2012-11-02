// ----------------------------------------------------------------------------------
// <copyright file="DbConnectionFactory.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort
{
    using System;
    using System.Data.Common;
    using Effort.DataLoaders;
    using Effort.Provider;

    public static class DbConnectionFactory
    {
        static DbConnectionFactory()
        {
            EffortProviderConfiguration.RegisterProvider();
        }

        #region Persistent

        public static DbConnection CreatePersistent(string instanceId, IDataLoader dataLoader)
        {
            EffortConnection connection = Create(instanceId, dataLoader);

            return connection;
        }

        public static DbConnection CreatePersistent(string instanceId)
        {
            return CreatePersistent(instanceId, null);
        }

        #endregion

        #region Transient

        public static DbConnection CreateTransient(IDataLoader dataLoader)
        {
            string instanceId = Guid.NewGuid().ToString();

            EffortConnection connection = Create(instanceId, dataLoader);
            connection.MarkAsTransient();

            return connection;
        }

        public static DbConnection CreateTransient()
        {
            return CreateTransient(null);
        }

        #endregion

        private static EffortConnection Create(string instanceId, IDataLoader dataLoader)
        {
            EffortConnectionStringBuilder connectionString = new EffortConnectionStringBuilder();

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
