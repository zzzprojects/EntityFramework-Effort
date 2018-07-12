// --------------------------------------------------------------------------------------------
// <copyright file="CsvReader.cs" company="Effort Team">
//     Copyright (C) Effort Team
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
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;

    /// <summary>
    ///     Represents a reader that provides fast, non-cached, forward-only access to CSV 
    ///     data.  
    /// </summary>
    internal partial class CsvReader
        : IDataReader, IEnumerable<string[]>, IDisposable
    {
        #region Constants

        /// <summary>
        ///     Defines the default buffer size.
        /// </summary>
        public const int DefaultBufferSize = 0x1000;

        /// <summary>
        ///     Defines the default delimiter character separating each field.
        /// </summary>
        public const char DefaultDelimiter = ',';

        /// <summary>
        ///     Defines the default quote character wrapping every field.
        /// </summary>
        public const char DefaultQuote = '"';

        /// <summary>
        ///     Defines the default escape character letting insert quotation characters inside
        ///     a quoted field.
        /// </summary>
        public const char DefaultEscape = '"';

        /// <summary>
        ///     Defines the default comment character indicating that a line is commented out.
        /// </summary>
        public const char DefaultComment = '#';

        #endregion

        #region Fields

        /// <summary>
        ///     Contains the field header comparer.
        /// </summary>
        private static readonly StringComparer fieldHeaderComparer = 
            StringComparer.CurrentCultureIgnoreCase;

        #region Settings

        /// <summary>
        ///     Contains the <see cref="T:TextReader"/> pointing to the CSV file.
        /// </summary>
        private TextReader reader;

        /// <summary>
        ///     Contains the buffer size.
        /// </summary>
        private int bufferSize;

        /// <summary>
        ///     Contains the comment character indicating that a line is commented out.
        /// </summary>
        private char comment;

        /// <summary>
        ///     Contains the escape character letting insert quotation characters inside a
        ///     quoted field.
        /// </summary>
        private char escape;

        /// <summary>
        ///     Contains the delimiter character separating each field.
        /// </summary>
        private char delimiter;

        /// <summary>
        ///     Contains the quotation character wrapping every field.
        /// </summary>
        private char quote;

        /// <summary>
        ///     Determines which values should be trimmed.
        /// </summary>
        private ValueTrimmingOptions trimmingOptions;

        /// <summary>
        ///     Indicates if field names are located on the first non commented line.
        /// </summary>
        private bool hasHeaders;

        /// <summary>
        ///     Contains the default action to take when a parsing error has occured.
        /// </summary>
        private ParseErrorAction defaultParseErrorAction;

        /// <summary>
        ///     Contains the action to take when a field is missing.
        /// </summary>
        private MissingFieldAction missingFieldAction;

        /// <summary>
        ///     Indicates if the reader supports multiline.
        /// </summary>
        private bool supportsMultiline;

        /// <summary>
        ///     Indicates if the reader will skip empty lines.
        /// </summary>
        private bool skipEmptyLines;

        #endregion

        #region State

        /// <summary>
        ///     Indicates if the class is initialized.
        /// </summary>
        private bool initialized;

        /// <summary>
        ///     Contains the field headers.
        /// </summary>
        private string[] fieldHeaders;

        /// <summary>
        ///     Contains the dictionary of field indexes by header. The key is the field name
        ///     and the value is its index.
        /// </summary>
        private Dictionary<string, int> fieldHeaderIndexes;

        /// <summary>
        ///     Contains the current record index in the CSV file.
        ///     A value of <see cref="M:Int32.MinValue"/> means that the reader has not been 
        ///     initialized yet. 
        ///     Otherwise, a negative value means that no record has been read yet.
        /// </summary>
        private long currentRecordIndex;

        /// <summary>
        ///     Contains the starting position of the next unread field.
        /// </summary>
        private int nextFieldStart;

        /// <summary>
        ///     Contains the index of the next unread field.
        /// </summary>
        private int nextFieldIndex;

        /// <summary>
        ///     Contains the array of the field values for the current record.
        ///     A null value indicates that the field have not been parsed.
        /// </summary>
        private FieldValue[] fields;

        /// <summary>
        ///     Contains the maximum number of fields to retrieve for each record.
        /// </summary>
        private int fieldCount;

        /// <summary>
        ///     Contains the read buffer.
        /// </summary>
        private char[] buffer;

        /// <summary>
        ///     Contains the current read buffer length.
        /// </summary>
        private int bufferLength;

        /// <summary>
        ///     Indicates if the end of the reader has been reached.
        /// </summary>
        private bool eof;

        /// <summary>
        ///     Indicates if the last read operation reached an EOL character.
        /// </summary>
        private bool eol;

        /// <summary>
        ///     Indicates if the first record is in cache.
        ///     This can happen when initializing a reader with no headers because one record
        ///     must be read to get the field count automatically
        /// </summary>
        private bool firstRecordInCache;

        /// <summary>
        ///     Indicates if one or more field are missing for the current record.
        ///     Resets after each successful record read.
        /// </summary>
        private bool missingFieldFlag;

        /// <summary>
        ///     Indicates if a parse error occured for the current record.
        ///     Resets after each successful record read.
        /// </summary>
        private bool parseErrorFlag;

        /// <summary>
        ///     Contains the disposed status flag.
        /// </summary>
        private bool isDisposed = false;

        /// <summary>
        ///     Contains the locking object for multi-threading purpose.
        /// </summary>
        private readonly object latch = new object();

        #endregion

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented 
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(
            TextReader reader, 
            bool hasHeaders)
            : this(
                reader, 
                hasHeaders, 
                DefaultDelimiter, 
                DefaultQuote, 
                DefaultEscape, 
                DefaultComment, 
                ValueTrimmingOptions.UnquotedOnly, 
                DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented 
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(
            TextReader reader, 
            bool hasHeaders, 
            int bufferSize)
            : this(
                reader, 
                hasHeaders, 
                DefaultDelimiter, 
                DefaultQuote, 
                DefaultEscape, 
                DefaultComment, 
                ValueTrimmingOptions.UnquotedOnly, 
                bufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented 
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="delimiter">
        ///     The delimiter character separating each field (default is ',').
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(
            TextReader reader, 
            bool hasHeaders, 
            char delimiter)
            : this(
                reader, 
                hasHeaders, 
                delimiter, 
                DefaultQuote, 
                DefaultEscape, 
                DefaultComment, 
                ValueTrimmingOptions.UnquotedOnly, 
                DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="delimiter">
        ///     The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(
            TextReader reader,
            bool hasHeaders, 
            char delimiter, 
            int bufferSize)
            : this(
                reader, 
                hasHeaders, 
                delimiter, 
                DefaultQuote, 
                DefaultEscape, 
                DefaultComment, 
                ValueTrimmingOptions.UnquotedOnly, 
                bufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented 
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="delimiter">
        ///     The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="quote">
        ///     The quotation character wrapping every field (default is ''').
        /// </param>
        /// <param name="escape">
        ///     The escape character letting insert quotation characters inside a quoted field 
        ///     (default is '\').
        ///     If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">
        ///     The comment character indicating that a line is commented out (default is '#').
        /// </param>
        /// <param name="trimmingOptions">
        ///     Determines which values should be trimmed.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		Cannot read from <paramref name="reader"/>.
        /// </exception>
        public CsvReader(
            TextReader reader, 
            bool hasHeaders, 
            char delimiter, 
            char quote,
            char escape, 
            char comment, 
            ValueTrimmingOptions trimmingOptions)
            : this(
                reader, 
                hasHeaders, 
                delimiter, 
                quote, 
                escape, 
                comment, 
                trimmingOptions, 
                DefaultBufferSize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CsvReader"/> class.
        /// </summary>
        /// <param name="reader">
        ///     A <see cref="T:TextReader"/> pointing to the CSV file.
        /// </param>
        /// <param name="hasHeaders">
        ///     <see langword="true"/> if field names are located on the first non commented 
        ///     line, otherwise, <see langword="false"/>.
        /// </param>
        /// <param name="delimiter">
        ///     The delimiter character separating each field (default is ',').
        /// </param>
        /// <param name="quote">
        ///     The quotation character wrapping every field (default is ''').
        /// </param>
        /// <param name="escape">
        ///     The escape character letting insert quotation characters inside a quoted field 
        ///     (default is '\').
        ///     If no escape character, set to '\0' to gain some performance.
        /// </param>
        /// <param name="comment">
        ///     The comment character indicating that a line is commented out (default is '#').
        /// </param>
        /// <param name="trimmingOptions">
        ///     Determines which values should be trimmed.
        /// </param>
        /// <param name="bufferSize">
        ///     The buffer size in bytes.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="reader"/> is a <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<paramref name="bufferSize"/> must be 1 or more.
        /// </exception>
        public CsvReader(
            TextReader reader, 
            bool hasHeaders, 
            char delimiter, 
            char quote, 
            char escape, 
            char comment, 
            ValueTrimmingOptions trimmingOptions, 
            int bufferSize)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (bufferSize <= 0)
            {
                throw new ArgumentOutOfRangeException(
                    "bufferSize", 
                    bufferSize, 
                    ExceptionMessages.BufferSizeTooSmall);
            }

            this.bufferSize = bufferSize;

            if (reader is StreamReader)
            {
                Stream stream = ((StreamReader) reader).BaseStream;

                if (stream.CanSeek)
                {
                    // Handle bad implementations returning 0 or less
                    if (stream.Length > 0)
                        this.bufferSize = (int) Math.Min(bufferSize, stream.Length);
                }
            }

            this.reader = reader;
            this.delimiter = delimiter;
            this.quote = quote;
            this.escape = escape;
            this.comment = comment;

            this.hasHeaders = hasHeaders;
            this.trimmingOptions = trimmingOptions;
            this.supportsMultiline = true;
            this.skipEmptyLines = true;
            this.DefaultHeaderName = "Column";

            this.currentRecordIndex = -1;
            this.defaultParseErrorAction = ParseErrorAction.RaiseEvent;
        }

        #endregion

        #region Events

        /// <summary>
        ///     Occurs when there is an error while parsing the CSV stream.
        /// </summary>
        public event EventHandler<ParseErrorEventArgs> ParseError;

        /// <summary>
        ///     Raises the <see cref="M:ParseError"/> event.
        /// </summary>
        /// <param name="e">
        ///     The <see cref="ParseErrorEventArgs"/> that contains the event data.
        /// </param>
        protected virtual void OnParseError(ParseErrorEventArgs e)
        {
            EventHandler<ParseErrorEventArgs> handler = ParseError;

            if (handler != null)
                handler(this, e);
        }

        #endregion

        #region Properties

        #region Settings

        /// <summary>
        ///     Gets the comment character indicating that a line is commented out.
        /// </summary>
        /// <value> The comment character indicating that a line is commented out. </value>
        public char Comment
        {
            get
            {
                return this.comment;
            }
        }

        /// <summary>
        ///     Gets the escape character letting insert quotation characters inside a quoted
        ///     field.
        /// </summary>
        /// <value>
        ///     The escape character letting insert quotation characters inside a quoted field.
        /// </value>
        public char Escape
        {
            get
            {
                return this.escape;
            }
        }

        /// <summary>
        ///     Gets the delimiter character separating each field.
        /// </summary>
        /// <value>
        ///     The delimiter character separating each field.
        /// </value>
        public char Delimiter
        {
            get
            {
                return this.delimiter;
            }
        }

        /// <summary>
        ///     Gets the quotation character wrapping every field.
        /// </summary>
        /// <value>
        ///     The quotation character wrapping every field.
        /// </value>
        public char Quote
        {
            get
            {
                return this.quote;
            }
        }

        /// <summary>
        ///     Indicates if field names are located on the first non commented line.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if field names are located on the first non commented
        ///     line, otherwise, <see langword="false"/>.
        /// </value>
        public bool HasHeaders
        {
            get
            {
                return this.hasHeaders;
            }
        }

        /// <summary>
        ///     Indicates if spaces at the start and end of a field are trimmed.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if spaces at the start and end of a field are trimmed, 
        ///     otherwise, <see langword="false"/>.
        /// </value>
        public ValueTrimmingOptions TrimmingOption
        {
            get
            {
                return this.trimmingOptions;
            }
        }

        /// <summary>
        ///     Gets the buffer size.
        /// </summary>
        public int BufferSize
        {
            get
            {
                return this.bufferSize;
            }
        }

        /// <summary>
        ///     Gets or sets the default action to take when a parsing error has occured.
        /// </summary>
        /// <value>
        ///     The default action to take when a parsing error has occured.
        /// </value>
        public ParseErrorAction DefaultParseErrorAction
        {
            get
            {
                return this.defaultParseErrorAction;
            }

            set
            {
                this.defaultParseErrorAction = value;
            }
        }

        /// <summary>
        ///     Gets or sets the action to take when a field is missing.
        /// </summary>
        /// <value>
        ///     The action to take when a field is missing.
        /// </value>
        public MissingFieldAction MissingFieldAction
        {
            get
            {
                return this.missingFieldAction;
            }

            set
            {
                this.missingFieldAction = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating if the reader supports multiline fields.
        /// </summary>
        /// <value>
        /// A value indicating if the reader supports multiline field.
        /// </value>
        public bool SupportsMultiline
        {
            get
            {
                return this.supportsMultiline;
            }

            set
            {
                this.supportsMultiline = value;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating if the reader will skip empty lines.
        /// </summary>
        /// <value>
        ///     A value indicating if the reader will skip empty lines.
        /// </value>
        public bool SkipEmptyLines
        {
            get
            {
                return this.skipEmptyLines;
            }

            set
            {
                this.skipEmptyLines = value;
            }
        }

        /// <summary>
        ///     Gets or sets the default header name when it is an empty string or only 
        ///     whitespaces.
        ///     The header index will be appended to the specified name.
        /// </summary>
        /// <value>
        ///     The default header name when it is an empty string or only whitespaces.
        /// </value>
        public string DefaultHeaderName 
        { 
            get; 
            
            set; 
        }

        #endregion

        #region State

        /// <summary>
        ///     Gets the maximum number of fields to retrieve for each record.
        /// </summary>
        /// <value>
        ///     The maximum number of fields to retrieve for each record.
        /// </value>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public int FieldCount
        {
            get
            {
                EnsureInitialize();
                return this.fieldCount;
            }
        }

        /// <summary>
        ///     Gets a value that indicates whether the current stream position is at the end
        ///     of the stream.
        /// </summary>
        /// <value>
        ///     <see langword="true"/> if the current stream position is at the end of the
        ///     stream; otherwise <see langword="false"/>.
        /// </value>
        public virtual bool EndOfStream
        {
            get
            {
                return this.eof;
            }
        }

        /// <summary>
        ///     Gets the field headers.
        /// </summary>
        /// <returns>
        ///     The field headers or an empty array if headers are not supported.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public string[] GetFieldHeaders()
        {
            EnsureInitialize();
            Debug.Assert(this.fieldHeaders != null, "Field headers must be non null.");

            string[] fieldHeaders = new string[this.fieldHeaders.Length];

            for (int i = 0; i < fieldHeaders.Length; i++)
                fieldHeaders[i] = this.fieldHeaders[i];

            return fieldHeaders;
        }

        /// <summary>
        ///     Gets the current record index in the CSV file.
        /// </summary>
        /// <value>
        ///     The current record index in the CSV file.
        /// </value>
        public virtual long CurrentRecordIndex
        {
            get
            {
                return this.currentRecordIndex;
            }
        }

        /// <summary>
        ///     Indicates if one or more field are missing for the current record.
        ///     Resets after each successful record read.
        /// </summary>
        public bool MissingFieldFlag
        {
            get { return this.missingFieldFlag; }
        }

        /// <summary>
        ///     Indicates if a parse error occured for the current record.
        ///     Resets after each successful record read.
        /// </summary>
        public bool ParseErrorFlag
        {
            get { return this.parseErrorFlag; }
        }

        #endregion

        #endregion

        #region Indexers

        /// <summary>
        ///     Gets the field with the specified name and record position. 
        /// <see cref="M:hasHeaders"/> must be <see langword="true"/>.
        /// </summary>
        /// <value>
        ///     The field with the specified name and record position.
        /// </value>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="field"/> is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        ///	    The CSV does not have headers (<see cref="M:HasHeaders"/> property is 
        ///	    <see langword="false"/>).
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		<paramref name="field"/> not found.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        ///		Record index must be > 0.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        ///		Cannot move to a previous record in forward-only mode.
        /// </exception>
        /// <exception cref="T:EndOfStreamException">
        ///		Cannot read record at <paramref name="record"/>.
        ///	</exception>
        ///	<exception cref="T:MalformedCsvException">
        ///		The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public string this[int record, string field]
        {
            get
            {
                if (!this.MoveTo(record))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ExceptionMessages.CannotReadRecordAtIndex,
                            record));
                }

                return this[field];
            }
        }

        /// <summary>
        ///     Gets the field at the specified index and record position.
        /// </summary>
        /// <value>
        ///     The field at the specified index and record position.
        ///     A <see langword="null"/> is returned if the field cannot be found for the 
        ///     record.
        /// </value>
        /// <exception cref="T:ArgumentOutOfRangeException">
        ///		<paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        ///		Record index must be > 0.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        ///		Cannot move to a previous record in forward-only mode.
        /// </exception>
        /// <exception cref="T:EndOfStreamException">
        ///		Cannot read record at <paramref name="record"/>.
        /// </exception>
        /// <exception cref="T:MalformedCsvException">
        ///		The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public string this[int record, int field]
        {
            get
            {
                if (!this.MoveTo(record))
                {
                    throw new InvalidOperationException(
                        string.Format(
                            CultureInfo.InvariantCulture,
                            ExceptionMessages.CannotReadRecordAtIndex,
                            record));
                }

                return this[field];
            }
        }

        /// <summary>
        ///     Gets the field with the specified name. <see cref="M:hasHeaders"/> must be 
        ///     <see langword="true"/>.
        /// </summary>
        /// <value>
        ///     The field with the specified name.
        /// </value>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="field"/> is <see langword="null"/> or an empty string.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        ///	    The CSV does not have headers (<see cref="M:HasHeaders"/> property is 
        ///	    <see langword="false"/>).
        /// </exception>
        /// <exception cref="T:ArgumentException">
        ///		<paramref name="field"/> not found.
        /// </exception>
        /// <exception cref="T:MalformedCsvException">
        ///		The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public string this[string field]
        {
            get
            {
                if (string.IsNullOrEmpty(field))
                {
                    throw new ArgumentNullException("field");
                }

                if (!this.hasHeaders)
                {
                    throw new InvalidOperationException(ExceptionMessages.NoHeaders);
                }

                int index = GetFieldIndex(field);

                if (index < 0)
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            ExceptionMessages.FieldHeaderNotFound, field), 
                        "field");
                }
                
                return this[index];
            }
        }

        /// <summary>
        ///     Gets the field at the specified index.
        /// </summary>
        /// <value>
        ///     The field at the specified index.
        /// </value>
        /// <exception cref="T:ArgumentOutOfRangeException">
        ///		<paramref name="field"/> must be included in [0, <see cref="M:FieldCount"/>[.
        /// </exception>
        /// <exception cref="T:InvalidOperationException">
        ///		No record read yet. Call ReadLine() first.
        /// </exception>
        /// <exception cref="T:MalformedCsvException">
        ///		The CSV appears to be corrupt at the current position.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public virtual string this[int field]
        {
            get
            {
                return ReadField(field, false, false).Value;
            }
        }

        #endregion

        #region Methods

        #region EnsureInitialize

        /// <summary>
        ///     Ensures that the reader is initialized.
        /// </summary>
        private void EnsureInitialize()
        {
            if (!this.initialized)
            {
                this.ReadNextRecord(true, false);
            }

            Debug.Assert(this.fieldHeaders != null);
            Debug.Assert(
                this.fieldHeaders.Length > 0 ||
                this.fieldHeaders.Length == 0 && this.fieldHeaderIndexes == null);
        }

        #endregion

        #region GetFieldIndex

        /// <summary>
        ///     Gets the field index for the provided header.
        /// </summary>
        /// <param name="header">
        ///     The header to look for.
        /// </param>
        /// <returns>
        ///     The field index for the provided header. -1 if not found.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public int GetFieldIndex(string header)
        {
            EnsureInitialize();

            int index;

            if (this.fieldHeaderIndexes != null &&
                this.fieldHeaderIndexes.TryGetValue(header, out index))
            {
                return index;
            }
            else
            {
                return -1;
            }
        }

        #endregion

        #region CopyCurrentRecordTo

        /// <summary>
        ///     Copies the field array of the current record to a one-dimensional array,
        ///     starting at the beginning of the target array.
        /// </summary>
        /// <param name="array"> 
        ///     The one-dimensional <see cref="T:Array"/> that is the destination of the fields 
        ///     of the current record.
        /// </param>
        /// <param name="index">
        ///     The zero-based index in <paramref name="array"/> at which copying begins.
        /// </param>
        /// <exception cref="T:ArgumentNullException">
        ///		<paramref name="array"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="T:ArgumentOutOfRangeException">
        ///		<paramref name="index"/> is les than zero or is equal to or greater than the 
        ///		length <paramref name="array"/>. 
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///	    No current record.
        /// </exception>
        /// <exception cref="ArgumentException">
        ///		The number of fields in the record is greater than the available space from 
        ///		<paramref name="index"/> to the end of <paramref name="array"/>.
        /// </exception>
        public void CopyCurrentRecordTo(string[] array, int index)
        {
            if (array == null)
            {
                throw new ArgumentNullException("array");
            }

            if (index < 0 || index >= array.Length)
            {
                throw new ArgumentOutOfRangeException("index", index, string.Empty);
            }

            if (this.currentRecordIndex < 0 || !this.initialized)
            {
                throw new InvalidOperationException(ExceptionMessages.NoCurrentRecord);
            }

            if (array.Length - index < this.fieldCount)
            {
                throw new ArgumentException(ExceptionMessages.NotEnoughSpaceInArray, "array");
            }

            for (int i = 0; i < this.fieldCount; i++)
            {
                if (this.parseErrorFlag)
                {
                    array[index + i] = null;
                }
                else
                {
                    array[index + i] = this[i];
                }
            }
        }

        #endregion

        #region GetCurrentRawData

        /// <summary>
        ///     Gets the current raw CSV data.
        /// </summary>
        /// <remarks> Used for exception handling purpose. </remarks>
        /// <returns> The current raw CSV data. </returns>
        public string GetCurrentRawData()
        {
            if (this.buffer != null && this.bufferLength > 0)
            {
                return new string(this.buffer, 0, this.bufferLength);
            }
            else
            {
                return string.Empty;
            }
        }

        #endregion

        #region IsWhiteSpace

        /// <summary>
        ///     Indicates whether the specified Unicode character is categorized as white 
        ///     space.
        /// </summary>
        /// <param name="c">
        ///     A Unicode character.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if <paramref name="c"/> is white space; otherwise, 
        ///     <see langword="false"/>.
        /// </returns>
        private bool IsWhiteSpace(char c)
        {
            // Handle cases where the delimiter is a whitespace (e.g. tab)
            if (c == this.delimiter)
                return false;
            else
            {
                // See char.IsLatin1(char c) in Reflector
                if (c <= '\x00ff')
                    return (c == ' ' || c == '\t');
                else
                    return 
                        (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c) == 
                        System.Globalization.UnicodeCategory.SpaceSeparator);
            }
        }

        #endregion

        #region MoveTo

        /// <summary>
        ///     Moves to the specified record index.
        /// </summary>
        /// <param name="record">
        ///     The record index.
        /// </param>
        /// <returns>
        ///     <c>true</c> if the operation was successful; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public virtual bool MoveTo(long record)
        {
            if (record < this.currentRecordIndex)
                return false;

            // Get number of record to read
            long offset = record - this.currentRecordIndex;

            while (offset > 0)
            {
                if (!ReadNextRecord())
                {
                    return false;
                }

                offset--;
            }

            return true;
        }

        #endregion

        #region ParseNewLine

        /// <summary>
        ///     Parses a new line delimiter.
        /// </summary>
        /// <param name="pos">
        ///     The starting position of the parsing. Will contain the resulting end position.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if a new line delimiter was found; otherwise, 
        ///     <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private bool ParseNewLine(ref int pos)
        {
            Debug.Assert(pos <= this.bufferLength);

            // Check if already at the end of the buffer
            if (pos == this.bufferLength)
            {
                pos = 0;

                if (!ReadBuffer())
                {
                    return false;
                }
            }

            char c = this.buffer[pos];

            // Treat \r as new line only if it's not the delimiter

            if (c == '\r' && this.delimiter != '\r')
            {
                pos++;

                // Skip following \n (if there is one)

                if (pos < this.bufferLength)
                {
                    if (this.buffer[pos] == '\n')
                    {
                        pos++;
                    }
                }
                else
                {
                    if (ReadBuffer())
                    {
                        if (this.buffer[0] == '\n')
                        {
                            pos = 1;
                        }
                        else
                        {
                            pos = 0;
                        }
                    }
                }

                if (pos >= this.bufferLength)
                {
                    ReadBuffer();
                    pos = 0;
                }

                return true;
            }
            else if (c == '\n')
            {
                pos++;

                if (pos >= this.bufferLength)
                {
                    ReadBuffer();
                    pos = 0;
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Determines whether the character at the specified position is a new line
        ///     delimiter.
        /// </summary>
        /// <param name="pos">
        ///     The position of the character to verify.
        /// </param>
        /// <returns>
        /// 	<see langword="true"/> if the character at the specified position is a new line
        /// 	delimiter; otherwise, <see langword="false"/>.
        /// </returns>
        private bool IsNewLine(int pos)
        {
            Debug.Assert(pos < this.bufferLength);

            char c = this.buffer[pos];

            if (c == '\n')
            {
                return true;
            }
            else if (c == '\r' && this.delimiter != '\r')
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region ReadBuffer

        /// <summary>
        ///     Fills the buffer with data from the reader.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if data was successfully read; otherwise, 
        ///     <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private bool ReadBuffer()
        {
            if (this.eof)
                return false;

            CheckDisposed();

            this.bufferLength = this.reader.Read(this.buffer, 0, this.bufferSize);

            if (this.bufferLength > 0)
            {
                return true;
            }
            else
            {
                this.eof = true;
                this.buffer = null;

                return false;
            }
        }

        #endregion

        #region ReadField

        /// <summary>
        ///     Reads the field at the specified index.
        ///     Any unread fields with an inferior index will also be read as part of the
        ///     required parsing.
        /// </summary>
        /// <param name="field">
        ///     The field index.
        /// </param>
        /// <param name="initializing">
        ///     Indicates if the reader is currently initializing.
        /// </param>
        /// <param name="discardValue">
        ///     Indicates if the value(s) are discarded.
        /// </param>
        /// <returns>
        ///     The field at the specified index. 
        ///     A <see langword="null"/> indicates that an error occured or that the last field
        ///     has been reached during initialization.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">
        ///		<paramref name="field"/> is out of range.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///		There is no current record.
        /// </exception>
        /// <exception cref="MissingFieldCsvException">
        ///		The CSV data appears to be missing a field.
        /// </exception>
        /// <exception cref="MalformedCsvException">
        ///		The CSV data appears to be malformed.
        /// </exception>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private FieldValue ReadField(int field, bool initializing, bool discardValue)
        {
            if (!initializing)
            {
                if (field < 0 || field >= this.fieldCount)
                {
                    throw new ArgumentOutOfRangeException(
                        "field", 
                        field, 
                        string.Format(
                            CultureInfo.InvariantCulture, 
                            ExceptionMessages.FieldIndexOutOfRange, 
                            field));
                }

                if (this.currentRecordIndex < 0)
                {
                    throw new InvalidOperationException(ExceptionMessages.NoCurrentRecord);
                }

                // Directly return field if cached
                if (!this.fields[field].IsMissing)
                {
                    return this.fields[field].Value;
                }
                else if (this.missingFieldFlag)
                {
                    return HandleMissingField(FieldValue.Missing, field, ref this.nextFieldStart);
                }
            }

            CheckDisposed();

            int index = this.nextFieldIndex;

            while (index < field + 1)
            {
                // Handle case where stated start of field is past buffer
                // This can occur because nextFieldStart is simply 1 + last char position of 
                // previous field
                if (this.nextFieldStart == this.bufferLength)
                {
                    this.nextFieldStart = 0;

                    // Possible EOF will be handled later (see Handle_EOF1)
                    ReadBuffer();
                }

                FieldValue value = FieldValue.Missing;

                if (this.missingFieldFlag)
                {
                    value = HandleMissingField(value, index, ref this.nextFieldStart);
                }
                else if (this.nextFieldStart == this.bufferLength)
                {
                    // Handle_EOF1: Handle EOF here

                    // If current field is the requested field, then the value of the field is 
                    // "" as in "f1,f2,f3,(\s*)" otherwise, the CSV is malformed
                    if (index == field)
                    {
                        if (!discardValue)
                        {
                            value = null;
                            this.fields[index] = value;
                        }

                        this.missingFieldFlag = true;
                    }
                    else
                    {
                        value = HandleMissingField(value, index, ref this.nextFieldStart);
                    }
                }
                else
                {
                    // Trim spaces at start
                    if ((this.trimmingOptions & ValueTrimmingOptions.UnquotedOnly) != 0)
                    {
                        SkipWhiteSpaces(ref this.nextFieldStart);
                    }

                    if (this.eof)
                    {
                        value = string.Empty;
                        this.fields[field] = value;

                        if (field < this.fieldCount)
                        {
                            this.missingFieldFlag = true;
                        }
                    }
                    else if (this.buffer[this.nextFieldStart] != this.quote)
                    {
                        // Non-quoted field

                        int start = this.nextFieldStart;
                        int pos = this.nextFieldStart;

                        while (true)
                        {
                            while (pos < this.bufferLength)
                            {
                                char c = this.buffer[pos];

                                if (c == this.delimiter)
                                {
                                    this.nextFieldStart = pos + 1;

                                    break;
                                }
                                else if (c == '\r' || c == '\n')
                                {
                                    this.nextFieldStart = pos;
                                    this.eol = true;

                                    break;
                                }
                                else
                                {
                                    pos++;
                                }
                            }

                            if (pos < this.bufferLength)
                            {
                                break;
                            }
                            else
                            {
                                if (!discardValue)
                                {
                                    value += new string(this.buffer, start, pos - start);
                                }

                                start = 0;
                                pos = 0;
                                this.nextFieldStart = 0;

                                if (!ReadBuffer())
                                {
                                    break;
                                }
                            }
                        }

                        if (!discardValue)
                        {
                            if ((this.trimmingOptions & ValueTrimmingOptions.UnquotedOnly) == 0)
                            {
                                if (!this.eof && pos > start)
                                {
                                    value += new string(this.buffer, start, pos - start);
                                }
                            }
                            else
                            {
                                if (!this.eof && pos > start)
                                {
                                    // Do the trimming
                                    pos--;

                                    while (pos > -1 && IsWhiteSpace(this.buffer[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0)
                                    {
                                        value += new string(this.buffer, start, pos - start);
                                    }
                                }
                                else
                                {
                                    pos = -1;
                                }

                                // If pos <= 0, that means the trimming went past buffer start,
                                // and the concatenated value needs to be trimmed too.
                                if (pos <= 0)
                                {
                                    pos = (value.IsMissing ? -1 : value.Value.Length - 1);

                                    // Do the trimming
                                    while (pos > -1 && IsWhiteSpace(value.Value[pos]))
                                    {
                                        pos--;
                                    }

                                    pos++;

                                    if (pos > 0 && pos != value.Value.Length)
                                    {
                                        value = value.Value.Substring(0, pos);
                                    }
                                }
                            }

                            if (value.IsMissing)
                            {
                                value = null;
                            }
                        }

                        if (this.eol || this.eof)
                        {
                            this.eol = ParseNewLine(ref this.nextFieldStart);

                            // Reaching a new line is ok as long as the parser is initializing 
                            // or it is the last field
                            if (!initializing && index != this.fieldCount - 1)
                            {
                                if (!value.IsMissing && 
                                    (value.Value == null || value.Value.Length == 0))
                                {
                                    value = FieldValue.Missing;
                                }

                                value = HandleMissingField(value, index, ref this.nextFieldStart);
                            }
                        }

                        if (!discardValue)
                        {
                            this.fields[index] = value;
                        }
                    }
                    else
                    {
                        // Skip quote
                        int start = this.nextFieldStart + 1;
                        int pos = start;

                        bool quoted = true;
                        bool escaped = false;

                        if ((this.trimmingOptions & ValueTrimmingOptions.QuotedOnly) != 0)
                        {
                            SkipWhiteSpaces(ref start);
                            pos = start;
                        }

                        while (true)
                        {
                            while (pos < this.bufferLength)
                            {
                                char c = this.buffer[pos];

                                if (escaped)
                                {
                                    escaped = false;
                                    start = pos;
                                }
                                // IF current char is escape AND (escape and quote are 
                                // different OR next char is a quote)
                                else if (c == this.escape && (this.escape != this.quote || (pos + 1 < this.bufferLength && this.buffer[pos + 1] == this.quote) || (pos + 1 == this.bufferLength && this.reader.Peek() == this.quote)))
                                {
                                    if (!discardValue)
                                    {
                                        value += new string(this.buffer, start, pos - start);
                                    }

                                    escaped = true;
                                }
                                else if (c == this.quote)
                                {
                                    quoted = false;
                                    break;
                                }

                                pos++;
                            }

                            if (!quoted)
                            {
                                break;
                            }
                            else
                            {
                                if (!discardValue && !escaped)
                                {
                                    value += new string(this.buffer, start, pos - start);
                                }

                                start = 0;
                                pos = 0;
                                this.nextFieldStart = 0;

                                if (!ReadBuffer())
                                {
                                    HandleParseError(new MalformedCsvException(GetCurrentRawData(), this.nextFieldStart, Math.Max(0, this.currentRecordIndex), index), ref this.nextFieldStart);
                                    return null;
                                }
                            }
                        }

                        if (!this.eof)
                        {
                            // Append remaining parsed buffer content
                            if (!discardValue && pos > start)
                            {
                                value += new string(this.buffer, start, pos - start);
                            }

                            if (!discardValue && 
                                !value.IsMissing && 
                                (this.trimmingOptions & ValueTrimmingOptions.QuotedOnly) != 0)
                            {
                                int newLength = value.Value.Length;

                                while (newLength > 0 && IsWhiteSpace(value.Value[newLength - 1]))
                                {
                                    newLength--;
                                }

                                if (newLength < value.Value.Length)
                                {
                                    value = value.Value.Substring(0, newLength);
                                }
                            }

                            // Skip quote
                            this.nextFieldStart = pos + 1;

                            // Skip whitespaces between the quote and the delimiter/eol
                            SkipWhiteSpaces(ref this.nextFieldStart);

                            // Skip delimiter
                            bool delimiterSkipped;
                            if (this.nextFieldStart < this.bufferLength && this.buffer[this.nextFieldStart] == this.delimiter)
                            {
                                this.nextFieldStart++;
                                delimiterSkipped = true;
                            }
                            else
                            {
                                delimiterSkipped = false;
                            }

                            // Skip new line delimiter if initializing or last field
                            // (if the next field is missing, it will be caught when parsed)
                            if (!this.eof && !delimiterSkipped && (initializing || index == this.fieldCount - 1))
                            {
                                this.eol = ParseNewLine(ref this.nextFieldStart);
                            }

                            // If no delimiter is present after the quoted field and it is not the last field, then it is a parsing error
                            if (!delimiterSkipped && !this.eof && !(this.eol || IsNewLine(this.nextFieldStart)))
                            {
                                HandleParseError(new MalformedCsvException(GetCurrentRawData(), this.nextFieldStart, Math.Max(0, this.currentRecordIndex), index), ref this.nextFieldStart);
                            }
                        }

                        if (!discardValue)
                        {
                            // Resolve missing value
                            if (value.IsMissing)
                            {
                                // Field is quoted, so it shoul be empty
                                value = string.Empty;
                            }

                            this.fields[index] = value;
                        }
                    }
                }

                this.nextFieldIndex = Math.Max(index + 1, this.nextFieldIndex);

                if (index == field)
                {
                    // If initializing, return null to signify the last field has been reached

                    if (initializing)
                    {
                        if (this.eol || this.eof)
                        {
                            return FieldValue.Missing;
                        }
                        else
                        {
                            // Resolve missing value
                            if (value.IsMissing)
                            {
                                // Indicate that its not missing
                                value = null;
                            }

                            return value;
                        }
                    }
                    else
                    {
                        return value;
                    }
                }

                index++;
            }

            // Getting here is bad ...
            HandleParseError(new MalformedCsvException(GetCurrentRawData(), this.nextFieldStart, Math.Max(0, this.currentRecordIndex), index), ref this.nextFieldStart);
            return FieldValue.Missing;
        }

        #endregion

        #region ReadNextRecord

        /// <summary>
        ///     Reads the next record.
        /// </summary>
        /// <returns>
        ///     <see langword="true"/> if a record has been successfully reads; otherwise, 
        ///     <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public bool ReadNextRecord()
        {
            return ReadNextRecord(false, false);
        }

        /// <summary>
        ///     Reads the next record.
        /// </summary>
        /// <param name="onlyReadHeaders">
        ///     Indicates if the reader will proceed to the next record after having read 
        ///     headers.
        ///     <see langword="true"/> if it stops after having read headers; otherwise, 
        ///     <see langword="false"/>.
        /// </param>
        /// <param name="skipToNextLine">
        ///     Indicates if the reader will skip directly to the next line without parsing the 
        ///     current one. 
        ///     To be used when an error occurs.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if a record has been successfully reads; otherwise, 
        ///     <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        protected virtual bool ReadNextRecord(bool onlyReadHeaders, bool skipToNextLine)
        {
            if (this.eof)
            {
                if (this.firstRecordInCache)
                {
                    this.firstRecordInCache = false;
                    this.currentRecordIndex++;

                    return true;
                }
                else
                {
                    return false;
                }
            }

            CheckDisposed();

            if (!this.initialized)
            {
                this.buffer = new char[this.bufferSize];

                // will be replaced if and when headers are read
                this.fieldHeaders = new string[0];

                if (!ReadBuffer())
                {
                    return false;
                }

                if (!SkipEmptyAndCommentedLines(ref this.nextFieldStart))
                {
                    return false;
                }

                // Keep growing this.fields array until the last field has been found
                // and then resize it to its final correct size

                this.fieldCount = 0;
                this.fields = new FieldValue[16];

                while (!ReadField(this.fieldCount, true, false).IsMissing)
                {
                    if (this.parseErrorFlag)
                    {
                        this.fieldCount = 0;
                        Array.Clear(this.fields, 0, this.fields.Length);
                        this.parseErrorFlag = false;
                        this.nextFieldIndex = 0;
                    }
                    else
                    {
                        this.fieldCount++;

                        if (this.fieldCount == this.fields.Length)
                        {
                            Array.Resize<FieldValue>(ref this.fields, (this.fieldCount + 1) * 2);
                        }
                    }
                }

                // fieldCount contains the last field index, but it must contains the field count,
                // so increment by 1
                this.fieldCount++;

                if (this.fields.Length != this.fieldCount)
                {
                    Array.Resize<FieldValue>(ref this.fields, this.fieldCount);
                }

                this.initialized = true;

                // If headers are present, call ReadNextRecord again
                if (this.hasHeaders)
                {
                    // Don't count first record as it was the headers
                    this.currentRecordIndex = -1;

                    this.firstRecordInCache = false;

                    this.fieldHeaders = new string[this.fieldCount];
                    this.fieldHeaderIndexes = new Dictionary<string, int>(this.fieldCount, fieldHeaderComparer);

                    for (int i = 0; i < this.fields.Length; i++)
                    {
                        string headerName = this.fields[i].Value;
                        if (string.IsNullOrEmpty(headerName) || headerName.Trim().Length == 0)
                        {
                            headerName = this.DefaultHeaderName + i.ToString();
                        }

                        this.fieldHeaders[i] = headerName;
                        this.fieldHeaderIndexes.Add(headerName, i);
                    }

                    // Proceed to first record
                    if (!onlyReadHeaders)
                    {
                        // Calling again ReadNextRecord() seems to be simpler, 
                        // but in fact would probably cause many subtle bugs because a derived 
                        // class does not expect a recursive behavior so simply do what is 
                        // needed here and no more.

                        if (!SkipEmptyAndCommentedLines(ref this.nextFieldStart))
                        {
                            return false;
                        }

                        Array.Clear(this.fields, 0, this.fields.Length);
                        this.nextFieldIndex = 0;
                        this.eol = false;

                        this.currentRecordIndex++;
                        return true;
                    }
                }
                else
                {
                    if (onlyReadHeaders)
                    {
                        this.firstRecordInCache = true;
                        this.currentRecordIndex = -1;
                    }
                    else
                    {
                        this.firstRecordInCache = false;
                        this.currentRecordIndex = 0;
                    }
                }
            }
            else
            {
                if (skipToNextLine)
                {
                    SkipToNextLine(ref this.nextFieldStart);
                }
                else if (this.currentRecordIndex > -1 && !this.missingFieldFlag)
                {
                    // If not already at end of record, move there
                    if (!this.eol && !this.eof)
                    {
                        if (!this.supportsMultiline)
                        {
                            SkipToNextLine(ref this.nextFieldStart);
                        }
                        else
                        {
                            // a dirty trick to handle the case where extra fields are present
                            while (!ReadField(this.nextFieldIndex, true, true).IsMissing)
                            {
                            }
                        }
                    }
                }

                if (!this.firstRecordInCache && !SkipEmptyAndCommentedLines(ref this.nextFieldStart))
                {
                    return false;
                }

                if (this.hasHeaders || !this.firstRecordInCache)
                {
                    this.eol = false;
                }

                // Check to see if the first record is in cache.
                // This can happen when initializing a reader with no headers
                // because one record must be read to get the field count automatically
                if (this.firstRecordInCache)
                {
                    this.firstRecordInCache = false;
                }
                else
                {
                    Array.Clear(this.fields, 0, this.fields.Length);
                    this.nextFieldIndex = 0;
                }

                this.missingFieldFlag = false;
                this.parseErrorFlag = false;
                this.currentRecordIndex++;
            }

            return true;
        }

        #endregion

        #region SkipEmptyAndCommentedLines

        /// <summary>
        ///     Skips empty and commented lines.
        ///     If the end of the buffer is reached, its content be discarded and filled again
        ///     from the reader.
        /// </summary>
        /// <param name="pos">
        ///     The position in the buffer where to start parsing. 
        ///     Will contains the resulting position after the operation.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the end of the reader has not been reached; 
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private bool SkipEmptyAndCommentedLines(ref int pos)
        {
            if (pos < this.bufferLength)
            {
                DoSkipEmptyAndCommentedLines(ref pos);
            }

            while (pos >= this.bufferLength && !this.eof)
            {
                if (ReadBuffer())
                {
                    pos = 0;
                    DoSkipEmptyAndCommentedLines(ref pos);
                }
                else
                {
                    return false;
                }
            }

            return !this.eof;
        }

        /// <summary>
        ///     <para>Worker method.</para>
        ///     <para>Skips empty and commented lines.</para>
        /// </summary>
        /// <param name="pos">
        ///     The position in the buffer where to start parsing. 
        ///     Will contains the resulting position after the operation.
        /// </param>
        ///     <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private void DoSkipEmptyAndCommentedLines(ref int pos)
        {
            while (pos < this.bufferLength)
            {
                if (this.buffer[pos] == this.comment)
                {
                    pos++;
                    SkipToNextLine(ref pos);
                }
                else if (this.skipEmptyLines && ParseNewLine(ref pos))
                {
                    continue;
                }
                else
                {
                    break;
                }
            }
        }

        #endregion

        #region SkipWhiteSpaces

        /// <summary>
        ///     Skips whitespace characters.
        /// </summary>
        /// <param name="pos">
        ///     The starting position of the parsing. Will contain the resulting end position.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the end of the reader has not been reached; 
        ///     otherwise, <see langword="false"/>.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private bool SkipWhiteSpaces(ref int pos)
        {
            while (true)
            {
                while (pos < this.bufferLength && IsWhiteSpace(this.buffer[pos]))
                {
                    pos++;
                }

                if (pos < this.bufferLength)
                {
                    break;
                }
                else
                {
                    pos = 0;

                    if (!ReadBuffer())
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        #endregion

        #region SkipToNextLine

        /// <summary>
        ///     Skips ahead to the next NewLine character.
        ///     If the end of the buffer is reached, its content be discarded and filled again 
        ///     from the reader.
        /// </summary>
        /// <param name="pos">
        ///     The position in the buffer where to start parsing. 
        ///     Will contains the resulting position after the operation.
        /// </param>
        /// <returns>
        ///     <see langword="true"/> if the end of the reader has not been reached; 
        ///     otherwise, <see langword="false"/>.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        private bool SkipToNextLine(ref int pos)
        {
            // ((pos = 0) == 0) is a little trick to reset position inline
            while ((pos < this.bufferLength || (ReadBuffer() && ((pos = 0) == 0))) && !ParseNewLine(ref pos))
            {
                pos++;
            }

            return !this.eof;
        }

        #endregion

        #region HandleParseError

        /// <summary>
        ///     Handles a parsing error.
        /// </summary>
        /// <param name="error">
        ///     The parsing error that occured.
        /// </param>
        /// <param name="pos">
        ///     The current position in the buffer.
        /// </param>
        /// <exception cref="ArgumentNullException">
        ///	    <paramref name="error"/> is <see langword="null"/>.
        /// </exception>
        private void HandleParseError(MalformedCsvException error, ref int pos)
        {
            if (error == null)
            {
                throw new ArgumentNullException("error");
            }

            this.parseErrorFlag = true;

            switch (this.defaultParseErrorAction)
            {
                case ParseErrorAction.ThrowException:
                    throw error;

                case ParseErrorAction.RaiseEvent:
                    ParseErrorEventArgs e = 
                        new ParseErrorEventArgs(error, ParseErrorAction.ThrowException);
                    OnParseError(e);

                    switch (e.Action)
                    {
                        case ParseErrorAction.ThrowException:
                            throw e.Error;

                        case ParseErrorAction.RaiseEvent:
                            throw new InvalidOperationException(
                                string.Format(
                                    CultureInfo.InvariantCulture, 
                                    ExceptionMessages.ParseErrorActionInvalidInsideParseErrorEvent, 
                                    e.Action), e.Error);

                        case ParseErrorAction.AdvanceToNextLine:
                            // already at EOL when fields are missing, so don't skip to next line in that case
                            if (!this.missingFieldFlag && pos >= 0)
                            {
                                SkipToNextLine(ref pos);
                            }
                            break;

                        default:
                            throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ParseErrorActionNotSupported, e.Action), e.Error);
                    }
                    break;

                case ParseErrorAction.AdvanceToNextLine:
                    // already at EOL when fields are missing, so don't skip to next line in that case
                    if (!this.missingFieldFlag && pos >= 0)
                    {
                        SkipToNextLine(ref pos);
                    }
                    break;

                default:
                    throw new NotSupportedException(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.ParseErrorActionNotSupported, this.defaultParseErrorAction), error);
            }
        }

        #endregion

        #region HandleMissingField

        /// <summary>
        ///     Handles a missing field error.
        /// </summary>
        /// <param name="value">
        ///     The partially parsed value, if available.
        /// </param>
        /// <param name="fieldIndex">
        ///     The missing field index.
        /// </param>
        /// <param name="currentPosition">
        ///     The current position in the raw data.
        /// </param>
        /// <returns>
        ///     The resulting value according to <see cref="M:MissingFieldAction"/>.
        ///     If the action is set to <see cref="T:MissingFieldAction.TreatAsParseError"/>,
        ///     then the parse error will be handled according to 
        ///     <see cref="DefaultParseErrorAction"/>.
        /// </returns>
        private FieldValue HandleMissingField(FieldValue value, int fieldIndex, ref int currentPosition)
        {
            if (fieldIndex < 0 || fieldIndex >= this.fieldCount)
            {
                throw new ArgumentOutOfRangeException(
                    "fieldIndex", 
                    fieldIndex, 
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        ExceptionMessages.FieldIndexOutOfRange, 
                        fieldIndex));
            }

            this.missingFieldFlag = true;

            for (int i = fieldIndex + 1; i < this.fieldCount; i++)
            {
                this.fields[i] = null;
            }

            if (!value.IsMissing)
            {
                return value;
            }
            else
            {
                switch (this.missingFieldAction)
                {
                    case MissingFieldAction.ParseError:
                        HandleParseError(
                            new MissingFieldCsvException(
                                GetCurrentRawData(),
                                currentPosition, 
                                Math.Max(0, this.currentRecordIndex), 
                                fieldIndex), 
                            ref currentPosition);

                        return FieldValue.Missing;

                    case MissingFieldAction.ReplaceByEmpty:
                        return string.Empty;

                    case MissingFieldAction.ReplaceByNull:
                        return null;

                    default:
                        throw new NotSupportedException(
                            string.Format(
                                CultureInfo.InvariantCulture, 
                                ExceptionMessages.MissingFieldActionNotSupported, 
                                this.missingFieldAction));
                }
            }
        }

        #endregion

        #endregion

        #region IDataReader support methods

        /// <summary>
        ///     Validates the state of the data reader.
        /// </summary>
        /// <param name="validations">
        ///     The validations to accomplish.
        /// </param>
        /// <exception cref="InvalidOperationException">
        ///	    No current record.
        /// </exception>
        /// <exception cref="InvalidOperationException">
        ///	    This operation is invalid when the reader is closed.
        /// </exception>
        private void ValidateDataReader(DataReaderValidations validations)
        {
            if ((validations & DataReaderValidations.IsInitialized) != 0 && !this.initialized)
            {
                throw new InvalidOperationException(ExceptionMessages.NoCurrentRecord);
            }

            if ((validations & DataReaderValidations.IsNotClosed) != 0 && this.isDisposed)
            {
                throw new InvalidOperationException(ExceptionMessages.ReaderClosed);
            }
        }

        /// <summary>
        ///     Copy the value of the specified field to an array.
        /// </summary>
        /// <param name="field">
        ///     The index of the field.
        /// </param>
        /// <param name="fieldOffset">
        ///     The offset in the field value.
        /// </param>
        /// <param name="destinationArray">
        ///     The destination array where the field value will be copied.
        /// </param>
        /// <param name="destinationOffset">
        ///     The destination array offset.
        /// </param>
        /// <param name="length">
        ///     The number of characters to copy from the field value.
        /// </param>
        /// <returns>
        ///     The length.
        /// </returns>
        private long CopyFieldToArray(
            int field, 
            long fieldOffset, 
            Array destinationArray, 
            int destinationOffset, 
            int length)
        {
            EnsureInitialize();

            if (field < 0 || field >= this.fieldCount)
            {
                throw new ArgumentOutOfRangeException("field", field, string.Format(CultureInfo.InvariantCulture, ExceptionMessages.FieldIndexOutOfRange, field));
            }

            if (fieldOffset < 0 || fieldOffset >= int.MaxValue)
            {
                throw new ArgumentOutOfRangeException("fieldOffset");
            }

            // Array.Copy(...) will do the remaining argument checks

            if (length == 0)
            {
                return 0;
            }

            string value = this[field];

            if (value == null)
            {
                value = string.Empty;
            }

            Debug.Assert(fieldOffset < int.MaxValue);
            Debug.Assert(
                destinationArray.GetType() == typeof(char[]) ||
                destinationArray.GetType() == typeof(byte[]));

            if (destinationArray.GetType() == typeof(char[]))
            {
                Array.Copy(value.ToCharArray((int) fieldOffset, length), 0, destinationArray, destinationOffset, length);
            }
            else
            {
                char[] chars = value.ToCharArray((int) fieldOffset, length);
                byte[] source = new byte[chars.Length];
                

                for (int i = 0; i < chars.Length; i++)
                {
                    source[i] = Convert.ToByte(chars[i]);
                }

                Array.Copy(source, 0, destinationArray, destinationOffset, length);
            }

            return length;
        }

        #endregion

        #region IDataReader Members

        int IDataReader.RecordsAffected
        {
            get
            {
                // For SELECT statements, -1 must be returned.
                return -1;
            }
        }

        bool IDataReader.IsClosed
        {
            get
            {
                return this.eof;
            }
        }

        bool IDataReader.NextResult()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            return false;
        }

        void IDataReader.Close()
        {
            Dispose();
        }

        bool IDataReader.Read()
        {
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            return ReadNextRecord();
        }

        int IDataReader.Depth
        {
            get
            {
                ValidateDataReader(DataReaderValidations.IsNotClosed);

                return 0;
            }
        }

        DataTable IDataReader.GetSchemaTable()
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            DataTable schema = new DataTable("SchemaTable");
            schema.Locale = CultureInfo.InvariantCulture;
            schema.MinimumCapacity = this.fieldCount;

            AddColumn(schema, SchemaTableColumn.AllowDBNull, typeof(bool));
            AddColumn(schema, SchemaTableColumn.BaseColumnName, typeof(string));
            AddColumn(schema, SchemaTableColumn.BaseSchemaName, typeof(string));
            AddColumn(schema, SchemaTableColumn.BaseTableName, typeof(string));

            AddColumn(schema, SchemaTableColumn.ColumnName, typeof(string));
            AddColumn(schema, SchemaTableColumn.ColumnOrdinal, typeof(int));
            AddColumn(schema, SchemaTableColumn.ColumnSize, typeof(int));
            AddColumn(schema, SchemaTableColumn.DataType, typeof(object));
            AddColumn(schema, SchemaTableColumn.IsAliased, typeof(bool));
            AddColumn(schema, SchemaTableColumn.IsExpression, typeof(bool));
            AddColumn(schema, SchemaTableColumn.IsKey, typeof(bool));
            AddColumn(schema, SchemaTableColumn.IsLong, typeof(bool));
            AddColumn(schema, SchemaTableColumn.IsUnique, typeof(bool));
            AddColumn(schema, SchemaTableColumn.NumericPrecision, typeof(short));
            AddColumn(schema, SchemaTableColumn.NumericScale, typeof(short));
            AddColumn(schema, SchemaTableColumn.ProviderType, typeof(int));

            AddColumn(schema, SchemaTableOptionalColumn.BaseCatalogName, typeof(string));
            AddColumn(schema, SchemaTableOptionalColumn.BaseServerName, typeof(string));
            AddColumn(schema, SchemaTableOptionalColumn.IsAutoIncrement, typeof(bool));
            AddColumn(schema, SchemaTableOptionalColumn.IsHidden, typeof(bool));
            AddColumn(schema, SchemaTableOptionalColumn.IsReadOnly, typeof(bool));
            AddColumn(schema, SchemaTableOptionalColumn.IsRowVersion, typeof(bool));

            string[] columnNames;

            if (this.hasHeaders)
            {
                columnNames = this.fieldHeaders;
            }
            else
            {
                columnNames = new string[this.fieldCount];

                for (int i = 0; i < this.fieldCount; i++)
                {
                    columnNames[i] = "Column" + i.ToString(CultureInfo.InvariantCulture);
                }
            }

            // null marks columns that will change for each row
            object[] schemaRow = new object[] { 
                    true,					// 00- AllowDBNull
                    null,					// 01- BaseColumnName
                    string.Empty,			// 02- BaseSchemaName
                    string.Empty,			// 03- BaseTableName
                    null,					// 04- ColumnName
                    null,					// 05- ColumnOrdinal
                    int.MaxValue,			// 06- ColumnSize
                    typeof(string),			// 07- DataType
                    false,					// 08- IsAliased
                    false,					// 09- IsExpression
                    false,					// 10- IsKey
                    false,					// 11- IsLong
                    false,					// 12- IsUnique
                    DBNull.Value,			// 13- NumericPrecision
                    DBNull.Value,			// 14- NumericScale
                    (int) DbType.String,	// 15- ProviderType
                    string.Empty,			// 16- BaseCatalogName
                    string.Empty,			// 17- BaseServerName
                    false,					// 18- IsAutoIncrement
                    false,					// 19- IsHidden
                    true,					// 20- IsReadOnly
                    false					// 21- IsRowVersion
              };

            for (int i = 0; i < columnNames.Length; i++)
            {
                schemaRow[1] = columnNames[i]; // Base column name
                schemaRow[4] = columnNames[i]; // Column name
                schemaRow[5] = i; // Column ordinal

                schema.Rows.Add(schemaRow);
            }

            return schema;
        }

        private static void AddColumn(DataTable schema, string columnName, Type type)
        {
            DataColumn column = schema.Columns.Add(columnName, type);
            column.ReadOnly = true;
        }

        #endregion

        #region IDataRecord Members

        int IDataRecord.GetInt32(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            string value = this[i];

            return Int32.Parse(value == null ? string.Empty : value, CultureInfo.CurrentCulture);
        }

        object IDataRecord.this[string name]
        {
            get
            {
                ValidateDataReader(
                    DataReaderValidations.IsInitialized | 
                    DataReaderValidations.IsNotClosed);

                return this[name];
            }
        }

        object IDataRecord.this[int i]
        {
            get
            {
                ValidateDataReader(
                    DataReaderValidations.IsInitialized | 
                    DataReaderValidations.IsNotClosed);

                return this[i];
            }
        }

        object IDataRecord.GetValue(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            if (((IDataRecord)this).IsDBNull(i))
            {
                return DBNull.Value;
            }
            else
            {
                return this[i];
            }
        }

        bool IDataRecord.IsDBNull(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized |
                DataReaderValidations.IsNotClosed);

            return (this[i] == null);
        }

        long IDataRecord.GetBytes(
            int i, 
            long fieldOffset, 
            byte[] buffer, 
            int bufferoffset, 
            int length)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return CopyFieldToArray(i, fieldOffset, buffer, bufferoffset, length);
        }

        byte IDataRecord.GetByte(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Byte.Parse(this[i], CultureInfo.CurrentCulture);
        }

        Type IDataRecord.GetFieldType(int i)
        {
            EnsureInitialize();
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= this.fieldCount)
            {
                throw new ArgumentOutOfRangeException(
                    "i", 
                    i, 
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        ExceptionMessages.FieldIndexOutOfRange, 
                        i));
            }

            return typeof(string);
        }

        decimal IDataRecord.GetDecimal(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Decimal.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetValues(object[] values)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            IDataRecord record = (IDataRecord) this;

            for (int i = 0; i < this.fieldCount; i++)
            {
                values[i] = record.GetValue(i);
            }

            return this.fieldCount;
        }

        string IDataRecord.GetName(int i)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            if (i < 0 || i >= this.fieldCount)
            {
                throw new ArgumentOutOfRangeException(
                    "i", 
                    i, 
                    string.Format(
                        CultureInfo.InvariantCulture, 
                        ExceptionMessages.FieldIndexOutOfRange, 
                        i));
            }

            if (this.hasHeaders)
            {
                return this.fieldHeaders[i];
            }
            else
            {
                return "Column" + i.ToString(CultureInfo.InvariantCulture);
            }
        }

        long IDataRecord.GetInt64(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Int64.Parse(this[i], CultureInfo.CurrentCulture);
        }

        double IDataRecord.GetDouble(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Double.Parse(this[i], CultureInfo.CurrentCulture);
        }

        bool IDataRecord.GetBoolean(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            string value = this[i];

            int result;

            if (Int32.TryParse(value, out result))
            {
                return (result != 0);
            }
            else
            {
                return Boolean.Parse(value);
            }
        }

        Guid IDataRecord.GetGuid(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return new Guid(this[i]);
        }

        DateTime IDataRecord.GetDateTime(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return DateTime.Parse(this[i], CultureInfo.CurrentCulture);
        }

        int IDataRecord.GetOrdinal(string name)
        {
            EnsureInitialize();
            ValidateDataReader(DataReaderValidations.IsNotClosed);

            int index;

            if (!this.fieldHeaderIndexes.TryGetValue(name, out index))
            {
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, ExceptionMessages.FieldHeaderNotFound, name), "name");
            }

            return index;
        }

        string IDataRecord.GetDataTypeName(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return typeof(string).FullName;
        }

        float IDataRecord.GetFloat(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Single.Parse(this[i], CultureInfo.CurrentCulture);
        }

        IDataReader IDataRecord.GetData(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            if (i == 0)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        long IDataRecord.GetChars(
            int i, 
            long fieldoffset, 
            char[] buffer, 
            int bufferoffset, 
            int length)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return CopyFieldToArray(i, fieldoffset, buffer, bufferoffset, length);
        }

        string IDataRecord.GetString(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return this[i];
        }

        char IDataRecord.GetChar(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Char.Parse(this[i]);
        }

        short IDataRecord.GetInt16(int i)
        {
            ValidateDataReader(
                DataReaderValidations.IsInitialized | 
                DataReaderValidations.IsNotClosed);

            return Int16.Parse(this[i], CultureInfo.CurrentCulture);
        }

        #endregion

        #region IEnumerable<string[]> Members

        /// <summary>
        ///     Returns an <see cref="T:RecordEnumerator"/>  that can iterate through CSV 
        ///     records.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:RecordEnumerator"/>  that can iterate through CSV records.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        public CsvReader.RecordEnumerator GetEnumerator()
        {
            return new CsvReader.RecordEnumerator(this);
        }

        /// <summary>
        ///     Returns an <see cref="T:System.Collections.Generics.IEnumerator"/>  that can 
        ///     iterate through CSV records.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Collections.Generics.IEnumerator"/>  that can iterate 
        ///     through CSV records.
        /// </returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	    The instance has been disposed of.
        /// </exception>
        IEnumerator<string[]> IEnumerable<string[]>.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        /// <summary>
        /// Returns an <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator"/>  that can iterate through CSV records.</returns>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        ///	The instance has been disposed of.
        /// </exception>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #region IDisposable members

        /// <summary>
        ///     Gets a value indicating whether the instance has been disposed of.
        /// </summary>
        /// <value>
        /// 	<see langword="true"/> if the instance has been disposed of; otherwise, 
        /// 	<see langword="false"/>.
        /// </value>
        [System.ComponentModel.Browsable(false)]
        public bool IsDisposed
        {
            get { return this.isDisposed; }
        }

        /// <summary>
        ///     Checks if the instance has been disposed of, and if it has, throws an 
        ///     <see cref="T:System.ComponentModel.ObjectDisposedException"/>; otherwise, does
        ///     nothing.
        /// </summary>
        /// <exception cref="T:System.ComponentModel.ObjectDisposedException">
        /// 	The instance has been disposed of.
        /// </exception>
        /// <remarks>
        /// 	Derived classes should call this method at the start of all methods and 
        /// 	properties that should not be accessed after a call to 
        /// 	<see cref="M:Dispose()"/>.
        /// </remarks>
        protected void CheckDisposed()
        {
            if (this.isDisposed)
            {
                throw new ObjectDisposedException(this.GetType().FullName);
            }
        }

        /// <summary>
        ///     Releases all resources used by the instance.
        /// </summary>
        /// <remarks>
        /// 	Calls <see cref="M:Dispose(Boolean)"/> with the disposing parameter set to 
        /// 	<see langword="true"/> to free unmanaged and managed resources.
        /// </remarks>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                Dispose(true);
                GC.SuppressFinalize(this);
            }
        }

        /// <summary>
        ///     Releases the unmanaged resources used by this instance and optionally releases
        ///     the managed resources.
        /// </summary>
        /// <param name="disposing">
        /// 	<see langword="true"/> to release both managed and unmanaged resources; 
        /// 	<see langword="false"/> to release only unmanaged resources.
        /// </param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.isDisposed)
            {
                try
                { 
                    if (disposing)
                    {
                        // Acquire a lock on the object while disposing.
                        if (this.reader != null)
                        {
                            lock (this.latch)
                            {
                                if (this.reader != null)
                                {
                                    this.reader.Dispose();

                                    this.reader = null;
                                    this.buffer = null;
                                    this.eof = true;
                                }
                            }
                        }
                    }
                }
                finally
                {
                    // Ensure that the flag is set
                    this.isDisposed = true;
                }
            }
        }

        /// <summary>
        /// Releases unmanaged resources and performs other cleanup operations before the 
        /// instance is reclaimed by garbage collection.
        /// </summary>
        ~CsvReader()
        {
            Dispose(false);
        }

        #endregion
    }
}