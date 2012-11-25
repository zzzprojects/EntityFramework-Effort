// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaKey.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
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
    using Effort.Internal.Common;

    /// <summary>
    ///     Represents a key that identifies <see cref="T:DbSchema"/> objects.
    /// </summary>
    internal class DbSchemaKey : IEquatable<DbSchemaKey>
    {
        /// <summary>
        ///     Serialized form the StoreItemCollection, used as the key.
        /// </summary>
        private string innerKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DbSchemaKey" /> class.
        /// </summary>
        /// <param name="storeItemCollection">
        ///     The store item collection that the corresponding <see cref="T:DbSchema"/> is 
        ///     based on.
        /// </param>
        public DbSchemaKey(StoreItemCollection storeItemCollection)
        {
            // Find container
            EntityContainer entityContainer = 
                storeItemCollection.GetItems<EntityContainer>().FirstOrDefault();

            StringBuilder builder = new StringBuilder();

            builder.Append(entityContainer.Name);
            builder.Append("(");

            // Find entity sets
            IEnumerable<EntitySet> sets = entityContainer
                .BaseEntitySets
                .OfType<EntitySet>()
                .OrderBy(s => s.GetTableName());

            foreach (EntitySet set in sets)
            {
                builder.Append(set.GetTableName());
                builder.Append("(");

                IEnumerable<EdmProperty> properties = 
                    set.ElementType.Properties.OrderBy(p => p.GetColumnName());

                foreach (EdmProperty property in properties)
                {
                    builder.Append(property.GetColumnName());
                    builder.Append("(");

                    builder.Append(property.TypeUsage.EdmType.FullName);

                    builder.Append(")");
                }

                builder.Append(")");
            }

            builder.Append(")");

            this.innerKey = builder.ToString();
        }

        /// <summary>
        ///     Prevents a default instance of the <see cref="DbSchemaKey" /> class from being
        ///     created.
        /// </summary>
        private DbSchemaKey()
        {
        }

        /// <summary>
        ///     Creates a <see cref="DbSchemaKey"/> object based on the specified string.
        /// </summary>
        /// <param name="value"> The string. </param>
        /// <returns> The <see cref="DbSchemaKey"/> object. </returns>
        public static DbSchemaKey FromString(string value)
        {
            DbSchemaKey result = new DbSchemaKey();
            result.innerKey = value;

            return result;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="DbSchemaKey" /> is equal to this 
        ///     instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="DbSchemaKey" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this 
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DbSchemaKey other)
        {
            if (other == null)
            {
                return false;
            }

            return other.innerKey == this.innerKey;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this 
        ///     instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this 
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as DbSchemaKey);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data 
        ///     structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.innerKey.GetHashCode();
        }

        /// <summary>
        ///     Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        ///     A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return this.innerKey;
        }
    }
}
