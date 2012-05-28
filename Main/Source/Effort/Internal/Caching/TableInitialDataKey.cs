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

namespace Effort.Internal.Caching
{
    internal class TableInitialDataKey : IEquatable<TableInitialDataKey>
    {
        private string path;
        private string tableName;

        public TableInitialDataKey(string path, string tableName)
        {
            this.path = path ?? string.Empty;
            this.tableName = tableName ?? string.Empty;
        }

        public bool Equals(TableInitialDataKey other)
        {
            return
                string.Equals(this.tableName, other.tableName, StringComparison.InvariantCulture) &&
                string.Equals(this.path, other.path, StringComparison.InvariantCultureIgnoreCase);
                
        }

        public override bool Equals(object obj)
        {
            TableInitialDataKey other = obj as TableInitialDataKey;

            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            return this.tableName.GetHashCode() | this.path.GetHashCode();
        }
    }
}
