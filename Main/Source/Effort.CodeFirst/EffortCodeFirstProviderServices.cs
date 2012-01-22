using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Effort.Components;
using System.Data.Common;

namespace Effort.CodeFirst
{
	internal class EffortCodeFirstProviderServices : EffortWrapperProviderServices
	{
		public static readonly EffortCodeFirstProviderServices CodeFirstInstance;

		static EffortCodeFirstProviderServices()
		{
			var invariantName = CodeFirstEmulatorProviderConfiguration.ProviderInvariantName;
			EffortCodeFirstProviderServices.CodeFirstInstance = new EffortCodeFirstProviderServices(invariantName);
		}

		internal EffortCodeFirstProviderServices(string invariantName)
			: base(invariantName)
		{

		}

		protected override string GetDbProviderManifestToken(System.Data.Common.DbConnection connection)
		{
			return this.ProviderInvariantName;
		}
		protected override System.Data.Common.DbProviderManifest GetDbProviderManifest(string manifestToken)
		{
			var providerInvariantName = "System.Data.SqlClient";
			var token = "2008";

			DbProviderServices services = GetProviderServicesByName(providerInvariantName);
			DbProviderManifest wrappedProviderManifest = services.GetProviderManifest(token);
			DbProviderManifest wrapperManifest = this.CreateProviderManifest(providerInvariantName, wrappedProviderManifest);

			return wrapperManifest;
		}

		protected override bool DbDatabaseExists(DbConnection connection, int? commandTimeout, System.Data.Metadata.Edm.StoreItemCollection storeItemCollection)
		{
			return true;
		}
	}
}
