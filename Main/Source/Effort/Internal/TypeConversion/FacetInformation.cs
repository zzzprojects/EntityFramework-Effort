// --------------------------------------------------------------------------------------------
// <copyright file="FacetInformation.cs" company="Effort Team">
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

namespace Effort.Internal.TypeConversion
{
    /// <summary>
    ///     Contains EDM type facet information about a field.
    /// </summary>
    internal class FacetInformation
    {
        /// <summary>
        ///     Gets or sets a value indicating whether the field is nullable.
        /// </summary>
        /// <value>
        ///     <c>true</c> if nullable; otherwise, <c>false</c>.
        /// </value>
        public bool Nullable { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the field is an identity field.
        /// </summary>
        /// <value>
        ///     <c>true</c> if identity field; otherwise, <c>false</c>.
        /// </value>
        public bool Identity { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the field value is computed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if computed; otherwise, <c>false</c>.
        /// </value>
        public bool Computed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether length of the field is limited.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the length of the field is limited; otherwise, <c>false</c>.
        /// </value>
        public bool LimitedLength { get; set; }

        /// <summary>
        ///     Gets or sets the max lenght of the field.
        /// </summary>
        /// <value>
        ///     The max lenght of the field.
        /// </value>
        public int MaxLenght { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the length of the field is fixed.
        /// </summary>
        /// <value>
        ///     <c>true</c> if the length of the field is fixed; otherwise, <c>false</c>.
        /// </value>
        public bool FixedLength { get; set; }
    }
}
