// --------------------------------------------------------------------------------------------
// <copyright file="EffortCommand.cs" company="Effort Team">
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

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Text.RegularExpressions;
    using Effort.Internal.Caching;
    using Effort.Internal.DbManagement;

    /// <summary>
    /// Represents an Effort command that realizes text representations.
    /// </summary>
    public sealed class EffortCommand : EffortCommandBase
    {
        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">
        /// An instance of <see cref="T:System.Data.CommandBehavior" />.
        /// </param>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbDataReader" />.
        /// </returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Executes the query.
        /// </summary>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public override int ExecuteNonQuery()
        {
            // Source:
            //// http://regexadvice.com/forums/thread/55175.aspx

            Regex regex = new Regex(@"CREATE *SCHEMA *\((.*)\)");

            var matches = regex.Matches(this.CommandText.Trim());

            if (matches.Count != 1)
            {
                throw new NotSupportedException();
            }

            string keyString = matches[0].Groups[1].Value;

            DbSchemaKey key = DbSchemaKey.FromString(keyString);
            DbSchema schema = DbSchemaStore.GetDbSchema(key);

            DbContainer container = this.EffortConnection.DbContainer;

            container.Initialize(schema);

            return 0;
        }

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set 
        /// returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>
        /// The first column of the first row in the result set.
        /// </returns>
        public override object ExecuteScalar()
        {
            throw new NotSupportedException();
        }
    }
}
