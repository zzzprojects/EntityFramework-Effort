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
using System.Data.Common;
using System.Data.Entity.Infrastructure;
using System.Data.Metadata.Edm;
using Effort.Components;
using Effort.DatabaseManagement;

namespace Effort.CodeFirst
{
	public class EffortCodeFirstConnection : EffortWrapperConnection
	{
		private Type contextType;
        private DatabaseCache databaseCache;

		public EffortCodeFirstConnection()
			: base(ProviderModes.DatabaseEmulator)
		{
		}

		protected override string DefaultWrappedProviderName
		{
			get { throw new NotImplementedException(); }
		}

		internal override DatabaseCache DatabaseCache
		{
			get
			{
				if (this.databaseCache == null)
				{
					this.databaseCache = this.CreateDatabaseSandboxed();
				}

				return this.databaseCache;
			}
		}

		private void ProcessConnectionString()
		{
            DbConnectionStringBuilder db = new DbConnectionStringBuilder();
			db.ConnectionString = this.ConnectionString;

			this.contextType = Type.GetType(db["context"].ToString());
		}

		public override void Open()
		{
			if (this.DesignMode)
			{
				base.Open();
				return;
			}

            this.ProcessConnectionString();

			// Virtualize connection state
            this.MarkAsOpen();
		}


        public override string ConnectionString
        {
            get
            {
                if (base.ConnectionString == null)
                {
                    return this.WrappedConnection.ConnectionString;
                }

                return base.ConnectionString;
            }
            set
            {
                base.ConnectionString = value;
            }
        }

		protected override DbProviderFactory DbProviderFactory
		{
			get
			{
				if (this.DesignMode)
				{
					return DbProviderFactories.GetFactory(this.WrappedProviderInvariantName);
				}

				return CodeFirstEmulatorProviderFactory.Instance;
			}
		}

		protected internal override MetadataWorkspace GetWorkspace(out string[] metadataFiles)
		{
			metadataFiles = new string[] { this.contextType.FullName };

			IObjectContextAdapter context =
				Activator.CreateInstance(this.contextType) as IObjectContextAdapter;
			
			return context.ObjectContext.MetadataWorkspace;
		}

		protected override void Dispose(bool disposing)
		{
            
		}
	}
}
