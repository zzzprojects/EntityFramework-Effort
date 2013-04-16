// --------------------------------------------------------------------------------------------
// <copyright file="DbCommandDefinitionWrapper.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.
//     This source is subject to the Microsoft Public License (Ms-PL).
//     Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//     All other rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------

namespace Effort.Test.Internal.WrapperProviders
{
    using System;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core.Common.CommandTrees;
#else
    using System.Data.Common.CommandTrees;
#endif

    /// <summary>
    /// Implementation of common methods of <see cref="DbCommandDefinition"/> class.
    /// </summary>
    internal class DbCommandDefinitionWrapper : DbCommandDefinition
    {
        private DbCommandDefinition wrappedCommandDefinition;
        private DbCommandTree commandTree;
        private Func<DbCommand, DbCommandDefinitionWrapper, DbCommand> commandCreator;

        /// <summary>
        /// Initializes a new instance of the DbCommandDefinitionWrapper class.
        /// </summary>
        /// <param name="wrappedCommandDefinition">The wrapped command definition.</param>
        /// <param name="commandTree">The command tree.</param>
        /// <param name="commandCreator">The command creator delegate.</param>
        public DbCommandDefinitionWrapper(DbCommandDefinition wrappedCommandDefinition, DbCommandTree commandTree, Func<DbCommand, DbCommandDefinitionWrapper, DbCommand> commandCreator)
        {
            this.wrappedCommandDefinition = wrappedCommandDefinition;
            this.commandTree = commandTree;
            this.commandCreator = commandCreator;
        }

        /// <summary>
        /// Gets the command tree.
        /// </summary>
        /// <value>The command tree.</value>
        public DbCommandTree CommandTree
        {
            get { return this.commandTree; }
        }

        /// <summary>
        /// Gets the wrapped command definition.
        /// </summary>
        /// <value>The wrapped command definition.</value>
        protected DbCommandDefinition WrappedCommandDefinition
        {
            get { return this.wrappedCommandDefinition; }
        }

        /// <summary>
        /// Creates and returns a <see cref="T:System.Data.Common.DbCommandDefinition"/> object associated with the current connection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Data.Common.DbCommandDefinition"/>.
        /// </returns>
        public override DbCommand CreateCommand()
        {
            return this.commandCreator(this.wrappedCommandDefinition.CreateCommand(), this);
        }
    }
}
