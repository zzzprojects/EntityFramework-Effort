// --------------------------------------------------------------------------------------------
// <copyright file="DataRowPropertyAttribute.cs" company="Effort Team">
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
// -------------------------------------------------------------------------------------------


namespace Effort.Internal.TypeGeneration
{
    using System;

    /// <summary>
    ///     When applied to the property of a <see cref="DataRow"/> type, specifies the index 
    ///     of the property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
    public class DataRowPropertyAttribute : Attribute
    {
        private readonly int index;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DataRowPropertyAttribute"/> class.
        /// </summary>
        /// <param name="index"> The index of the property. </param>
        public DataRowPropertyAttribute(int index)
        {
            this.index = index;
        }

        /// <summary>
        ///     Gets the index of the property.
        /// </summary>
        /// <value>
        ///     The index.
        /// </value>
        public int Index
        {
            get { return this.index; }
        }
    }
}
