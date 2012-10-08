using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EFProviderWrapperToolkit;
using Effort.Provider;
using System.Data.Common;
using Effort.Test.Environment.ResultSets;

namespace Effort.Test.Environment.DataReaderInspector
{
    internal class DataReaderInspectorConnection : DbConnectionWrapper
    {
        private IResultSetComposer composer;

        public DataReaderInspectorConnection(IResultSetComposer composer)
        {
            this.composer = composer;
        }

        public IResultSetComposer Composer
        {
            get
            {
                return this.composer;
            }
        }

        protected override string DefaultWrappedProviderName
        {
            get 
            { 
                return EffortProviderConfiguration.ProviderInvariantName; 
            }
        }

        protected override DbProviderFactory DbProviderFactory
        {
            get
            {
                return DataReaderInspectorProviderFactory.Instance;
            }
        }
    }
}
