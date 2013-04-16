// --------------------------------------------------------------------------------------------
// <copyright file="DbCommandWrapper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.
//     This source is subject to the Microsoft Public License (Ms-PL).
//     Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//     All other rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.WrapperProviders
{
    using System;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// DbCommand wrapper base class.
    /// </summary>
    internal class DbCommandWrapper : DbCommand
    {
        private DbCommand wrappedCommand;
        private DbConnection connection;
        private DbCommandDefinitionWrapper definition;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbCommandWrapper"/> class.
        /// </summary>
        /// <param name="wrappedCommand">The wrapped command.</param>
        /// <param name="definition">The definition.</param>
        public DbCommandWrapper(DbCommand wrappedCommand, DbCommandDefinitionWrapper definition)
        {
            this.wrappedCommand = wrappedCommand;
            this.definition = definition;
        }

        /// <summary>
        /// Gets the command definition.
        /// </summary>
        /// <value>The command definition.</value>
        public DbCommandDefinitionWrapper Definition
        {
            get { return this.definition; }
        }

        /// <summary>
        /// Gets or sets the text command to run against the data source.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The text command to execute. The default value is an empty string ("").
        /// </returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "This is a pass-through")]
        public override string CommandText
        {
            get { return this.wrappedCommand.CommandText; }
            set { this.wrappedCommand.CommandText = value; }
        }

        /// <summary>
        /// Gets or sets the wait time before terminating the attempt to execute a command and generating an error.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The time in seconds to wait for the command to execute.
        /// </returns>
        public override int CommandTimeout
        {
            get { return this.wrappedCommand.CommandTimeout; }
            set { this.wrappedCommand.CommandTimeout = value; }
        }

        /// <summary>
        /// Indicates or specifies how the <see cref="P:System.Data.Common.DbCommand.CommandText"/> property is interpreted.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Data.CommandType"/> values. The default is Text.
        /// </returns>
        public override CommandType CommandType
        {
            get { return this.wrappedCommand.CommandType; }
            set { this.wrappedCommand.CommandType = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the command object should be visible in a customized interface control.
        /// </summary>
        /// <value></value>
        /// <returns>true, if the command object should be visible in a control; otherwise false. The default is true.
        /// </returns>
        public override bool DesignTimeVisible
        {
            get { return false; }
            set { }
        }

        /// <summary>
        /// Gets or sets how command results are applied to the <see cref="T:System.Data.DataRow"/> when used by the Update method of a <see cref="T:System.Data.Common.DbDataAdapter"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// One of the <see cref="T:System.Data.UpdateRowSource"/> values. The default is Both unless the command is automatically generated. Then the default is None.
        /// </returns>
        public override UpdateRowSource UpdatedRowSource
        {
            get { return this.wrappedCommand.UpdatedRowSource; }
            set { this.wrappedCommand.UpdatedRowSource = value; }
        }

        /// <summary>
        /// Gets the wrapped command.
        /// </summary>
        /// <value>The wrapped command.</value>
        protected DbCommand WrappedCommand
        {
            get { return this.wrappedCommand; }
        }

        /// <summary>
        /// Gets or sets the <see cref="T:System.Data.Common.DbConnection"/> used by this <see cref="T:System.Data.Common.DbCommand"/>.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The connection to the data source.
        /// </returns>
        protected override DbConnection DbConnection
        {
            get
            {
                return this.connection;
            }

            set
            {
                DbConnectionWrapper conn = (DbConnectionWrapper)value;
                if (conn != null)
                {
                    this.connection = conn;
                    this.wrappedCommand.Connection = conn.WrappedConnection;
                }
                else
                {
                    this.connection = null;
                    this.wrappedCommand.Connection = null;
                }
            }
        }

        /// <summary>
        /// Gets the collection of <see cref="T:System.Data.Common.DbParameter"/> objects.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The parameters of the SQL statement or stored procedure.
        /// </returns>
        protected override DbParameterCollection DbParameterCollection
        {
            get { return this.wrappedCommand.Parameters; }
        }

        /// <summary>
        /// Gets or sets the <see cref="P:System.Data.Common.DbCommand.DbTransaction"/> within which this <see cref="T:System.Data.Common.DbCommand"/> object executes.
        /// </summary>
        /// <value></value>
        /// <returns>
        /// The transaction within which a Command object of a .NET Framework data provider executes. The default value is a null reference (Nothing in Visual Basic).
        /// </returns>
        protected override DbTransaction DbTransaction
        {
            get { return this.wrappedCommand.Transaction; }
            set { this.wrappedCommand.Transaction = value; }
        }

        /// <summary>
        /// Attempts to cancels the execution of a <see cref="T:System.Data.Common.DbCommand"/>.
        /// </summary>
        public override void Cancel()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Executes a SQL statement against a connection object.
        /// </summary>
        /// <returns>The number of rows affected.</returns>
        public override int ExecuteNonQuery()
        {
            return this.wrappedCommand.ExecuteNonQuery();
        }

#if !NET40
        /// <summary>
        /// This is the asynchronous version of <see cref="M:System.Data.Common.DbCommand.ExecuteNonQuery" />. 
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public override Task<int> ExecuteNonQueryAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var cancelled = new TaskCompletionSource<int>();
                cancelled.SetCanceled();

                return cancelled.Task;
            }

            return this.wrappedCommand.ExecuteNonQueryAsync(cancellationToken);
        }
#endif

        /// <summary>
        /// Executes the query and returns the first column of the first row in the result set returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>
        /// The first column of the first row in the result set.
        /// </returns>
        public override object ExecuteScalar()
        {
            return this.wrappedCommand.ExecuteScalar();
        }

#if !NET40
        /// <summary>
        /// This is the asynchronous version of <see cref="M:System.Data.Common.DbCommand.ExecuteScalar" />. 
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        public override Task<object> ExecuteScalarAsync(CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var cancelled = new TaskCompletionSource<object>();
                cancelled.SetCanceled();

                return cancelled.Task;
            }

            return this.wrappedCommand.ExecuteScalarAsync(cancellationToken);
        }
#endif

        /// <summary>
        /// Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare()
        {
            this.wrappedCommand.Prepare();
        }

        /// <summary>
        /// Creates a new instance of a <see cref="T:System.Data.Common.DbParameter"/> object.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbParameter"/> object.
        /// </returns>
        protected override DbParameter CreateDbParameter()
        {
            return this.wrappedCommand.CreateParameter();
        }

        /// <summary>
        /// Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">An instance of <see cref="T:System.Data.CommandBehavior"/>.</param>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbDataReader"/>.
        /// </returns>
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            return this.wrappedCommand.ExecuteReader(behavior);
        }

#if !NET40
        /// <summary>
        /// This is the asynchronous version of <see cref="M:System.Data.Common.DbCommand.ExecuteDbDataReaderAsync" />. 
        /// </summary>
        /// <param name="behavior">Options for statement execution and data retrieval.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// </returns>
        protected override Task<DbDataReader> ExecuteDbDataReaderAsync(CommandBehavior behavior, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
            {
                var cancelled = new TaskCompletionSource<DbDataReader>();
                cancelled.SetCanceled();

                return cancelled.Task;
            }

            return this.wrappedCommand.ExecuteReaderAsync(behavior, cancellationToken);
        }
#endif
    }
}
