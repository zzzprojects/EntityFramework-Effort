using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;
using System.Data.Common;
using System.Data.Common.CommandTrees;

namespace Effort.Components
{
    /// <summary>
    /// Implementation of <see cref="DbProviderServices"/> for MMMDBWrapperProvider.
    /// </summary>
    internal class EffortWrapperProviderServices : DbProviderServicesBase
    {
        static EffortWrapperProviderServices()
        {
            string invariantName = string.Empty;

            invariantName = DatabaseAcceleratorProviderConfiguration.ProviderInvariantName;
            EffortWrapperProviderServices.EmulatorInstance = new EffortWrapperProviderServices(invariantName);

            invariantName = DatabaseEmulatorProviderConfiguration.ProviderInvariantName;
            EffortWrapperProviderServices.AcceleratorInstance = new EffortWrapperProviderServices(invariantName);
        }

        internal static EffortWrapperProviderServices EmulatorInstance { private set; get; }
        internal static EffortWrapperProviderServices AcceleratorInstance { private set; get; }

        private string invariantName;

        /// <summary>
        /// Prevents a default instance of the EFSampleProviderServices class from being created.
        /// </summary>
        private EffortWrapperProviderServices(string invariantName)
        {
            this.invariantName = invariantName;
        }


        /// <summary>
        /// Gets the default name of the wrapped provider.
        /// </summary>
        /// <returns>
        /// Default name of the wrapped provider (to be used when
        /// provider is not specified in the connction string)
        /// </returns>
        protected override string DefaultWrappedProviderName
        {
            get { throw new NotSupportedException(); }
        }

        /// <summary>
        /// Gets the provider invariant iname.
        /// </summary>
        /// <returns>Provider invariant name.</returns>
        protected override string ProviderInvariantName 
        {
            get { return this.invariantName; }
        }

        /// <summary>
        /// Creates the command definition wrapper.
        /// </summary>
        /// <param name="wrappedCommandDefinition">The wrapped command definition.</param>
        /// <param name="commandTree">The command tree.</param>
        /// <returns>
        /// The <see cref="DbCommandDefinitionWrapper"/> object.
        /// </returns>
        public override DbCommandDefinitionWrapper CreateCommandDefinitionWrapper(DbCommandDefinition wrappedCommandDefinition, DbCommandTree commandTree)
        {
            return new DbCommandDefinitionWrapper(
                wrappedCommandDefinition, 
                commandTree, 
                (cmd, def) => new EffortWrapperCommand(cmd, def));
        }
    }
}
