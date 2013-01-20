// --------------------------------------------------------------------------------------------
// <copyright file="CachingTableDataLoaderKey.cs" company="Effort Team">
//     Copyright (C) 2011-2013 Effort Team
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
    ///     Represents a key the identifies data that was loaded by a data loader component.
    /// </summary>
    internal class CachingTableDataLoaderKey : IEquatable<CachingTableDataLoaderKey>
    {
        /// <summary>
        ///     Identifies the data loader configuration
        /// </summary>
        private DataLoaderConfigurationKey loaderConfiguration;

        /// <summary>
        ///     The name of the table.
        /// </summary>
        private string tableName;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachingTableDataLoaderKey" /> 
        ///     class.
        /// </summary>
        /// <param name="loaderConfiguration">
        ///     Identifies the data loader configuration.
        /// </param>
        /// <param name="tableName">
        ///     The name of the table.
        /// </param>
        public CachingTableDataLoaderKey(
            DataLoaderConfigurationKey loaderConfiguration, 
            string tableName)
        {
            if (loaderConfiguration == null)
            {
                throw new ArgumentNullException("loaderConfiguration");
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentNullException("tableName");
            }

            this.loaderConfiguration = loaderConfiguration;
            this.tableName = tableName;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="CachingTableDataLoaderKey" /> is 
        ///     equal to this instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="CachingTableDataLoaderKey" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="CachingTableDataLoaderKey" /> is equal
        ///     to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(CachingTableDataLoaderKey other)
        {
            if (other == null)
            {
                return false;
            }

            return
                this.loaderConfiguration.Equals(other.loaderConfiguration) &&
                this.tableName.Equals(other.tableName, StringComparison.InvariantCulture);
        }

        /// <summary>
        ///     Determines whether the specified <see cref="System.Object" /> is equal to this 
        ///     instance.
        /// </summary>
        /// <param name="obj">
        ///     The <see cref="System.Object" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="System.Object" /> is equal to this 
        ///     instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as CachingTableDataLoaderKey);
        }

        /// <summary>
        ///     Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        ///     A hash code for this instance, suitable for use in hashing algorithms and data 
        ///     structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return this.loaderConfiguration.GetHashCode() % this.tableName.GetHashCode();
        }
    }
}
