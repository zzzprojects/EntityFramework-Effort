// --------------------------------------------------------------------------------------------
// <copyright file="DataReaderInspectorCommand.cs" company="Effort Team">
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

namespace Effort.Test.Internal.DataReaderInspector
{
    using System;
    using System.Data;
    using System.Data.Common;
    using Effort.Test.Internal.ResultSets;
    using Effort.Test.Internal.WrapperProviders;

    internal class DataReaderInspectorCommand : DbCommandWrapper
    {
        public DataReaderInspectorCommand(
            DbCommand wrappedCommand,
            DbCommandDefinitionWrapper definition)

            : base(wrappedCommand, definition)
        {
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            IResultSetComposer composer = null;
            DataReaderInspectorConnection connection = this.Connection as DataReaderInspectorConnection;

            if (connection != null)
            {
                 composer = connection.Composer;
            }

            if (composer == null)
            {
                throw new InvalidOperationException();
            }

            return new DataReaderInspectorDataReader(base.ExecuteDbDataReader(behavior), composer);
        }
    }
}
