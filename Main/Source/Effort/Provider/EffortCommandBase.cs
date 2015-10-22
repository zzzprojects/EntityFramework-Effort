// --------------------------------------------------------------------------------------------
// <copyright file="EffortCommandBase.cs" company="Effort Team">
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
// --------------------------------------------------------------------------------------------

namespace Effort.Provider
{
    using System;
    using System.Data;
    using System.Data.Common;

    /// <summary>
    ///     Provides a base class for Effort-specific classes that represent commands.
    /// </summary>
    public abstract class EffortCommandBase : DbCommand, ICloneable
    {
        private EffortConnection connection;
        private EffortTransaction transaction;
        private EffortParameterCollection parameters;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EffortCommandBase" /> class.
        /// </summary>
        public EffortCommandBase()
        {
            this.parameters = new EffortParameterCollection();
        }

        /// <summary>
        ///     Gets or sets the text command to run against the data source.
        /// </summary>
        /// <returns>
        ///     The text command to execute. The default value is an empty string ("").
        /// </returns>
        public override string CommandText
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the wait time before terminating the attempt to execute a command 
        ///     and generating an error.
        /// </summary>
        /// <returns>
        ///     The time in seconds to wait for the command to execute.
        /// </returns>
        public override int CommandTimeout
        {
            get;
            set;
        }

        /// <summary>
        ///     Indicates or specifies how the command is interpreted.
        /// </summary>
        /// <returns>
        ///     One of the <see cref="T:System.Data.CommandType" /> values. The default is 
        ///     Text.
        /// </returns>
        public override CommandType CommandType
        {
            get;
            set;
        }

        /// <summary>
        ///     Adds a new parameter with the supplied name.
        /// </summary>
        /// <param name="name"> The name of the parameter. </param>
        protected void AddParameter(string name)
        {
            EffortParameter parameter = new EffortParameter();
            parameter.ParameterName = name;

            this.Parameters.Insert(0, parameter);
        }

        /// <summary>
        ///     Gets the collection of <see cref="EffortParameter" /> objects.
        /// </summary>
        /// <returns> The parameters of the SQL statement or stored procedure. </returns>
        protected override DbParameterCollection DbParameterCollection
        {
            get 
            {
                return this.parameters;
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="EffortConnection" /> used by this command.
        /// </summary>
        /// <returns>
        ///     The connection to the data source.
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Provided connection object is incompatible
        /// </exception>
        protected override DbConnection DbConnection
        {
            get
            {
                return this.connection;
            }

            set
            {
                // Clear connection
                if (value == null)
                {
                    this.connection = null;
                    return;
                }

                EffortConnection newConnection = value as EffortConnection;

                if (newConnection == null)
                {
                    throw new ArgumentException(
                        "Provided connection object is incompatible");
                }

                this.connection = newConnection;
            }
        }

        /// <summary>
        ///     Gets or sets the <see cref="EffortTransaction" /> within which this command 
        ///     executes.
        /// </summary>
        /// <returns>
        ///     The transaction within which a Command object of a .NET Framework data provider 
        ///     executes. The default value is a null reference (Nothing in Visual Basic).
        /// </returns>
        /// <exception cref="System.ArgumentException">
        ///     Provided transaction object is incompatible
        /// </exception>
        protected override DbTransaction DbTransaction
        {
            get
            {
                return this.transaction;
            }

            set
            {
                // Clear transaction
                if (value == null)
                {
                    this.transaction = null;
                    return;
                }

                EffortTransaction newTransaction = value as EffortTransaction;

                if (newTransaction == null)
                {
                    throw new ArgumentException(
                        "Provided transaction object is incompatible");
                }

                this.transaction = newTransaction;
                this.connection = newTransaction.Connection as EffortConnection;
            }
        }

        /// <summary>
        ///     Gets or sets a value indicating whether the command object should be visible in
        ///     a customized interface control.
        /// </summary>
        /// <returns>
        ///     true, if the command object should be visible in a control; otherwise false.
        ///     The default is true.
        /// </returns>
        public override bool DesignTimeVisible
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the strongly typed <see cref="EffortConnection" /> used by this command.
        /// </summary>
        /// <returns>
        ///     The connection to the data source.
        /// </returns>
        protected EffortConnection EffortConnection
        {
            get
            {
                return this.connection;
            }
        }

        /// <summary>
        ///     Gets the strongly typed <see cref="EffortTransaction" /> within which this
        ///     command executes.
        /// </summary>
        /// <returns>
        ///     The transaction within which a Command object of a .NET Framework data provider
        ///     executes. The default value is a null reference (Nothing in Visual Basic).
        /// </returns>
        protected EffortTransaction EffortTransaction
        {
            get
            {
                return this.transaction;
            }
        }

        /// <summary>
        ///     Executes the query.
        /// </summary>
        /// <returns>
        ///     The number of rows affected.
        /// </returns>
        public override abstract int ExecuteNonQuery();

        /// <summary>
        ///     Executes the query and returns the first column of the first row in the result
        ///     set returned by the query. All other columns and rows are ignored.
        /// </summary>
        /// <returns>
        ///     The first column of the first row in the result set.
        /// </returns>
        public override abstract object ExecuteScalar();

        /// <summary>
        ///     Creates a prepared (or compiled) version of the command on the data source.
        /// </summary>
        public override void Prepare()
        {
        }

        /// <summary>
        ///     Gets or sets how command results are applied to the 
        ///     <see cref="T:System.Data.DataRow" /> when used by the Update method of a 
        ///     <see cref="T:System.Data.Common.DbDataAdapter" />.
        /// </summary>
        /// <returns>
        ///     One of the <see cref="T:System.Data.UpdateRowSource" /> values. The default is
        ///     Both unless the command is automatically generated. Then the default is None.
        /// </returns>
        public override UpdateRowSource UpdatedRowSource
        {
            get;
            set;
        }

        /// <summary>
        ///     Attempts to cancels the execution of a 
        ///     <see cref="T:System.Data.Common.DbCommand" />.
        /// </summary>
        public override void Cancel()
        {
        }

        /// <summary>
        ///     Creates a new instance of a <see cref="EffortParameter" /> object.
        /// </summary>
        /// <returns>
        ///     A <see cref="EffortParameter" /> object.
        /// </returns>
        protected override DbParameter CreateDbParameter()
        {
            return new EffortParameter();
        }

        /// <summary>
        ///     Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        ///     A new object that is a copy of this instance.
        /// </returns>
        public abstract object Clone();

        /// <summary>
        ///     Executes the command text against the connection.
        /// </summary>
        /// <param name="behavior">
        ///     An instance of <see cref="T:System.Data.CommandBehavior" />.
        /// </param>
        /// <returns>
        ///     A <see cref="T:System.Data.Common.DbDataReader" />.
        /// </returns>
        protected override abstract DbDataReader ExecuteDbDataReader(CommandBehavior behavior);
    }
}
