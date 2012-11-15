// --------------------------------------------------------------------------------------------
// <copyright file="FieldValue.cs" company="Effort Team">
//     Copyright (C) 2012 Effort Team
//     Copyright (C) 2006 Sébastien Lorion
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

namespace Effort.Internal.Csv
{
    using System;

    /// <summary>
    /// Represent a parsed field value.
    /// </summary>
    internal struct FieldValue
    {
        /// <summary>
        /// Indicates if the field has value.
        /// </summary>
        private bool hasValue;

        /// <summary>
        /// The value of the field.
        /// </summary>
        private string value;

        /// <summary>
        /// Prevents a default instance of the <see cref="FieldValue" /> struct from being 
        /// created.
        /// </summary>
        /// <param name="value">The field if not missing</param>
        /// <param name="hasValue">if set to <c>true</c> the field has value.</param>
        private FieldValue(string value, bool hasValue)
        {
            this.hasValue = hasValue;
            this.value = value;
        }

        /// <summary>
        /// Representing a missing value;
        /// </summary>
        public static readonly FieldValue Missing = new FieldValue(null, false);

        /// <summary>
        /// Gets a value indicating whether the field value is missing
        /// </summary>
        /// <value>
        /// <c>true</c> if the value is missing; otherwise, <c>false</c>.
        /// </value>
        public bool IsMissing 
        {
            get
            {
                return !this.hasValue;
            }
        }

        /// <summary>
        /// Gets the field value.
        /// </summary>
        /// <value>
        /// The field value.
        /// </value>
        /// <exception cref="System.InvalidOperationException">
        /// The field value is missing.
        /// </exception>
        public string Value
        {
            get
            {
                if (!this.hasValue)
                {
                    throw new InvalidOperationException();
                }

                return this.value;
            }
        }

        /// <summary>
        /// Implicit conversion from <see cref="System.String"/> to <see cref="FieldValue"/>.
        /// </summary>
        /// <param name="value">The <see cref="System.String"/> value.</param>
        /// <returns>The <see cref="FieldValue"/> value.</returns>
        public static implicit operator FieldValue(string value)
        {
            return new FieldValue(value, true);
        }

        /// <summary>
        /// Concats a <see cref="FieldValue"/> value with a <see cref="System.String"/> value.
        /// </summary>
        /// <param name="left">The <see cref="FieldValue"/> value.</param>
        /// <param name="right">The <see cref="System.String"/> value.</param>
        /// <returns>The result of the concatenation.</returns>
        public static FieldValue operator +(FieldValue left, string right)
        {
            if (left.IsMissing)
            {
                return right;
            }
            else
            {
                return left.value + right;
            }
        }

        /// <summary>
        /// Concats a <see cref="System.String"/> value with a <see cref="FieldValue"/> value.
        /// </summary>
        /// <param name="left">The <see cref="System.String"/> value.</param>
        /// <param name="right">The <see cref="FieldValue"/> value.</param>
        /// <returns>The result of the concatenation.</returns>
        public static FieldValue operator +(string left, FieldValue right)
        {
            if (right.IsMissing)
            {
                return left;
            }
            else
            {
                return left + right.value;
            }
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (!this.hasValue)
            {
                return "Missing";
            }
            else if (this.value == null)
            {
                return "null";
            }
            else
            {
                return "\"" + this.value + "\"";
            }
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
            if (this.hasValue)
            {
                return 0;
            }
            else if (this.value == null)
            {
                return 1;
            }
            else
            {
                return this.value.GetHashCode();
            }
        }
    }
}
