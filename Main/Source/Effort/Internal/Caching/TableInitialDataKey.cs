// --------------------------------------------------------------------------------------------
// <copyright file="TableInitialDataKey.cs" company="Effort Team">
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
    /// Represents a key the identifies data that was loaded by a data loader component.
    /// </summary>
    internal class TableInitialDataKey : IEquatable<TableInitialDataKey>
    {
        /// <summary>
        /// The type of the data loader.
        /// </summary>
        private Type loaderType;

        /// <summary>
        /// The argument that represent the state of the data loader.
        /// </summary>
        private string loaderArg;

        /// <summary>
        /// The type of the entity that is manifested with the loaded data.
        /// </summary>
        private Type entityType;

        /// <summary>
        /// Initializes a new instance of the <see cref="TableInitialDataKey" /> class.
        /// </summary>
        /// <param name="loaderType">
        /// The type of the data loader.
        /// </param>
        /// <param name="loaderArg">
        /// The argument that represent the state of the data loader.
        /// </param>
        /// <param name="entityType">
        /// The type of the entity that is manifested with the loaded data.
        /// </param>
        public TableInitialDataKey(Type loaderType, string loaderArg, Type entityType)
        {
            this.loaderType = loaderType;
            this.loaderArg = loaderArg ?? string.Empty;
            this.entityType = entityType;
        }

        /// <summary>
        /// Determines whether the specified <see cref="TableInitialDataKey" /> is equal to 
        /// this instance.
        /// </summary>
        /// <param name="obj">
        /// The <see cref="TableInitialDataKey" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="TableInitialDataKey" /> is equal to this 
        ///   instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(TableInitialDataKey other)
        {
            return
                this.loaderType.Equals(other.loaderType) &&
                string.Equals(
                    this.loaderArg, 
                    other.loaderArg, 
                    StringComparison.InvariantCultureIgnoreCase) &&
                this.entityType.Equals(other.entityType);
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
            TableInitialDataKey other = obj as TableInitialDataKey;

            if (other == null)
            {
                return false;
            }

            return this.Equals(other);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data 
        /// structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return 
                this.loaderType.GetHashCode() % 
                this.loaderArg.GetHashCode() % 
                this.entityType.GetHashCode();
        }
    }
}
