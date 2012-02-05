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
using System.Reflection;

namespace Effort.DatabaseManagement
{
	internal class DatabaseSchema
	{
		private Dictionary<string, TableInformation> schemaDetails;
		private Dictionary<Type, TableInformation> schemaDetailsByTypeName;

		public DatabaseSchema()
		{
			this.schemaDetails = new Dictionary<string, TableInformation>();
			this.schemaDetailsByTypeName = new Dictionary<Type, TableInformation>();
		}

		public void Register(string tableName, Type entityType, PropertyInfo[] primaryKeyFields, PropertyInfo identityField,
			PropertyInfo[] properties)
		{
			var info = new TableInformation(entityType, primaryKeyFields, identityField, properties);
			this.schemaDetails.Add(tableName, info);
			this.schemaDetailsByTypeName.Add(entityType, info);
		}

		public TableInformation GetInformation(string tableName)
		{
			return this.schemaDetails[tableName];
		}

		public TableInformation GetInformationByTypeName(Type entityType )
		{
			TableInformation info;

			if (this.schemaDetailsByTypeName.TryGetValue(entityType, out info))
				return info;
			else
				return null;
		}

		public string[] GetTableNames()
		{
			return this.schemaDetails.Keys.ToArray();
		}

	}
}
