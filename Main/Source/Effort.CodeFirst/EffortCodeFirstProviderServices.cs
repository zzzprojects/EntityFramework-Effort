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
