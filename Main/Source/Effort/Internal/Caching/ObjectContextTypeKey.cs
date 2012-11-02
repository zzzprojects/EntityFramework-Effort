// ----------------------------------------------------------------------------------
// <copyright file="ObjectContextTypeKey.cs" company="Effort Team">
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

    internal class ObjectContextTypeKey : IEquatable<ObjectContextTypeKey>
    {
        private string entityConnectionString;
        private string effortConnectionString;
        private string objectContextType;

        public ObjectContextTypeKey(string entityConnectionString, string effortConnectionString, Type objectContextType)
        {
            this.entityConnectionString = entityConnectionString ?? string.Empty;
            this.effortConnectionString = effortConnectionString ?? string.Empty;
            this.objectContextType = objectContextType.FullName;
        }


        public bool Equals(ObjectContextTypeKey other)
        {
            if (other == null)
            {
                return false;
            }

            return this.entityConnectionString.Equals(other.entityConnectionString, StringComparison.InvariantCultureIgnoreCase) &&
                this.effortConnectionString.Equals(other.effortConnectionString, StringComparison.InvariantCultureIgnoreCase) &&
                this.objectContextType.Equals(other.objectContextType, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return 
                this.entityConnectionString.GetHashCode() % 
                this.effortConnectionString.GetHashCode() % 
                this.objectContextType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ObjectContextTypeKey);
        }
    }
}
