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
    internal class ObjectContextTypeKey : IEquatable<ObjectContextTypeKey>
    {
        private ConnectionStringKey connectionStringKey;
        private string source;
        private bool shared;
        private string contextType;

        public ObjectContextTypeKey(string connectionString, Type contextType, string source, bool shared)
        {
            this.connectionStringKey = new ConnectionStringKey(connectionString);
            this.source = source ?? string.Empty;
            this.shared = shared;
            this.contextType = contextType.FullName;
        }

        public bool Equals(ObjectContextTypeKey other)
        {
            return
                this.connectionStringKey.Equals(other.connectionStringKey) &&
                this.contextType.Equals(other.contextType) &&
                this.source.Equals(other.source) &&
                this.shared.Equals(other.shared);
        }

        public override bool Equals(object obj)
        {
            ObjectContextTypeKey other = obj as ObjectContextTypeKey;

            if (other == null)
            {
                return false;
            }

            return this.Equals(obj);
        }

        public override int GetHashCode()
        {
            return
                this.connectionStringKey.GetHashCode() ^
                this.contextType.GetHashCode() ^
                this.source.GetHashCode() ^
                (this.shared ? 1 : 0);
        }
    }
}
