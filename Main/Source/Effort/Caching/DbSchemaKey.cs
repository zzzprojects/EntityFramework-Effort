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

namespace Effort.Caching
{
    internal class DbSchemaKey : IEquatable<DbSchemaKey>
    {
        private string[] metadataFiles;

        public DbSchemaKey(string[] metadataFiles)
        {
            this.metadataFiles = metadataFiles
                .Select(f => f ?? string.Empty)
                .Select(f => f.Trim().ToLowerInvariant())
                .OrderBy(f => f)
                .ToArray();
        }

        public bool Equals(DbSchemaKey other)
        {
            if (this.metadataFiles.Length != other.metadataFiles.Length)
            {
                return false;
            }

            for (int i = 0; i < this.metadataFiles.Length; i++)
            {
                if (this.metadataFiles[i] != other.metadataFiles[i])
                {
                    return false;
                }
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            DbSchemaKey other = obj as DbSchemaKey;

            if (obj == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        public override int GetHashCode()
        {
            int result = 0;

            for (int i = 0; i < this.metadataFiles.Length; i++)
            {
                result ^= (i + 1) * this.metadataFiles[i].GetHashCode();
            }

            return result;
        }
    }
}
