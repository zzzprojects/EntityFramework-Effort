// --------------------------------------------------------------------------------------------
// <copyright file="EffortParameter.cs" company="Effort Team">
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

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    ///     Represents a parameter to a <see cref="T:EffortCommand"/>.
    /// </summary>
    public class EffortParameter : DbParameter
    {
        /// <summary>
        ///     Gets or sets the <see cref="T:System.Data.DbType" /> of the parameter.
        /// </summary>
        /// <returns>
        ///     One of the <see cref="T:System.Data.DbType" /> values. The default is 
        ///     <see cref="F:System.Data.DbType.String" />.
        /// </returns>
        public override DbType DbType
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether the parameter is input-only, 
        ///     output-only, bidirectional, or a stored procedure return value parameter.
        /// </summary>
        /// <returns>
        ///     One of the <see cref="T:System.Data.ParameterDirection" /> values. The default
        ///     is Input.
        /// </returns>
        public override ParameterDirection Direction
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets a value that indicates whether the parameter accepts null values.
        /// </summary>
        /// <returns>
        ///     true if null values are accepted; otherwise false. The default is false.
        /// </returns>
        public override bool IsNullable
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the <see cref="T:System.Data.Common.DbParameter" />.
        /// </summary>
        ///     <returns>The name of the <see cref="T:System.Data.Common.DbParameter" />. The 
        ///     default is an empty string ("").
        /// </returns>
        public override string ParameterName
        {
            get;
            set;
        }

        /// <summary>
        ///     Resets the <see cref="P:DbType" /> property to its original settings.
        /// </summary>
        public override void ResetDbType()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        ///     Gets or sets the maximum size, in bytes, of the data within the column.
        /// </summary>
        /// <returns>
        ///     The maximum size, in bytes, of the data within the column. The default value is 
        ///     inferred from the parameter value.
        /// </returns>
        public override int Size
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the name of the source column mapped to the 
        ///     <see cref="T:System.Data.DataSet" /> and used for loading or returning the 
        ///     <see cref="P:System.Data.Common.DbParameter.Value" />.
        /// </summary>
        /// <returns>
        ///     The name of the source column mapped to the 
        ///     <see cref="T:System.Data.DataSet" />. The default is an empty string.
        /// </returns>
        public override string SourceColumn
        {
            get;
            set;
        }

        /// <summary>
        ///     Sets or gets a value which indicates whether the source column can be null.
        ///     This allows <see cref="T:System.Data.Common.DbCommandBuilder" /> to correctly
        ///     generate Update statements for columns that can be null.
        /// </summary>
        /// <returns>
        ///     true if the source column can be null; false if it is not.
        /// </returns>
        public override bool SourceColumnNullMapping
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the <see cref="T:System.Data.DataRowVersion" /> to use when you
        ///     load <see cref="P:System.Data.Common.DbParameter.Value" />.
        /// </summary>
        /// <returns>
        ///     One of the <see cref="T:System.Data.DataRowVersion" /> values. The default is 
        ///     Current.
        /// </returns>
        public override DataRowVersion SourceVersion
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the value of the parameter.
        /// </summary>
        /// <returns>
        ///     An <see cref="T:System.Object" /> that is the value of the parameter. The 
        ///     default value is null.
        /// </returns>
        public override object Value
        {
            get;
            set;
        }
    }
}
