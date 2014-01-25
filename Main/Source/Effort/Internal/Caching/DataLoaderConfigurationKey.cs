// --------------------------------------------------------------------------------------------
// <copyright file="DataLoaderConfigurationKey.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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
    using Effort.DataLoaders;

    /// <summary>
    ///     Represents a key that identifies a data loader configuration.
    /// </summary>
    internal class DataLoaderConfigurationKey : IEquatable<DataLoaderConfigurationKey>
    {
        /// <summary>
        ///     The type of the data loader.
        /// </summary>
        private Type type;

        /// <summary>
        ///     The argument of the data loader that describes its complete state.
        /// </summary>
        private string argument;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataLoaderConfigurationKey" /> 
        ///     class.
        /// </summary>
        /// <param name="loader"> The data loader. </param>
        public DataLoaderConfigurationKey(IDataLoader loader)
        {
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }

            this.type = loader.GetType();
            this.argument = loader.Argument ?? string.Empty;
        }

        /// <summary>
        ///     Determines whether the specified <see cref="DataLoaderConfigurationKey" /> is 
        ///     equal to this instance.
        /// </summary>
        /// <param name="other">
        ///     The <see cref="DataLoaderConfigurationKey" /> to compare with this instance.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="DataLoaderConfigurationKey" /> is equal
        ///     to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(DataLoaderConfigurationKey other)
        {
            if (other == null)
            {
                return false;
            }

            return
                this.type.Equals(other.type) &&
                this.argument.Equals(other.argument, StringComparison.InvariantCulture);
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
            return base.Equals(obj as DataLoaderConfigurationKey);
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
            return this.type.GetHashCode() % this.argument.GetHashCode();
        }
    }
}
