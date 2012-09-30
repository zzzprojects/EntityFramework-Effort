
namespace Effort.Test.Tools.DataReaderInspector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using EFProviderWrapperToolkit;
    using Effort.Provider;
    using System.Data.Common;
    using System.Data.Common.CommandTrees;

    internal class DataReaderInspectorProviderServices : DbProviderServicesBase
    {
        protected override string DefaultWrappedProviderName
        {
            get { return EffortProviderConfiguration.ProviderInvariantName; }
        }

        protected override string ProviderInvariantName
        {
            get { return DataReaderInspectorProviderConfiguration.ProviderInvariantName; }
        }

        public override DbCommandDefinitionWrapper CreateCommandDefinitionWrapper(DbCommandDefinition wrappedCommandDefinition, DbCommandTree commandTree)
        {
            return new DbCommandDefinitionWrapper(
                wrappedCommandDefinition,
                commandTree,
                (tree, definition) => new DataReaderInspectorCommand(tree, definition));
        }
    }
}
