// --------------------------------------------------------------------------------------------
// <copyright file="TableName.cs" company="Effort Team">
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

using System;

namespace Effort.Internal.DbManagement.Schema
{
    internal class TableName : IEquatable<TableName>, IComparable<TableName>
    {
        public TableName(string schema, string name)
        {
            this.Schema = schema;
            this.Name = name;
        }

        public string Schema { get; set; }

        public string Name { get; set; }

        public string FullName
        {
            get { return string.Concat(this.Schema, ".", this.Name); }
        }

        public override int GetHashCode()
        {
            return FullName.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            TableName other = obj as TableName;

            if (other == null)
            {
                return false;
            }

            return Equals(other);
        }

        public bool Equals(TableName other)
        {
            return other.FullName == this.FullName;
        }

        public int CompareTo(TableName other)
        {
            return this.FullName.CompareTo(other);
        }
    }
}
