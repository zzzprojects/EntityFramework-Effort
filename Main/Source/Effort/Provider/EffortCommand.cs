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
using System.Data.Common;
using System.Data;
using System.Text.RegularExpressions;
using Effort.Internal.Caching;
using Effort.Internal.DbManagement;

namespace Effort.Provider
{
    public class EffortCommand : EffortCommandBase
    {
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        public override int ExecuteNonQuery()
        {
            // http://regexadvice.com/forums/thread/55175.aspx
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

        public override object ExecuteScalar()
        {
            throw new NotImplementedException();
        }
    }
}
