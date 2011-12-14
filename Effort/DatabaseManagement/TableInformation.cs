using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace Effort.DatabaseManagement
{
	internal class TableInformation
	{
		public TableInformation(Type entityType, PropertyInfo[] primaryKeys, PropertyInfo identityField, PropertyInfo[] properties)
		{
			this.EntityType = entityType;
			this.PrimaryKeyFields = primaryKeys;
			this.IdentityField = identityField;
			this.Properties = properties;
		}

		public Type EntityType { get; private set; }

		public PropertyInfo[] PrimaryKeyFields { get; private set; }

		public PropertyInfo IdentityField { get; private set; }

		// szukseges a DataReader-ben a sorrendhelyes visszaadashoz
		public PropertyInfo[] Properties { get; private set; }
	}
}
