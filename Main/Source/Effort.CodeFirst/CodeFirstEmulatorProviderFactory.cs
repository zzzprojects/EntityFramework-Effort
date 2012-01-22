using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using EFProviderWrapperToolkit;
using Effort.Components;
namespace Effort.CodeFirst
{
	public class CodeFirstEmulatorProviderFactory : DbProviderFactoryBase
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Factory class is immutable.")]
		public static readonly CodeFirstEmulatorProviderFactory Instance = new CodeFirstEmulatorProviderFactory();

		private CodeFirstEmulatorProviderFactory()
			: base(EffortCodeFirstProviderServices.CodeFirstInstance)
		{
		}

		public override DbConnection CreateConnection()
		{
			return new EffortCodeFirstConnection();
		}
	}
}
