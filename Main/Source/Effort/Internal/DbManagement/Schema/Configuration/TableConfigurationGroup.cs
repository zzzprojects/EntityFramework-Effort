// --------------------------------------------------------------------------------------------
// <copyright file="TableConfigurationGroup.cs" company="Effort Team">
//     Copyright (C) 2011-2014 Effort Team
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

namespace Effort.Internal.DbManagement.Schema.Configuration
{
    using System.Collections.Generic;

    internal class TableConfigurationGroup : ITableConfiguration
    {
        private readonly IList<ITableConfiguration> members;

        public TableConfigurationGroup()
        {
            this.members = new List<ITableConfiguration>();
        }

        public void Register<T>()
            where T : ITableConfiguration, new()
        {
            this.members.Add(new T());
        }

        public void Register<T>(T configuration)
            where T : ITableConfiguration
        {
            this.members.Add(configuration);
        }

        public void Configure(EntityInfo entityInfo, DbTableInfoBuilder builder)
        {
            foreach (ITableConfiguration configuration in this.members)
            {
                configuration.Configure(entityInfo, builder);
            }
        }
    }
}
