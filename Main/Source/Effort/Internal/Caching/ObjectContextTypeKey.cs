// --------------------------------------------------------------------------------------------
// <copyright file="ObjectContextTypeKey.cs" company="Effort Team">
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

namespace Effort.Internal.Caching
{
    using System;

    /// <summary>
    /// Represents a key that identifies dynamically created Effort-ready DbContext types.
    /// </summary>
    internal class ObjectContextTypeKey : IEquatable<ObjectContextTypeKey>
    {
        /// <summary>
        /// The entity connection string that identifies the database instance.
        /// </summary>
        private string entityConnectionString;

        /// <summary>
        /// The effort connection string that containes the database configuration.
        /// </summary>
        private string effortConnectionString;

        /// <summary>
        /// The base type of the ObjectContext.
        /// </summary>
        private string objectContextType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObjectContextTypeKey" /> class.
        /// </summary>
        /// <param name="entityConnectionString">
        /// The entity connection string that identifies the database instance.
        /// </param>
        /// <param name="effortConnectionString">
        /// The effort connection string that containes the database configuration.
        /// </param>
        /// <param name="objectContextType">
        /// The base type of the ObjectContext.
        /// </param>
        public ObjectContextTypeKey(
            string entityConnectionString, 
            string effortConnectionString, 
            Type objectContextType)
        {
            this.entityConnectionString = entityConnectionString ?? string.Empty;
            this.effortConnectionString = effortConnectionString ?? string.Empty;
            this.objectContextType = objectContextType.FullName;
        }

        /// <summary>
        /// Determines whether the specified <see cref="ObjectContextTypeKey" /> is equal to 
        /// this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="ObjectContextTypeKey" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="ObjectContextTypeKey" /> is equal to this 
        ///   instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(ObjectContextTypeKey other)
        {
            if (other == null)
            {
                return false;
            }

            return 
                this.entityConnectionString.Equals(
                    other.entityConnectionString, 
                    StringComparison.InvariantCultureIgnoreCase) &&
                this.effortConnectionString.Equals(
                    other.effortConnectionString, 
                    StringComparison.InvariantCultureIgnoreCase) &&
                this.objectContextType.Equals(
                    other.objectContextType, 
                    StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and 
        /// data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 
                this.entityConnectionString.GetHashCode() % 
                this.effortConnectionString.GetHashCode() % 
                this.objectContextType.GetHashCode();
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this 
        /// instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this 
        ///   instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as ObjectContextTypeKey);
        }
    }
}
