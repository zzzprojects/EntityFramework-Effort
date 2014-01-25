// --------------------------------------------------------------------------------------------
// <copyright file="ColumnDescription.cs" company="Effort Team">
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

namespace Effort.DataLoaders
{
    using System;

    /// <summary>
    ///     Stores the metadata of a table column.
    /// </summary>
    public sealed class ColumnDescription
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ColumnDescription" /> class.
        /// </summary>
        /// <param name="name"> The name of the column. </param>
        /// <param name="type"> The type of the column. </param>
        internal ColumnDescription(string name, Type type)
        {
            this.Name = name;
            this.Type = type;
        }

        /// <summary>
        ///     Gets the name of the column.
        /// </summary>
        /// <value>
        ///     The name of the column.
        /// </value>
        public string Name { get; private set; }

        /// <summary>
        ///     Gets the type of the column.
        /// </summary>
        /// <value>
        ///     The type of the colum.
        /// </value>
        public Type Type { get; private set; }
    }
}
