// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaKey.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.Caching
{
    using System;
    using System.Collections.Generic;
    using System.Data.Metadata.Edm;
    using System.Linq;
    using System.Text;

    internal class DbSchemaKey : IEquatable<DbSchemaKey>
    {
        private string description;

        public DbSchemaKey(StoreItemCollection storeItemCollection)
        {
            // Find container
            EntityContainer entityContainer = 
                storeItemCollection.GetItems<EntityContainer>().FirstOrDefault();

            StringBuilder builder = new StringBuilder();

            builder.Append(entityContainer.Name);
            builder.Append("(");

            // Find entity sets
            IEnumerable<EntitySet> sets = 
                entityContainer.BaseEntitySets.OfType<EntitySet>().OrderBy(s => s.Name);

            foreach (EntitySet set in sets)
            {
                builder.Append(set.Name);
                builder.Append("(");

                IEnumerable<EdmProperty> properties = 
                    set.ElementType.Properties.OrderBy(p => p.Name);

                foreach (EdmProperty property in properties)
                {
                    builder.Append(property.Name);
                    builder.Append("(");

                    builder.Append(property.TypeUsage.EdmType.FullName);

                    builder.Append(")");
                }

                builder.Append(")");
            }

            builder.Append(")");

            this.description = builder.ToString();
        }

        private DbSchemaKey()
        {
        }

        public static DbSchemaKey FromString(string key)
        {
            DbSchemaKey result = new DbSchemaKey();
            result.description = key;

            return result;
        }

        public bool Equals(DbSchemaKey other)
        {
            if (other == null)
            {
                return false;
            }

            return other.description == this.description;
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as DbSchemaKey);
        }

        public override int GetHashCode()
        {
            return this.description.GetHashCode();
        }

        public override string ToString()
        {
            return this.description;
        }
    }
}
