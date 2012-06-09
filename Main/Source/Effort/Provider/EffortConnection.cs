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
using System.Data;
using System.Data.Common;
using Effort.DataLoaders;
using Effort.Internal.Caching;
using Effort.Internal.DbManagement;

namespace Effort.Provider
{
    public class EffortConnection : DbConnection
    {
        private Guid identifier;
        private ConnectionState state;
        private DbContainer dbContainer;

        public EffortConnection()
        {
            this.identifier = Guid.NewGuid();
            this.state = ConnectionState.Closed;
        }

        public override string ConnectionString
        {
            get;
            set;
        }

        internal DbContainer DbContainer
        {
            get
            {
                return this.dbContainer;
            }
        }


        public override void ChangeDatabase(string databaseName)
        {
            
        }

        protected override DbCommand CreateDbCommand()
        {
            throw new NotImplementedException();
        }

        public override string DataSource
        {
            get { throw new NotImplementedException(); }
        }

        public override string Database
        {
            get { throw new NotImplementedException(); }
        }

        public override void Open()
        {
            EffortConnectionStringBuilder connectionString = new EffortConnectionStringBuilder(this.ConnectionString);

            this.dbContainer = DbContainerStore.GetDbContainer(connectionString.InstanceId, CreateDbContainer);
            
            this.state = ConnectionState.Open;
        }

        private DbContainer CreateDbContainer()
        {
            EffortConnectionStringBuilder connectionString = new EffortConnectionStringBuilder(this.ConnectionString);
            IDataLoader dataLoader = null;

            if (connectionString.DataLoaderType != null)
            {
                // TODO: check parameterless constructor

                dataLoader = Activator.CreateInstance(connectionString.DataLoaderType) as IDataLoader;
                dataLoader.Argument = connectionString.DataLoaderArgument;
                dataLoader.Cached = connectionString.DataLoaderCached;
            }

            return new DbContainer(dataLoader);
        }

        public override void Close()
        {
            this.state = ConnectionState.Closed;
        }

        public override string ServerVersion
        {
            get 
            {
                return typeof(NMemory.Database).Assembly.GetName().Version.ToString(); 
            }
        }

        public override ConnectionState State
        {
            get 
            { 
                return this.state; 
            }
        }

        internal void MarkAsTransient()
        {
            // TODO: implement
        }

        protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel)
        {
            return new EffortTransaction(this, isolationLevel);
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get 
            { 
                return EffortProviderFactory.Instance; 
            }
        }

        public override void EnlistTransaction(System.Transactions.Transaction transaction)
        {
            
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
