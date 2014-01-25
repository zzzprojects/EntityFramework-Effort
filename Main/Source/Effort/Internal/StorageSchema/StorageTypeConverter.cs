// --------------------------------------------------------------------------------------------
// <copyright file="StorageTypeConverter.cs" company="Effort Team">
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

namespace Effort.Internal.StorageSchema
{
    using System.Collections.Generic;
#if !EFOLD
    using System.Data.Entity.Core.Metadata.Edm;
#else
    using System.Data.Metadata.Edm;
#endif
    using System.Linq;

    internal class StorageTypeConverter
    {
        private IProviderInformation oldProvider;
        private IProviderInformation newProvider;

        private IDictionary<string, PrimitiveType> oldStoreTypes;

        public StorageTypeConverter(IProviderInformation oldProvider, IProviderInformation newProvider)
        {
            this.oldProvider = oldProvider;
            this.newProvider = newProvider;

            this.oldStoreTypes = this.oldProvider.Manifest.GetStoreTypes().ToDictionary(p => p.Name);
        }

        public bool TryConvertType(string oldStorageTypeName, out string result, out Facet[] facets)
        {
            PrimitiveType oldStorageType = null;

            if (this.oldStoreTypes.TryGetValue(oldStorageTypeName, out oldStorageType))
            {
                TypeUsage oldStorageTypeUsage = TypeUsage.CreateDefaultTypeUsage(oldStorageType);
                facets = oldStorageTypeUsage.Facets.ToArray();

                // TODO: Add injection point
                if (oldStorageType.NamespaceName == "SqlServer" &&
                    (oldStorageType.Name == "timestamp" || oldStorageType.Name == "rowversion"))
                {
                    result = "rowversion";
                    return true;
                }
                else
                {
                    TypeUsage edmType = this.oldProvider.Manifest.GetEdmType(oldStorageTypeUsage);
                    TypeUsage newStorageType = this.newProvider.Manifest.GetStoreType(edmType);

                    result = newStorageType.EdmType.Name;
                    return true;
                }
            }

            facets = new Facet[0];
            result = string.Empty;
            return false;
        }
    }
}
