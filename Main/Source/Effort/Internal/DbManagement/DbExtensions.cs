// --------------------------------------------------------------------------------------------
// <copyright file="DbExtensions.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Internal.DbManagement
{
    using System.Linq;
    using Effort.Exceptions;
    using Effort.Internal.Common;
    using NMemory;
    using NMemory.Tables;

    internal static class DbExtensions
    {
        public static ITable GetTable(this Database database, string name)
        {
            string cliName = TypeHelper.NormalizeForCliTypeName(name);

            ITable table = database
                .Tables
                .GetAllTables()
                .Where(t => t.EntityType.Name.Equals(cliName))
                .FirstOrDefault();

            if (table == null)
            {
                throw new EffortException(
                    string.Format(ExceptionMessages.TableNotFound, name));
            }

            return table;
        }

        public static bool ContainsTable(this Database database, string name)
        {
            string cliName = TypeHelper.NormalizeForCliTypeName(name);

            return database
                .Tables
                .GetAllTables()
                .Any(t => t.EntityType.Name.Equals(cliName));
        }
    }
}
