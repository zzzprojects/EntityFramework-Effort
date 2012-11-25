// --------------------------------------------------------------------------------------------
// <copyright file="CsvReader.RecordEnumerator.cs" company="Effort Team">
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
    using System.Collections;
    using System.Collections.Generic;

    internal partial class CsvReader
    {
        /// <summary>
        /// Supports a simple iteration over the records of a <see cref="T:CsvReader"/>.
        /// </summary>
        public struct RecordEnumerator
            : IEnumerator<string[]>, IEnumerator
        {
            #region Fields

            /// <summary>
            /// Contains the enumerated <see cref="T:CsvReader"/>.
            /// </summary>
            private CsvReader reader;

            /// <summary>
            /// Contains the current record.
            /// </summary>
            private string[] current;

            /// <summary>
            /// Contains the current record index.
            /// </summary>
            private long currentRecordIndex;

            #endregion

            #region Constructors

            /// <summary>
            /// Initializes a new instance of the <see cref="T:RecordEnumerator"/> class.
            /// </summary>
            /// <param name="reader">The <see cref="T:CsvReader"/> to iterate over.</param>
            /// <exception cref="T:ArgumentNullException">
            ///		<paramref name="reader"/> is a <see langword="null"/>.
            /// </exception>
            public RecordEnumerator(CsvReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }

                this.reader = reader;
                this.current = null;

                this.currentRecordIndex = reader.currentRecordIndex;
            }

            #endregion

            #region IEnumerator<string[]> Members

            /// <summary>
            /// Gets the current record.
            /// </summary>
            public string[] Current
            {
                get { return this.current; }
            }

            /// <summary>
            /// Advances the enumerator to the next record of the CSV.
            /// </summary>
            /// <returns>
            /// <see langword="true"/> if the enumerator was successfully advanced to the next 
            /// record, <see langword="false"/> if the enumerator has passed the end of the CSV.
            /// </returns>
            public bool MoveNext()
            {
                if (this.reader.currentRecordIndex != this.currentRecordIndex)
                {
                    throw new InvalidOperationException(
                        ExceptionMessages.EnumerationVersionCheckFailed);
                }

                if (this.reader.ReadNextRecord())
                {
                    this.current = new string[this.reader.fieldCount];

                    this.reader.CopyCurrentRecordTo(this.current, 0);
                    this.currentRecordIndex = this.reader.currentRecordIndex;

                    return true;
                }
                else
                {
                    this.current = null;
                    this.currentRecordIndex = this.reader.currentRecordIndex;

                    return false;
                }
            }

            #endregion

            #region IEnumerator Members

            /// <summary>
            /// Sets the enumerator to its initial position, which is before the first record in the CSV.
            /// </summary>
            public void Reset()
            {
                if (this.reader.currentRecordIndex != this.currentRecordIndex)
                {
                    throw new InvalidOperationException(
                        ExceptionMessages.EnumerationVersionCheckFailed);
                }

                this.reader.MoveTo(-1);

                this.current = null;
                this.currentRecordIndex = this.reader.currentRecordIndex;
            }

            /// <summary>
            /// Gets the current record.
            /// </summary>
            object IEnumerator.Current
            {
                get
                {
                    if (this.reader.currentRecordIndex != this.currentRecordIndex)
                    {
                        throw new InvalidOperationException(
                            ExceptionMessages.EnumerationVersionCheckFailed);
                    }

                    return this.Current;
                }
            }

            #endregion

            #region IDisposable Members

            /// <summary>
            /// Performs application-defined tasks associated with freeing, releasing, or 
            /// resetting unmanaged resources.
            /// </summary>
            public void Dispose()
            {
                this.reader = null;
                this.current = null;
            }

            #endregion
        }
    }
}