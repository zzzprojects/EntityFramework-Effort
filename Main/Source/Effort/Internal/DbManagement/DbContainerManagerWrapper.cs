// --------------------------------------------------------------------------------------------
// <copyright file="DbContainerManagerWrapper.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.DbManagement
{
    using System.Collections.Generic;
    using System.Linq;
    using Effort.Internal.Common;
    using Effort.Internal.DbManagement.Engine;
    using Effort.Provider;

    internal class DbContainerManagerWrapper : IDbManager
    {
        private DbContainer container;

        private static readonly string MigrationHistoryTable = "__MigrationHistory";

        public DbContainerManagerWrapper(DbContainer container)
        {
            this.container = container;
        }

        public void SetIdentityFields(bool enabled)
        {
            this.container.SetIdentityFields(enabled);
        }

        public void ClearMigrationHistory()
        {
            var table = this.container.GetTable(MigrationHistoryTable) as IExtendedTable;

            table.Clear();
        }

        public void ClearTables()
        {
            var db = this.container.Internal;
            var specialEntityTypes = GetEntityTypeNames(MigrationHistoryTable);

            this.SetRelations(false);

            foreach (IExtendedTable table in db.Tables.GetAllTables())
            {
                if (specialEntityTypes.Contains(table.ElementType.Name))
                {
                    continue;
                }

                table.Clear();
            }

            this.SetRelations(true);
        }

        private void SetRelations(bool enabled)
        {
            var db = this.container.Internal;

            foreach (var relation in db.Tables.GetAllRelations())
            {
                relation.IsEnabled = enabled;
            }
        }

        private static List<string> GetEntityTypeNames(params string[] tableNames)
        {
            return tableNames
                .Select(x => TypeHelper.NormalizeForCliTypeName(x))
                .ToList();
        }
    }
}
