using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;
using MMDB;
using System.Data.Common;
using Effort.DatabaseManagement;
using System.Data;
using Effort.Components;
using System.Data.Metadata.Edm;
using System.Data.Entity.Infrastructure;

namespace Effort.CodeFirst
{
	public class EffortCodeFirstConnection : EffortWrapperConnection
	{
		public static MetadataWorkspace Workspace { get; set; }
		private Type contextType;

		public EffortCodeFirstConnection()
			: base(ProviderModes.DatabaseEmulator)
		{
		}

		protected override string DefaultWrappedProviderName
		{
			get { throw new NotImplementedException(); }
		}

		internal override Database DatabaseCache
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

		public void processConnectionString()
		{
			var db = new DbConnectionStringBuilder();
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

			if (this.connectionString == null)
			{
				this.connectionString = this.WrappedConnection.ConnectionString;
			}

			this.processConnectionString();

			// Virtualize connection state
			this.connectionState = ConnectionState.Open;
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

		internal override MetadataWorkspace getWorkspace(out string[] metadataFiles)
		{
			metadataFiles = new[] { this.contextType.FullName };
			IObjectContextAdapter context =
				Activator.CreateInstance(this.contextType) as IObjectContextAdapter;
			
			return context.ObjectContext.MetadataWorkspace;
		}

		protected override void Dispose(bool disposing)
		{
		}
	}
}
