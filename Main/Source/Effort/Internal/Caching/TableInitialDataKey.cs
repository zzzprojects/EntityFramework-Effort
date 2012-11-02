// ----------------------------------------------------------------------------------
// <copyright file="TableInitialDataKey.cs" company="Effort Team">
//     Copyright (C) 2012 by Effort Team
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
// ----------------------------------------------------------------------------------

namespace Effort.Internal.Caching
{
    using System;

    internal class TableInitialDataKey : IEquatable<TableInitialDataKey>
    {
        private Type loaderType;
        private string loaderArg;
        private Type entityType;

        public TableInitialDataKey(Type loaderType, string loaderArg, Type entityType)
        {
            this.loaderType = loaderType;
            this.loaderArg = loaderArg ?? string.Empty;
            this.entityType = entityType;
        }

        public bool Equals(TableInitialDataKey other)
        {
            return
                this.loaderType.Equals(other.loaderType) &&
                string.Equals(this.loaderArg, other.loaderArg, StringComparison.InvariantCultureIgnoreCase) &&
                this.entityType.Equals(other.entityType);
                
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
            return this.loaderType.GetHashCode() % this.loaderArg.GetHashCode() % this.entityType.GetHashCode();
        }
    }
}
