#region License

// Copyright (c) 2011 Effort Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

#endregion

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
        protected EffortWrapperProviderServices(string invariantName)
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
