// --------------------------------------------------------------------------------------------
// <copyright file="DbTableInfoBuilder.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
// ------------------------------------------------------------------------------------------

namespace Effort.Internal.DbManagement.Schema
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Effort.Internal.DbManagement.Schema.Configuration;
    using NMemory.Indexes;

    internal class DbTableInfoBuilder
    {
        private IList<IKeyInfo> uniqueKeys;
        private IList<IKeyInfo> otherKeys;
        private IList<object> constraintFactories;

        private Type entityType;
        private ILookup<string, PropertyInfo> members;
        public EntityInfo EntityInfo { get; set; }

        public DbTableInfoBuilder()
        {
            this.uniqueKeys = new List<IKeyInfo>();
            this.otherKeys = new List<IKeyInfo>();
            this.constraintFactories = new List<object>();
        }

        public IKeyInfo PrimaryKey { get; set; }

        public MemberInfo IdentityField { get; set; }

        public TableName Name { get; set; }

        public Type EntityType 
        {
            get
            {
                return this.entityType;
            }

            set
            {
                this.entityType = value;

                if (this.entityType != null)
                {
                    this.members = this.entityType
                        .GetProperties()
                        .ToLookup(p => p.Name, p => p);
                }
                else
                {
                    this.members = null;
                }
            }
        }

        protected IEnumerable<IKeyInfo> AllKeys
        {
            get
            {
                return this.AllUniqueKeys.Concat(this.otherKeys);
            }
        }

        protected IEnumerable<IKeyInfo> AllUniqueKeys
        {
            get
            {
                IEnumerable<IKeyInfo> result = Enumerable.Empty<IKeyInfo>();

                if (this.PrimaryKey != null)
                {
                    result = result.Concat(Enumerable.Repeat(this.PrimaryKey, 1));
                }

                return result.Concat(this.uniqueKeys);
            }
        }

        public void AddKey(IKeyInfo key, bool isUnique)
        {
            if (isUnique)
            {
                this.uniqueKeys.Add(key);
            }
            else
            {
                this.otherKeys.Add(key);
            }
        }

        public void AddContraintFactory(object constraintFactory)
        {
            this.constraintFactories.Add(constraintFactory);
        }

        // strictOrder never in true, just for keep info.
        public IKeyInfo FindKey(MemberInfo[] members, bool strictOrder, bool unique)
        {
            if (!strictOrder)
            {
                members = members.OrderBy(m => m.Name).ToArray();
            }

            IEnumerable<IKeyInfo> keys = unique ? this.AllUniqueKeys : this.AllKeys;

            foreach (IKeyInfo key in this.AllKeys)
            {
                MemberInfo[] keyMembers = key.EntityKeyMembers;

                if (!strictOrder)
                {
                    keyMembers = keyMembers.OrderBy(m => m.Name).ToArray();
                }

                if (members.SequenceEqual(keyMembers))
                {
                    return key;
                }
            }

            return null;
        }

        public PropertyInfo FindMember(EntityPropertyInfo property)
        {
            return FindMember(property.Name);
        }

        public PropertyInfo FindMember(string name)
        {
            if (this.members == null)
            {
                return null;
            }

            return this.members[name].FirstOrDefault();
        }

        public DbTableInfo Create()
        {
            return new DbTableInfo(
                tableName:              this.Name,
                entityType:             this.entityType,
                identityField:          this.IdentityField,
                properties:             this.entityType.GetProperties(),
                constraintFactories:    this.constraintFactories.ToArray(),
                primaryKeyInfo:         this.PrimaryKey,
                uniqueKeys:             this.uniqueKeys.ToArray(),
                foreignKeys:            this.otherKeys.ToArray(),
                entityInfo:             this.EntityInfo);
        }
    }
}
