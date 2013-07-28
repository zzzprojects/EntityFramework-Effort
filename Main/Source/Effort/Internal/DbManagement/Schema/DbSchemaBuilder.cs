// --------------------------------------------------------------------------------------------
// <copyright file="DbSchemaBuilder.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
// -------------------------------------------------------------------------------------------

namespace Effort.Internal.DbManagement.Schema
{
    using System.Collections.Generic;
    using System.Linq;

    internal class DbSchemaBuilder
    {
        private readonly IList<DbTableInfoBuilder> tables;
        private readonly IList<DbRelationInfo> relations;

        public DbSchemaBuilder()
        {
            this.tables = new List<DbTableInfoBuilder>();
            this.relations = new List<DbRelationInfo>();
        }

        public void Register(DbTableInfoBuilder table)
        {
            this.tables.Add(table);
        }

        public void Register(DbRelationInfo relation)
        {
            this.relations.Add(relation);
        }

        public DbTableInfoBuilder Find(string tableName)
        {
            return this.tables.FirstOrDefault(b => b.Name == tableName);
        }

        public DbSchema Create()
        {
            return new DbSchema(
                tables:     this.tables.Select(t => t.Create()),
                relations:  this.relations);
        }
    }
}
