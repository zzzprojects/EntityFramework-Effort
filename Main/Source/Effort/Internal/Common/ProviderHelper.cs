// --------------------------------------------------------------------------------------------
// <copyright file="ProviderHelper.cs" company="Effort Team">
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

namespace Effort.Internal.Common
{
    using System;
    using System.Data;
    using System.Data.Common;
#if !EFOLD
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.Entity.Core;
#endif

    internal class ProviderHelper
    {
        public static DbProviderManifest GetProviderManifest(
            string providerInvariantName, 
            string providerManifestToken)
        {

#if !EFOLD
            DbProviderServices providerServices =
                DbConfiguration
                    .DependencyResolver
                    .GetService(
                        typeof(DbProviderServices),
                        providerInvariantName) as DbProviderServices;
                 
#else
            IServiceProvider serviceProvider = 
                DbProviderFactories.GetFactory(providerInvariantName) as IServiceProvider;

            if (serviceProvider == null)
            {
                throw new ProviderIncompatibleException();
            }

            DbProviderServices providerServices = 
                serviceProvider.GetService(typeof(DbProviderServices)) as DbProviderServices;

            if (providerServices == null)
            {
                throw new ProviderIncompatibleException();
            }
#endif
       
            return providerServices.GetProviderManifest(providerManifestToken);
        }

        public static DbConnection CreateConnection(string providerInvariantName)
        {
            DbProviderFactory factory = DbProviderFactories.GetFactory(providerInvariantName);

            return factory.CreateConnection();
        }
    }
}
